﻿namespace EyeInTheSky.Model
{
    using System;

    public class EmailConfiguration
    {
        public EmailConfiguration(string hostname,
            string sender)
        {
            if (hostname == null)
            {
                throw new ArgumentNullException("hostname");
            }

            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            this.Hostname = hostname;
            this.Sender = sender;
        }

        public string Sender { get; private set; }
        
        public string Hostname { get; private set; }
        
        // These properties are optional, and set by Castle
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string SubjectPrefix { get; set; }
        
        public int Port { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string Thumbprint { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}