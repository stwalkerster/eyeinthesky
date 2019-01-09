namespace EyeInTheSky.Services
{
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class StalkConfiguration : ConfigFileBase<IStalk>
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
    }
}