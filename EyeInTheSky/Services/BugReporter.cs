namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Model;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using Stwalkerster.Bot.PhabricatorLib;
    using Stwalkerster.Bot.PhabricatorLib.Applications;
    using Stwalkerster.Bot.PhabricatorLib.Applications.Maniphest;

    public class BugReporter : IBugReporter
    {
        private readonly ILogger logger;
        private readonly IMqService mqService;
        private readonly RabbitMqConfiguration rabbitConfig;
        private readonly Maniphest maniphest;
        private readonly string projectPhid;
        private readonly bool phabReportingActive;

        public BugReporter(IAppConfiguration appConfig, ILogger logger, IMqService mqService, RabbitMqConfiguration rabbitConfig)
        {
            this.logger = logger;
            this.mqService = mqService;
            this.rabbitConfig = rabbitConfig;
            var url = appConfig.PhabUrl;
            var key = appConfig.PhabToken;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                this.logger.Warn("Reporting to Phabricator is disabled!");
                this.phabReportingActive = false;
            }
            else
            {
                this.phabReportingActive = true;

                var conduitClient = new ConduitClient(url, key);
                this.maniphest = new Maniphest(conduitClient);

                var phidLookup = new PHIDLookup(conduitClient);
                this.projectPhid = phidLookup.GetPHIDForObject("#eits")["#eits"];
            }
        }

        public void ReportBug(LogParseException ex)
        {
            if (this.phabReportingActive)
            {
                this.ReportToPhabricator(ex);
            }

            this.ReportToAmqp(ex);
        }
        
        private void ReportToAmqp(LogParseException logParseException)
        {
            IModel channel = null;
            try
            {
                channel = this.mqService.CreateChannel();
                
                var routingKey = string.Join(".",
                    logParseException.Log,
                    logParseException.EditFlags,
                    logParseException.Channel.Replace(".", "-"));
                
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.AppId = this.rabbitConfig.UserAgent;
                basicProperties.UserId = this.rabbitConfig.Username;
                basicProperties.ContentType = "application/json";
                
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add("log", logParseException.Log);
                basicProperties.Headers.Add("editFlags", logParseException.EditFlags);
                basicProperties.Headers.Add("channel", logParseException.Channel);

                var sw = new StringWriter();
                JsonSerializer.Create().Serialize(sw, logParseException);
                
                var body = Encoding.UTF8.GetBytes(sw.ToString());
                
                channel.BasicPublish(this.rabbitConfig.ObjectPrefix + "errorlog", routingKey, basicProperties, body);
                this.logger.InfoFormat("Reported parse failure of type {0} to AMQP", routingKey);
            }
            finally
            {
                this.mqService.ReturnChannel(channel);
            }
        }

        private void ReportToPhabricator(LogParseException ex)
        {
            var title = string.Format("Unhandled log entry of type {0} / {1}", ex.Log, ex.EditFlags);
            var description = string.Format("```\n{0}\n```\n```\n{1}\n```\nFrom: {2}", ex.Comment, ex.LineData, ex.Channel);
            
            try
            {
                var fulltext = new ApplicationEditorSearchConstraint("query", title);
                var statuses = ManiphestSearchConstraintFactory.Statuses(new List<string> {"open", "stalled"});

                var maniphestTasks = this.maniphest.Search(constraints: new[] {fulltext, statuses});

                var fod = maniphestTasks.FirstOrDefault(x => x.Title == title);
                if (fod == null)
                {
                    fod = new ManiphestTask
                    {
                        Title = title,
                        Description = description,
                        Priority = "normal",
                        Points = 1
                    };
                    fod.AddProjects(this.projectPhid);
                }
                else
                {
                    fod.AddComment(description);
                    fod.Points = fod.Points.GetValueOrDefault(0) + 1;
                }

                this.maniphest.Edit(fod);
            }
            catch (Exception ex2)
            {
                this.logger.Warn("Exception while logging bug", ex2);
                this.logger.Error("Exception encountered while logging bug; including here for reference", ex);
            }
        }
    }
}