#region Usings

using System;

#endregion

namespace EyeInTheSky
{
    using Castle.Core.Logging;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Logger
    /// </summary>
    [Obsolete("Use log4net")]
    internal class Logger
    {
        private static Logger _instance;

        protected Logger()
        {
            this.log4net = ServiceLocator.Current.GetInstance<ILogger>();
            this.ialLogger = this.log4net.CreateChildLogger("LegacyIAL");
            this.commandLogger = this.log4net.CreateChildLogger("LegacyCommand");
        }

        private static object singletonlock = new object();

        public static Logger instance()
        {
            lock (singletonlock)
            {
                return _instance ?? (_instance = new Logger());
            }
        }

        private ILogger log4net;
        private ILogger ialLogger;
        private ILogger commandLogger;

        /// <summary>
        /// Log types
        /// </summary>
        public enum LogTypes
        {
            /// <summary>
            /// IRC stuff YELLOW
            /// </summary>
            IAL, // 

            /// <summary>
            /// command log events, BLUE
            /// </summary>
            Command, // 

            /// <summary>
            /// general log events, WHITE
            /// </summary>
            General, // 

            /// <summary>
            /// error events, RED
            /// </summary>
            Error, // 
        }

        /// <summary>
        /// Adds to log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public void addToLog(string message, LogTypes type)
        {
            lock (this)
            {
                switch (type)
                {
                    case LogTypes.IAL:
                        this.ialLogger.Info(message);
                        break;
                    case LogTypes.Command:
                        this.commandLogger.Info(message);
                        break;
                    case LogTypes.General:
                        this.log4net.Info(message);
                        break;
                    case LogTypes.Error:
                        this.log4net.Error(message);
                        break;
                }
            }
        }
    }
}