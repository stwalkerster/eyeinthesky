namespace EyeInTheSky.Services.Interfaces
{
    using Castle.Core;
    using RabbitMQ.Client;

    public interface IMqService : IStartable
    {
        IModel CreateChannel();
        void ReturnChannel(IModel channel);
    }
}