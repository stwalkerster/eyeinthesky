namespace EyeInTheSky.Web.Misc
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Web.Extensions;
    using Stwalkerster.IrcClient.Model;

    public class ChannelDisplayUser
    {
        public IrcChannelUser Member { get; set; }
        public ChannelUser LocalUser { get; set; }
        public IBotUser GlobalUser { get; set; }

        public string Construction { get; set; }

        public string LocalFlags
        {
            get
            {
                if (this.LocalUser != null)
                {
                    return this.LocalUser.LocalFlags;
                }

                return string.Empty;
            }
        }

        public string GlobalFlags
        {
            get
            {
                if (this.GlobalUser != null)
                {
                    return this.GlobalUser.GlobalFlags;
                }

                return string.Empty;
            }
        }

        public string IsLocallySubscribed
        {
            get
            {
                if (this.LocalUser != null)
                {
                    return this.LocalUser.Subscribed.ToIcon();
                }

                if (this.GlobalUser != null)
                {
                    return false.ToIcon();
                }

                if (this.GlobalUser == null)
                {
                    return false.ToIcon();
                }

                return false.ToIcon();
            }
        }

        public string Account
        {
            get
            {
                if (this.Member != null)
                {
                    return this.Member.User.Account;
                }

                if (this.GlobalUser != null)
                {
                    return this.GlobalUser.Mask.ToString();
                }

                return "(unknown user)";
            }
        }

        public string MemberMask
        {
            get
            {
                if (this.Member != null)
                {
                    return string.Format(
                        "{0}!{1}@{2}",
                        this.Member.User.Nickname,
                        this.Member.User.Username,
                        this.Member.User.Hostname);
                }

                return string.Empty;
            }
        }

        public string IsOperator
        {
            get
            {
                if (this.Member != null)
                {
                    return this.Member.Operator.ToIcon();
                }

                return string.Empty;
            }
        }

        public string IsVoice
        {
            get
            {
                if (this.Member != null)
                {
                    return this.Member.Voice.ToIcon();
                }

                return string.Empty;
            }
        }
    }
}