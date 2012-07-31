﻿// /****************************************************************************
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
//  *                                                                          *
//  *   This file is also licenced under the MIT licence, you may use          *
//  *   whichever licence you want.                                            *
//  *                                                                          *
//  ****************************************************************************/
#region Usings

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace EyeInTheSky
{
    /// <summary>
    /// Nagios monitoring service
    /// </summary>
    internal class Nagios
    {
        private readonly TcpListener _service;

        private bool _alive;

        private readonly Thread _monitorthread;

        private const string Message = "EyeInTheSky (Nagios Monitor service)";

        /// <summary>
        /// Initializes a new instance of the Nagios class.
        /// </summary>
        /// <param name="port">The port.</param>
        public Nagios(int port = 62168)
        {
            _monitorthread = new Thread(threadMethod);

            _service = new TcpListener(IPAddress.Any, port);
            _monitorthread.Start();
        }

        private void threadMethod()
        {
            try
            {
                _alive = true;
                _service.Start();

                while (_alive)
                {
                    if (!_service.Pending())
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    var client = _service.AcceptTcpClient();

                    var sw = new StreamWriter(client.GetStream());

                    sw.WriteLine(Message);
                    sw.Flush();
                    client.Close();
                }
            }
            catch (ThreadAbortException)
            {
                threadFatalError(this, new EventArgs());
            }
            catch (ObjectDisposedException)
            {
                threadFatalError(this, new EventArgs());
            }
        }

        /// <summary>
        /// Stop all threads in this instance to allow for a clean shutdown.
        /// </summary>
        public void stop()
        {
            _service.Stop();
            _alive = false;
        }


        public event EventHandler threadFatalError;

    }
}