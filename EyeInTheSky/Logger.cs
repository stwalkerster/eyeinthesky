#region Usings

using System;
using System.IO;

#endregion

namespace EyeInTheSky
{
    /// <summary>
    /// Logger
    /// </summary>
    internal class Logger
    {
        private static Logger _instance;

        protected Logger()
        {
            this._ialLogger = new StreamWriter("ial.log");
            this._errorLogger = new StreamWriter("error.log");


            this._ialLogger.AutoFlush = true;
            this._errorLogger.AutoFlush = true;

            const string init = "Welcome to Helpmebot v6.";
            this._ialLogger.WriteLine(init);
            this._errorLogger.WriteLine(init);

            addToLog(init, LogTypes.General);

        }

        private static object singletonlock = new object();
        public static Logger instance()
        {
            lock (singletonlock)
            {
                return _instance ?? (_instance = new Logger());
            }
        }

        private readonly StreamWriter _ialLogger;
        private readonly StreamWriter _errorLogger;


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

        // DATE: GREEN

        /// <summary>
        /// Adds to log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public void addToLog(string message, LogTypes type)
        {
            lock (this)
            {
                Console.ResetColor();

                string dateString = "[ " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() +
                                    " ] ";

                switch (type)
                {
                    case LogTypes.IAL:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(dateString);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        this._ialLogger.WriteLine(dateString + message);
                        Console.WriteLine("I " + message);
                        break;
                    case LogTypes.Command:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(dateString);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("C " + message);
                        break;
                    case LogTypes.General:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(dateString);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("G " + message);
                        break;
                    case LogTypes.Error:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(dateString);
                        Console.ForegroundColor = ConsoleColor.Red;
                        this._errorLogger.WriteLine(dateString + message);
                        Console.WriteLine("E " + message);
                        break;
                    default:
                        break;
                }
                Console.ResetColor();
            }
        }

    }
}