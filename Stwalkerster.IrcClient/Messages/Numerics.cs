namespace Stwalkerster.IrcClient.Messages
{
    /// <summary>
    /// The IRC numeric commands.
    /// </summary>
    public class Numerics
    {
        /// <summary>
        /// The unavailable resource.
        /// </summary>
        public const string UnavailableResource = "437";

        /// <summary>
        /// The nickname in use.
        /// </summary>
        public const string NicknameInUse = "433";

        /// <summary>
        /// The welcome.
        /// </summary>
        public const string Welcome = "001";

        /// <summary>
        /// The I support message
        /// </summary>
        /// <remarks>
        /// It's literally called ISUPPORT in the documentation, hence suppressing inconsistent naming warning
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        public const string ISupport = "005";

        /// <summary>
        /// The no channel topic.
        /// </summary>
        public const string NoChannelTopic = "331";

        /// <summary>
        /// The channel topic.
        /// </summary>
        public const string ChannelTopic = "332";

        /// <summary>
        /// The name reply.
        /// </summary>
        public const string NameReply = "353";

        /// <summary>
        /// The end of names.
        /// </summary>
        public const string EndOfNames = "366";

        /// <summary>
        /// The end of who.
        /// </summary>
        public const string EndOfWho = "315";

        /// <summary>
        /// The who x reply.
        /// </summary>
        public const string WhoXReply = "354";

        /// <summary>
        /// The forwarding to other channel.
        /// </summary>
        public const string ForwardingToOtherChannel = "470";

        /// <summary>
        /// The channel full.
        /// </summary>
        public const string ChannelFull = "471";

        /// <summary>
        /// The invite only channel.
        /// </summary>
        public const string InviteOnlyChannel = "473";

        /// <summary>
        /// The banned from channel.
        /// </summary>
        public const string BannedFromChannel = "474";

        /// <summary>
        /// The bad channel key.
        /// </summary>
        public const string BadChannelKey = "475";

        /// <summary>
        /// The channel throttle exceeded.
        /// </summary>
        public const string ChannelThrottleExceeded = "480";

        public const string BanListEntry = "367";
        public const string QuietListEntry = "728";
        public const string ExemptListEntry = "348";
        public const string BanListEnd = "368";
        public const string QuietListEnd = "729";
        public const string ExemptListEnd = "349";
        public const string CurrentChannelMode = "324";

        #region SASL

        /// <summary>
        /// The SASL authentication failed.
        /// </summary>
        public const string SaslAuthFailed = "904";

        /// <summary>
        /// The SASL logged in.
        /// </summary>
        public const string SaslLoggedIn = "900";

        /// <summary>
        /// The SASL authentication succeeded.
        /// </summary>
        public const string SaslSuccess = "903";

        /// <summary>
        /// The SASL authentication was aborted.
        /// </summary>
        public const string SaslAborted = "906";

        #endregion
    }
}