// /****************************************************************************
//  *   This file is part of Helpmebot.                                        *
//  *                                                                          *
//  *   Helpmebot is free software: you can redistribute it and/or modify      *
//  *   it under the terms of the GNU General Public License as published by   *
//  *   the Free Software Foundation, either version 3 of the License, or      *
//  *   (at your option) any later version.                                    *
//  *                                                                          *
//  *   Helpmebot is distributed in the hope that it will be useful,           *
//  *   but WITHOUT ANY WARRANTY; without even the implied warranty of         *
//  *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
//  *   GNU General Public License for more details.                           *
//  *                                                                          *
//  *   You should have received a copy of the GNU General Public License      *
//  *   along with Helpmebot.  If not, see <http://www.gnu.org/licenses/>.     *
//  ****************************************************************************/
#region Usings

using System;
using System.Reflection;

#endregion

namespace EyeInTheSky
{
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        private UserRights _accessLevel;
        private bool _retrievedAccessLevel;

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

            User ret = new User
                           {
                               hostname = host,
                               nickname = nick,
                               username = user,
                           };
            return ret;
        }

        public static User newFromStringWithAccessLevel(string source, UserRights accessLevel)
        {
            return newFromStringWithAccessLevel(source, 0, accessLevel);
        }

        public static User newFromStringWithAccessLevel(string source, uint network, UserRights accessLevel)
        {
            User u = newFromString(source, network);
            u._accessLevel = accessLevel;
            return u;
        }

        /// <summary>
        ///   Recompiles the source string
        /// </summary>
        /// <returns>nick!user@host, OR nick@host, OR nick</returns>
        public override string ToString()
        {

            string endResult = string.Empty;

            if (this.nickname != null)
                endResult = this.nickname;

            if (this.username != null)
            {
                endResult += "!" + this.username;
            }
            if (this.hostname != null)
            {
                endResult += "@" + this.hostname;
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
                if (this.hostname == "pdpc/supporter/student/stwalkerster")
                    return UserRights.Developer;

                return UserRights.Normal;
            }
            set { throw new NotImplementedException(); }
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