namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using Stwalkerster.IrcClient.Model;

    public interface IBotUser : INamedItem
    {
        IrcUserMask Mask { get; }
        string GlobalFlags { get; set; }
        string EmailAddress { get; set; }
        bool EmailAddressConfirmed { get; }
        DateTime? EmailConfirmationTimestamp { get; }
        DateTime? DeletionConfirmationTimestamp { get; }
        string EmailConfirmationToken { get; }
        string DeletionConfirmationToken { get; }
        string ConfirmEmailAddress(string token);
        string ValidateDeleteAccount(string token);
        void PrepareDeleteAccount();
        void GarbageCollectTokens();

        string WebPassword { get; set; }
        Guid WebGuid { get; set; }
    }
}