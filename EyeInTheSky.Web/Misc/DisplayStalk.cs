namespace EyeInTheSky.Web.Misc
{
    using System;
    using System.Dynamic;
    using EyeInTheSky.Model.Interfaces;

    public class DisplayStalk
    {
        private readonly IAppConfiguration appConfiguration;
        public IStalk Stalk { get; private set; }

        public DisplayStalk(IStalk stalk, IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
            this.Stalk = stalk;
        }

        public dynamic StalkDisplayHints
        {
            get
            {
                dynamic result = new ExpandoObject();
                result.Border = "";
                result.Text = "";
                result.EnabledIcon = "";
                result.ExpiryIcon = "";
                result.HasEnabledIcon = false;
                result.HasExpiryIcon = false;

                if (!this.IsEnabled)
                {
                    result.Border = "border-info";
                    result.Text = "text-info";
                    result.EnabledIcon = "fas fa-times-circle";
                    result.HasEnabledIcon = true;

                    if (this.IsExpired)
                    {
                        result.ExpiryIcon = "fas fa-hourglass-end";
                        result.HasExpiryIcon = true;
                    }
                }
                else
                {
                    if (!this.IsExpiryDefined || (!this.IsExpired && !this.IsExpiringSoon))
                    {
                        result.Border = "border-success";
                        result.Text = "text-success";
                        result.EnabledIcon = "fas fa-check-circle";
                        result.HasEnabledIcon = true;

                        if (this.IsExpiryDefined)
                        {
                            result.ExpiryIcon = "fas fa-hourglass-start";
                            result.HasExpiryIcon = true;
                        }
                    }
                    else
                    {
                        if (this.IsExpiringSoon)
                        {
                            result.Border = "border-warning";
                            result.Text = "text-warning";
                            result.ExpiryIcon = "far fa-clock";
                            result.HasExpiryIcon = true;
                        }

                        if (this.IsExpired)
                        {
                            result.Border = "border-danger";
                            result.Text = "text-danger";
                            result.ExpiryIcon = "fas fa-hourglass-end";
                            result.HasExpiryIcon = true;
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

        public bool IsExpired
        {
            get { return this.Stalk.ExpiryTime.HasValue && this.Stalk.ExpiryTime.Value < DateTime.Now; }
        }
    }
}