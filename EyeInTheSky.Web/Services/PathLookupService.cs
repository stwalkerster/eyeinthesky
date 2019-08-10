namespace EyeInTheSky.Web.Services
{
    using Castle.Core;
    using EyeInTheSky.Services.Email;
    using EyeInTheSky.Services.Email.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;

    public class PathLookupService : IStartable
    {
        private readonly IEmailTemplateFormatter formatter;
        private readonly IChannelConfiguration channelConfiguration;
        private readonly WebConfiguration webConfig;

        public PathLookupService(IEmailTemplateFormatter formatter, IChannelConfiguration channelConfiguration, WebConfiguration webConfig)
        {
            this.formatter = formatter;
            this.channelConfiguration = channelConfiguration;
            this.webConfig = webConfig;
        }

        private void OnStalkFormat(object sender, StalkInfoFormattingEventArgs e)
        {
            var stalkEndpoint = this.webConfig.PublicEndpoint + "channels/" +
                                this.channelConfiguration[e.Stalk.Channel].Guid + "/stalk/" +
                                e.Stalk.Identifier;

            e.Append("Configuration:          " + stalkEndpoint);
        }

        public void Start()
        {
            this.formatter.OnStalkFormat += this.OnStalkFormat;
        }

        public void Stop()
        {
            this.formatter.OnStalkFormat -= this.OnStalkFormat;
        }
    }
}