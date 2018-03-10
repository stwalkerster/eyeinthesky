namespace EyeInTheSky.Model
{
    using System;

    public class EmailConfiguration
    {
        public EmailConfiguration(string hostname,
            int port,
            string sender,
            string to,
            string subject)
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

            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            this.Hostname = hostname;
            this.Port = port;
            this.Sender = sender;
            this.To = to;
            this.Subject = subject;
        }

        public EmailConfiguration(string hostname,
            int port,
            string username,
            string password,
            string sender,
            string to,
            string subject) : this(hostname, port, sender, to, subject)
        {
            this.Username = username;
            this.Password = password;
        }

        public EmailConfiguration(string hostname,
            int port,
            string username,
            string password,
            string sender,
            string to,
            string subject,
            string thumbprint) : this(hostname, port, username, password, sender, to, subject)
        {
            this.Thumbprint = thumbprint;
        }

        public EmailConfiguration(string hostname,
            int port,
            string sender,
            string to,
            string subject,
            string thumbprint) : this(hostname, port, sender, to, subject)
        {
            this.Thumbprint = thumbprint;
        }

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        
        public string Username { get; private set; }
        public string Password { get; private set; }
        
        public string Sender { get; private set; }
        public string To { get; private set; }
        
        public string Subject { get; private set; }
        public string Thumbprint { get; private set; }
    }
}