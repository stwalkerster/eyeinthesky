namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.PhabricatorLib;
    using Stwalkerster.Bot.PhabricatorLib.Applications;
    using Stwalkerster.Bot.PhabricatorLib.Applications.Maniphest;

    public class BugReporter : IBugReporter
    {
        private readonly ILogger logger;
        private readonly Maniphest maniphest;
        private readonly string projectPhid;
        private readonly bool active;

        public BugReporter(IAppConfiguration appConfig, ILogger logger)
        {
            this.logger = logger;
            var url = appConfig.PhabUrl;
            var key = appConfig.PhabToken;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                this.active = false;
                return;
            }
            
            this.active = true;
            
            var conduitClient = new ConduitClient(url, key);
            this.maniphest = new Maniphest(conduitClient);

            var phidLookup = new PHIDLookup(conduitClient);
            this.projectPhid = phidLookup.GetPHIDForObject("#eits")["#eits"];
        }

        public void ReportBug(BugException ex)
        {
            if (!this.active)
            {
                return;
            }

            try
            {
                var fulltext = new ApplicationEditorSearchConstraint("query", ex.Title);
                var statuses = ManiphestSearchConstraintFactory.Statuses(new List<string> {"open", "stalled"});

                var maniphestTasks = this.maniphest.Search(constraints: new[] {fulltext, statuses});

                var fod = maniphestTasks.FirstOrDefault(x => x.Title == ex.Title);
                if (fod == null)
                {
                    fod = new ManiphestTask
                    {
                        Title = ex.Title,
                        Description = ex.Description,
                        Priority = "normal",
                        Points = 1
                    };
                    fod.AddProjects(this.projectPhid);
                }
                else
                {
                    fod.AddComment(ex.Description);
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