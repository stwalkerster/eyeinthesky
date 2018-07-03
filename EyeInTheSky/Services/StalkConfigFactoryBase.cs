namespace EyeInTheSky.Services
{
    using Castle.Core.Logging;
    using EyeInTheSky.Services.Interfaces;

    public abstract class StalkConfigFactoryBase : ConfigFactoryBase
    {
        protected StalkConfigFactoryBase(ILogger logger, IStalkNodeFactory stalkNodeFactory) : base(logger)
        {
            
            this.StalkNodeFactory = stalkNodeFactory;
        }

        protected IStalkNodeFactory StalkNodeFactory { get; set; }
    }
}