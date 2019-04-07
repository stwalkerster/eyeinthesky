namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.Utilities.Encoders;
    using Stwalkerster.IrcClient.Model;

    public class BotUser : IBotUser
    {
        private string emailAddress;

        public BotUser(IrcUserMask mask)
        {
            this.Mask = mask;
            this.WebGuid = new Guid();
        }

        internal BotUser(IrcUserMask mask,
            string flags,
            string emailAddress,
            string emailConfirmationToken,
            string deletionConfirmationToken,
            bool emailAddressConfirmed,
            DateTime? emailConfirmationTimestamp,
            DateTime? deletionConfirmationTimestamp,
            Guid webGuid,
            string webPassword)
        {
            this.Mask = mask;
            this.GlobalFlags = flags;
            this.EmailConfirmationToken = emailConfirmationToken;
            this.DeletionConfirmationToken= deletionConfirmationToken;
            this.EmailAddressConfirmed = emailAddressConfirmed;
            this.EmailConfirmationTimestamp = emailConfirmationTimestamp;
            this.DeletionConfirmationTimestamp = deletionConfirmationTimestamp;
            this.WebGuid = webGuid;
            this.WebPassword = webPassword;
            this.emailAddress = emailAddress;
        }

        public string Identifier
        {
            get { return this.Mask.ToString(); }
        }

        public IrcUserMask Mask { get; private set; }

        public string GlobalFlags { get; set; }

        public string WebPassword { get; set; }
        public Guid WebGuid { get; set; }

        public string EmailAddress
        {
            get { return this.emailAddress; }
            set
            {
                if (value == this.emailAddress && value != null)
                {
                    return;
                }

                this.EmailAddressConfirmed = false;
                this.emailAddress = value;

                if (value == null)
                {
                    this.EmailConfirmationToken = null;
                    this.EmailConfirmationTimestamp = null;
                    return;
                }

                var buf = new byte[15];
                new SecureRandom().NextBytes(buf);

                this.EmailConfirmationToken = Base64.ToBase64String(buf);
                this.EmailConfirmationTimestamp = DateTime.Now.AddHours(1);
            }
        }

        public bool EmailAddressConfirmed { get; private set; }
        public DateTime? EmailConfirmationTimestamp { get; private set; }
        public DateTime? DeletionConfirmationTimestamp { get; private set; }

        public string EmailConfirmationToken { get; private set; }
        public string DeletionConfirmationToken { get; private set; }

        public void GarbageCollectTokens()
        {
            if (this.EmailConfirmationTimestamp < DateTime.Now)
            {
                this.emailAddress = null;
                this.EmailConfirmationToken = null;
                this.EmailAddressConfirmed = false;
                this.EmailConfirmationTimestamp = null;
            }

            if (this.DeletionConfirmationTimestamp < DateTime.Now)
            {
                this.DeletionConfirmationToken = null;
                this.DeletionConfirmationTimestamp = null;
            }
        }

        public string ConfirmEmailAddress(string token)
        {
            if (this.EmailConfirmationToken == null)
            {
                return "no-token";
            }

            if (token != this.EmailConfirmationToken)
            {
                return "token-mismatch";
            }

            if (this.EmailConfirmationTimestamp < DateTime.Now)
            {
                this.emailAddress = null;
                this.EmailConfirmationToken = null;
                this.EmailAddressConfirmed = false;
                this.EmailConfirmationTimestamp = null;
                return "token-expired";
            }

            this.EmailConfirmationToken = null;
            this.EmailAddressConfirmed = true;
            this.EmailConfirmationTimestamp = null;
            return "ok";

        }

        public string ValidateDeleteAccount(string token)
        {
            if (this.DeletionConfirmationToken == null)
            {
                return "no-token";
            }

            if (token != this.DeletionConfirmationToken)
            {
                return "token-mismatch";
            }

            if (this.DeletionConfirmationTimestamp < DateTime.Now)
            {
                this.DeletionConfirmationToken = null;
                this.DeletionConfirmationTimestamp = null;
                return "token-expired";
            }

            return "ok";
        }

        public void PrepareDeleteAccount()
        {
            var buf = new byte[15];
            new SecureRandom().NextBytes(buf);

            this.DeletionConfirmationToken = Base64.ToBase64String(buf);
            this.DeletionConfirmationTimestamp = DateTime.Now.AddHours(1);
        }

        public override string ToString()
        {
            return string.Format("Mask: {0}, GlobalFlags: {1}", this.Mask, this.GlobalFlags);
        }
    }
}