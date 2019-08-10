namespace EyeInTheSky.Web.Misc
{
    public class WebConfiguration
    {
        public WebConfiguration(string webServiceHostPort)
        {
            this.WebServiceHostPort = webServiceHostPort;
        }

        public bool DisableErrorTraces { get; set; }
        public bool RewriteLocalhost { get; set; }
        public string WebServiceHostPort { get; set; }
        public string DiagnosticsPassword { get; set; }
        public string DiagnosticsPath { get; set; }
        public string PublicEndpoint { get; set; }
    }
}