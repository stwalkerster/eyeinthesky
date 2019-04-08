namespace EyeInTheSky.Web.Misc
{
    using System;
    using System.Dynamic;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class DisplayStalk
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IStalkNodeFactory stalkNodeFactory;
        public IStalk Stalk { get; private set; }

        public DisplayStalk(IStalk stalk, IAppConfiguration appConfiguration, IStalkNodeFactory stalkNodeFactory)
        {
            this.appConfiguration = appConfiguration;
            this.stalkNodeFactory = stalkNodeFactory;
            this.Stalk = stalk;
        }

        public DisplayHints StalkDisplayHints
        {
            get
            {
                var result = new DisplayHints();
                result.ColourClass = "";
                result.EnabledIcon = "";
                result.ExpiryIcon = "";

                if (!this.IsEnabled)
                {
                    result.ColourClass = "info";
                    result.EnabledIcon = "fas fa-times-circle";
                    result.Description = "This stalk is currently disabled.";

                    if (this.IsExpired)
                    {
                        result.ExpiryIcon = "fas fa-hourglass-end";
                        result.Description = "This stalk is currently disabled and additionally has expired.";
                    }
                }
                else
                {
                    if (!this.IsExpiryDefined || (!this.IsExpired && !this.IsExpiringSoon))
                    {
                        result.ColourClass = "success";
                        result.EnabledIcon = "fas fa-check-circle";
                        result.Description = "This stalk is currently enabled.";

                        if (this.IsExpiryDefined)
                        {
                            result.ExpiryIcon = "fas fa-hourglass-start";
                            result.Description = "This stalk is currently enabled. An expiry has been defined for this stalk.";
                        }
                    }
                    else
                    {
                        if (this.IsExpiringSoon)
                        {
                            result.ColourClass = "warning";
                            result.ExpiryIcon = "far fa-clock";
                            result.Description = "This stalk is currently enabled, and will be expiring soon.";
                        }

                        if (this.IsExpired)
                        {
                            result.ColourClass = "danger";
                            result.ExpiryIcon = "fas fa-hourglass-end";
                            result.Description = "This stalk has expired.";
                        }
                    }
                }

                return result;
            }
        }

        public bool IsEnabled
        {
            get { return this.Stalk.IsEnabled; }
        }

        public bool IsExpiringSoon
        {
            get { return this.Stalk.IsExpiringSoon(); }
        }

        public bool IsExpiryDefined
        {
            get { return this.Stalk.ExpiryTime.HasValue; }
        }

        public string LastTrigger
        {
            get
            {
                if (!this.Stalk.LastTriggerTime.HasValue || this.Stalk.LastTriggerTime.Value == DateTime.MinValue)
                {
                    return "never";
                }

                return this.Stalk.LastTriggerTime.Value.ToString(this.appConfiguration.DateFormat);
            }
        }

        public string Expiry
        {
            get
            {
                if (!this.Stalk.ExpiryTime.HasValue || this.Stalk.ExpiryTime.Value == DateTime.MinValue)
                {
                    return "never";
                }

                return this.Stalk.ExpiryTime.Value.ToString(this.appConfiguration.DateFormat);
            }
        }

        public string DynamicExpiry
        {
            get
            {
                if (!this.Stalk.DynamicExpiry.HasValue)
                {
                    return "not configured";
                }

                return this.Stalk.DynamicExpiry.Value.ToString(this.appConfiguration.TimeSpanFormat);
            }
        }

        public bool IsExpired
        {
            get { return this.Stalk.ExpiryTime.HasValue && this.Stalk.ExpiryTime.Value < DateTime.Now; }
        }

        public class DisplayHints
        {
            public string Border
            {
                get { return this.ColourClass != null ? "border-" + this.ColourClass : string.Empty; }
            }

            public string Text
            {
                get { return this.ColourClass != null ? "text-" + this.ColourClass : string.Empty; }
            }

            public string Alert
            {
                get { return this.ColourClass != null ? "alert-" + this.ColourClass : string.Empty; }
            }

            public string EnabledIcon { get; set; }
            public string ExpiryIcon { get; set; }

            public bool HasEnabledIcon
            {
                get { return !string.IsNullOrWhiteSpace(this.EnabledIcon); }
            }
            public bool HasExpiryIcon
            {
                get { return !string.IsNullOrWhiteSpace(this.ExpiryIcon); }
            }

            public string ColourClass { get; set; }
            public string Description { get; set; }
        }

        public string Xml
        {
            get
            {
                return this.stalkNodeFactory.ToXml(new XmlDocument(), this.Stalk.SearchTree).OuterXml;
            }
        }
    }
}