namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Prometheus;

    public class ChannelConfiguration : ConfigFileBase<IIrcChannel>, IChannelConfiguration
    {
        public static readonly Histogram IndividualMatchDuration = Metrics
            .CreateHistogram(
                "eyeinthesky_stalk_match_duration_seconds",
                "Histogram of stalk match durations.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.00001, 2, 19),
                    LabelNames = new[] {"channel", "stalk"}
                });
        
        public static readonly Histogram GlobalMatchDuration = Metrics
            .CreateHistogram(
                "eyeinthesky_rc_event_handle_duration_seconds",
                "Histogram of RC event handling durations.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 13)
                });

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

            using (GlobalMatchDuration.NewTimer())
            {
                SortedList<string, IStalk> stalkListClone;
                lock (this.ItemList)
                {
                    stalkListClone = new SortedList<string, IStalk>(
                        this.ItemList.SelectMany(x => x.Value.Stalks.Values)
                            .Where(x => x.WatchChannel == channel)
                            .Where(x => x.IsActive())
                            .ToDictionary(x => x.Identifier + "@" + x.Channel));
                }

                foreach (var s in stalkListClone)
                {
                    bool isMatch;

                    try
                    {
                        using (IndividualMatchDuration.WithLabels(s.Value.Channel, s.Value.Identifier).NewTimer())
                        {
                            isMatch = s.Value.Match(rc);
                        }
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
}