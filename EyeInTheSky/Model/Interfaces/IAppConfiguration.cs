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
        string IrcAlertFormat { get; }
        string IrcStalkTagSeparator { get; }
        string EmailRcTemplate { get; }
        string EmailStalkTemplate { get; }
        string DateFormat { get; }
        EmailConfiguration EmailConfiguration { get; }
        IrcUser Owner { get; }
    }
}