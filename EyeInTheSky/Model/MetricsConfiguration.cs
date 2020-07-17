namespace EyeInTheSky.Model
{
    public class MetricsConfiguration
    {
        public int Port { get; }

        public MetricsConfiguration(int port)
        {
            this.Port = port;
        }
    }
}