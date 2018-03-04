namespace Stwalkerster.IrcClient.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Messages;
    using Stwalkerster.IrcClient.Model;

    /// <summary>
    /// The IRC Client interface.
    /// </summary>
    public interface IIrcClient
    {
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        string Nickname { get; set; }

        /// <summary>
        /// Gets the channels.
        /// </summary>
        Dictionary<string, IrcChannel> Channels { get; }

        /// <summary>
        /// Gets a value indicating whether the nick tracking is valid.
        /// </summary>
        bool NickTrackingValid { get; }

        /// <summary>
        /// Gets the user cache.
        /// </summary>
        Dictionary<string, IrcUser> UserCache { get; }

        /// <summary>
        /// Gets a value indicating whether the client logged in to a nickserv account
        /// </summary>
        bool ServicesLoggedIn { get; }

        bool NetworkConnected { get; }

        /// <summary>
        /// The received message.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> ReceivedMessage;

        /// <summary>
        /// The join received event.
        /// </summary>
        event EventHandler<JoinEventArgs> JoinReceivedEvent;

        event EventHandler<JoinEventArgs> PartReceivedEvent;

        /// <summary>
        /// The invite received event.
        /// </summary>
        event EventHandler<InviteEventArgs> InviteReceivedEvent;

        event EventHandler<ModeEventArgs> ModeReceivedEvent;

        /// <summary>
        /// The join.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        void JoinChannel(string channel);

        /// <summary>
        /// The part channel.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void PartChannel(string channel, string message);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void SendMessage(string destination, string message);

        /// <summary>
        /// The send notice.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void SendNotice(string destination, string message);

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Send(IMessage message);

        void Mode(string target, string changes);
    }
}