namespace EyeInTheSky.Commands
{
    class Quit : GenericCommand
    {
        public Quit()
        {
            RequiredAccessLevel = User.UserRights.Superuser;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            EyeInTheSkyBot.IrcWikimedia.ircQuit();
            EyeInTheSkyBot.IrcFreenode.ircQuit();

            EyeInTheSkyBot.IrcWikimedia.stop();
            EyeInTheSkyBot.IrcFreenode.stop();
        }

        #endregion
    }
}
