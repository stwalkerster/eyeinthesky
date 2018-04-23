namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.SharphConduit;
    using Stwalkerster.SharphConduit.Applications;
    using Stwalkerster.SharphConduit.Applications.Maniphest;

    public class BugReporter : IBugReporter
    {
        private readonly Maniphest maniphest;
        private readonly string projectPhid;
        private readonly bool active;

        public BugReporter(IAppConfiguration appConfig)
        {
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
                    Priority = "normal"
                };
                fod.AddProjects(this.projectPhid);
            }
            else
            {
                fod.AddComment(ex.Description);
            }

            this.maniphest.Edit(fod);
        }
    }
}