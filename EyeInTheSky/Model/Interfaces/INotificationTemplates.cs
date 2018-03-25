namespace EyeInTheSky.Model.Interfaces
{
    public interface INotificationTemplates
    {
        string EmailRcSubject { get; }
        string EmailRcTemplate { get; }
        string EmailStalkTemplate { get; }
        string EmailGreeting { get; }
        string EmailSignature { get; }
        string EmailStalkReport { get; }
        string EmailStalkReportSubject { get; }
        string IrcAlertFormat { get; }
        string IrcStalkTagSeparator { get; }
    }
}