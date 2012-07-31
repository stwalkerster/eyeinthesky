using System;

namespace EyeInTheSky
{
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>The nickname.</value>
        public string nickname { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string username { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>The hostname.</value>
        public string hostname { get; set; }

        /// <summary>
        /// News from string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static User newFromString(string source)
        {
            return newFromString(source, 0);
        }

        /// <summary>
        /// New user from string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="network">The network.</param>
        /// <returns></returns>
        public static User newFromString(string source, uint network)
        {
            string user, host;
            string nick = user = host = null;
            try
            {
                if ((source.Contains("@")) && (source.Contains("!")))
                {
                    char[] splitSeparators = {'!', '@'};
                    string[] sourceSegment = source.Split(splitSeparators, 3);
                    nick = sourceSegment[0];
                    user = sourceSegment[1];
                    host = sourceSegment[2];
                }
                else if (source.Contains("@"))
                {
                    char[] splitSeparators = {'@'};
                    string[] sourceSegment = source.Split(splitSeparators, 2);
                    nick = sourceSegment[0];
                    host = sourceSegment[1];
                }
                else
                {
                    nick = source;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                GlobalFunctions.errorLog(ex);
            }

            var ret = new User
                           {
                               hostname = host,
                               nickname = nick,
                               username = user,
                           };
            return ret;
        }

        /// <summary>
        ///   Recompiles the source string
        /// </summary>
        /// <returns>nick!user@host, OR nick@host, OR nick</returns>
        public override string ToString()
        {

            string endResult = string.Empty;

            if (nickname != null)
                endResult = nickname;

            if (username != null)
            {
                endResult += "!" + username;
            }
            if (hostname != null)
            {
                endResult += "@" + hostname;
            }

            return endResult;
        }

        /// <summary>
        /// Gets or sets the access level.
        /// </summary>
        /// <value>The access level.</value>
        public UserRights accessLevel
        {
            get
            {
                return EyeInTheSkyBot.Config.accessList.ContainsKey(hostname) ? EyeInTheSkyBot.Config.accessList[hostname].AccessLevel : UserRights.Normal;
            }
        }

        public enum UserRights
        {
            Developer = 3,
            Superuser = 2,
            Advanced = 1,
            Normal = 0,
            Semiignored = -1,
            Ignored = -2
        }
    }
}