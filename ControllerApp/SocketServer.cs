using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanWahlin
{
    class SocketServer
    {
        static Logger Out { get { return LogManager.GetLogger("Output"); } }

        TcpListener _Listener = null;
        static ManualResetEvent _TcpClientConnected = new ManualResetEvent(false);
        List<StreamWriter> _ClientStreams = new List<StreamWriter>();
        int port;

        public SocketServer(int port)
        {
            this.port = port;
        }

        public void StartSocketServer()
        {
            try
            {
                //Allowed port range 4502-4532
                _Listener = new TcpListener(IPAddress.Any, port);
                _Listener.Start();
                Out.Debug("Server listening...");
                while (true)
                {
                    _TcpClientConnected.Reset();
                    Out.Debug("Waiting for client connection...");
                    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), null);
                    _TcpClientConnected.WaitOne(); //Block until client connects
                }
            }
            catch (Exception exp)
            {
                Out.Error(exp);
            }
        }

        private void OnBeginAccept(IAsyncResult ar)
        {
            _TcpClientConnected.Set(); //Allow waiting thread to proceed
            TcpListener listener = _Listener;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            if (client.Connected)
            {
                Out.Debug("Client connected...");
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.AutoFlush = true;
                _ClientStreams.Add(writer);
            }
        }

        public void StopSocketServer()
        {
            foreach (StreamWriter writer in _ClientStreams)
            {
                writer.Dispose();
            }
            _ClientStreams.Clear();
            _Listener.Stop();
            _Listener = null;
        }

        public void SendData(string data)
        {
            if (_ClientStreams != null)
            {
                foreach (StreamWriter writer in _ClientStreams)
                {
                    if (writer != null)
                    {
                        writer.Write(data);
                    }
                }
            }
        }
    }
}
