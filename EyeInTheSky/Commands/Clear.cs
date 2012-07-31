namespace EyeInTheSky.Commands
{
    class Clear : GenericCommand
    {
        public Clear()
        {
            RequiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            EyeInTheSkyBot.Config.RetrieveStalkLog();

            EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                       "Stalk log has been cleared.");

        }

        #endregion
    }
}
