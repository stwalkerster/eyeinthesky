namespace EyeInTheSky.Services.Email
{
    using System;
    using System.Text;
    using EyeInTheSky.Model.Interfaces;

    public class StalkInfoFormattingEventArgs : EventArgs
    {
        private StringBuilder stringBuilder = new StringBuilder();
        public IStalk Stalk { get; private set; }

        public StalkInfoFormattingEventArgs(IStalk stalk)
        {
            this.Stalk = stalk;
        }

        public void Append(string data)
        {
            this.stringBuilder.AppendLine(data);
        }

        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }
    }
}