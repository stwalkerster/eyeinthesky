namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class StalkConfiguration : ConfigFileBase<IStalk>, IStalkConfiguration
    {
        public StalkConfiguration(
            IAppConfiguration configuration,
            ILogger logger,
            IStalkFactory stalkFactory,
            IFileService fileService)
            : base(configuration.StalkConfigFile,
                "stalks",
                logger,
                stalkFactory.NewFromXmlElement,
                stalkFactory.ToXmlElement,
                fileService)
        {
        }

        public IEnumerable<IStalk> MatchStalks(IRecentChange rc)
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot match when not initialised!");
            }
            
            SortedList<string, IStalk> stalkListClone;
            lock (this.ItemList)
            {
                stalkListClone = new SortedList<string, IStalk>(this.ItemList);
            }
            
            foreach (var s in stalkListClone)
            {
                bool isMatch;
             
                try
                {
                    isMatch = s.Value.Match(rc);
                }
                catch (InvalidOperationException ex)
                {
                    this.Logger.ErrorFormat(ex, "Error during evaluation of stalk {0}", s.Key);
                    // skip this stalk, resume with the others
                    continue;
                }
                
                if (isMatch)
                {
                    yield return s.Value;
                }
            }
        }
    }
}