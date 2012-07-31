namespace EyeInTheSky.Commands
{
    class Raw : GenericCommand
    {
        public Raw()
        {
            RequiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            EyeInTheSkyBot.IrcFreenode.sendRawLine(string.Join(" ", tokens));
        }

        #endregion
    }
}
