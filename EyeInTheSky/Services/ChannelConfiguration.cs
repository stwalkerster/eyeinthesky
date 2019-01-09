namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class ChannelConfiguration : ConfigFileBase<IIrcChannel>, IChannelConfiguration
    {
        public ChannelConfiguration(
            IAppConfiguration configuration,
            ILogger logger,
            IChannelFactory channelFactory,
            IFileService fileService) : base(
            configuration.ChannelConfigFile,
            "channels",
            logger,
            channelFactory.NewFromXmlElement,
            channelFactory.ToXmlElement,
            fileService)
        {
        }
        
        public IEnumerable<IStalk> MatchStalks(IRecentChange rc, string channel)
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot match when not initialised!");
            }
            
            SortedList<string, IStalk> stalkListClone;
            lock (this.ItemList)
            {
                stalkListClone = new SortedList<string, IStalk>(
                    this.ItemList.SelectMany(x => x.Value.Stalks.Values)
                        .Where(x => x.WatchChannel == channel)
                        .ToDictionary(x => x.Identifier + "@" + x.Channel));
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