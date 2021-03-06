﻿namespace EyeInTheSky.Model.Interfaces
{
    public interface IAppConfiguration
    {
        string FreenodeChannel { get; }
        string WikimediaChannel { get; }
        string CommandPrefix { get; }
        string StalkConfigFile { get; }
        string UserConfigFile { get; }
        string ChannelConfigFile { get; }
        string TemplateConfigFile { get; }
        string RcUser { get; }
        string DateFormat { get; }
        EmailConfiguration EmailConfiguration { get; }
        string Owner { get; }
        int MonitoringPort { get; set; }
        int MetricsPort { get; set; }
        string PhabUrl { get; set; }
        string PhabToken { get; set; }
        string PrivacyPolicy { get; set; }
        string TimeSpanFormat { get; }
    }
}