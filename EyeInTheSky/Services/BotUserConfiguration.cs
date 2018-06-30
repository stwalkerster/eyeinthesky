namespace EyeInTheSky.Services
{
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    
    public class BotUserConfiguration : ConfigFileBase<IBotUser>, IBotUserConfiguration
    {
        public BotUserConfiguration(
            IAppConfiguration configuration,
            ILogger logger,
            IBotUserFactory userFactory,
            IFileService fileService) : base(
            configuration.UserConfigFile,
            "users",
            logger,
            userFactory.NewFromXmlElement,
            userFactory.ToXmlElement,
            fileService)
        {
        }
    }
}