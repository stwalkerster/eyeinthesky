namespace EyeInTheSky.Model
{
    using System;

    public class EmailConfiguration
    {
        public EmailConfiguration(string hostname,
            string sender,
            string to)
        {
            if (hostname == null)
            {
                throw new ArgumentNullException("hostname");
            }

            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            this.Hostname = hostname;
            this.Sender = sender;
            this.To = to;
        }

        public string Hostname { get; private set; }
        public int Port { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string Sender { get; private set; }
        public string To { get; private set; }
        
        public string Thumbprint { get; set; }
    }
}