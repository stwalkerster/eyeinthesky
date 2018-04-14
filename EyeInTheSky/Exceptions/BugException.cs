using System;

namespace EyeInTheSky.Exceptions
{
    public class BugException : Exception
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        public BugException(string title, string description)
        {
            this.Title = title;
            this.Description = description;
        }
    }
}