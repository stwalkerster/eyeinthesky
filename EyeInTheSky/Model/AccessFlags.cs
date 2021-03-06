﻿namespace EyeInTheSky.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Stwalkerster.Bot.CommandLib.Model;

    public class AccessFlags : Flag
    {
        /// <summary>
        /// Configuration access to the bot. This should either be granted per-channel or globally
        /// </summary>
        public const string Configuration = "C";

        /// <summary>
        /// Administrative access to the bot. This should be granted globally-only, and should manage things like
        /// joining channels, force-deleting accounts, etc
        /// </summary>
        public const string GlobalAdmin = "A";

        /// <summary>
        /// Flag which all registered NickServ users hold
        /// </summary>
        public const string User = "u";

        /// <summary>
        /// Local administrative access to the bot. This should be automatically granted to chanops, otherwise granted
        /// locally only. This manages administrativia which affects one channel only, like parting a channel and
        /// managing local flags. Should be granted locally only.
        /// </summary>
        public const string LocalAdmin = "a";
        
        /// <summary>
        /// Grants the ability to join new channels.
        /// </summary>
        public const string ChannelJoin = "J";
    }
}