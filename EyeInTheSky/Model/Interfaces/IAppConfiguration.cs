namespace EyeInTheSky.Model.Interfaces
{
    using Stwalkerster.IrcClient.Model;

    public interface IAppConfiguration
    {
        string FreenodeChannel { get; }
        string WikimediaChannel { get; }
        string CommandPrefix { get; }
        string StalkConfigFile { get; }
        IrcUser RcUser { get; }
        string DateFormat { get; }
        string MediaWikiApiEndpoint { get; }
        string UserAgent { get; }
        EmailConfiguration EmailConfiguration { get; }
        IrcUser Owner { get; }
        int MonitoringPort { get; set; }
    }
}