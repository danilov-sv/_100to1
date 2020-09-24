using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Configuration;

namespace PolicySocketServices
{
    class PolicySocketServer
    {
        TcpListener _Listener = null;
        TcpClient _Client = null;
        static ManualResetEvent _TcpClientConnected = new ManualResetEvent(false);
        const string _PolicyRequestString = "<policy-file-request/>";
        int _ReceivedLength = 0;
        byte[] _Policy = null;
        byte[] _ReceiveBuffer = null;

        private void InitializeData()
        {
            string policyFile = ConfigurationManager.AppSettings["PolicyFilePath"];
            using (FileStream fs = new FileStream(policyFile, FileMode.Open))
            {
                _Policy = new byte[fs.Length];
                fs.Read(_Policy, 0, _Policy.Length);
            }
            _ReceiveBuffer = new byte[_PolicyRequestString.Length];
        }

        public void StartSocketServer()
        {
            InitializeData();

            try
            {
                //Using TcpListener which is a wrapper around a Socket
                //Allowed port is 943 for Silverlight sockets policy data
                _Listener = new TcpListener(IPAddress.Any, 943);
                _Listener.Start();
                Console.WriteLine("Policy server listening...");
                while (true)
                {
                    _TcpClientConnected.Reset();
                    Console.WriteLine("Waiting for client connection...");
                    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), null);
                    _TcpClientConnected.WaitOne(); //Block until client connects
                }
            }
            catch (Exception exp)
            {
                LogError(exp);
            }
        }

        private void OnBeginAccept(IAsyncResult ar)
        {
            _Client = _Listener.EndAcceptTcpClient(ar);
            _Client.Client.BeginReceive(_ReceiveBuffer, 0, _PolicyRequestString.Length, SocketFlags.None,
                new AsyncCallback(OnReceiveComplete), null);
        }

        private void OnReceiveComplete(IAsyncResult ar)
        {
            try
            {
                _ReceivedLength += _Client.Client.EndReceive(ar);
                //See if there's more data that we need to grab
                if (_ReceivedLength < _PolicyRequestString.Length)
                {
                    //Need to grab more data so receive remaining data
                    _Client.Client.BeginReceive(_ReceiveBuffer, _ReceivedLength, 
                        _PolicyRequestString.Length - _ReceivedLength,
                        SocketFlags.None, new AsyncCallback(OnReceiveComplete), null);
                    return;
                }

                //Check that <policy-file-request/> was sent from client
                string request = System.Text.Encoding.UTF8.GetString(_ReceiveBuffer, 0, _ReceivedLength);
                if (StringComparer.InvariantCultureIgnoreCase.Compare(request, _PolicyRequestString) != 0)
                {
                    //Data received isn't valid so close
                    _Client.Client.Close();
                    return;
                }
                //Valid request received....send policy file
                _Client.Client.BeginSend(_Policy, 0, _Policy.Length, SocketFlags.None, 
                    new AsyncCallback(OnSendComplete), null);
            }
            catch (Exception exp)
            {
                _Client.Client.Close();
                LogError(exp);
            }
            _ReceivedLength = 0;
            _TcpClientConnected.Set(); //Allow waiting thread to proceed
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                _Client.Client.EndSendFile(ar);
            }
            catch (Exception exp)
            {
                LogError(exp);
            }
            finally            
            {
                //Close client socket
                _Client.Client.Close();
            } 
        }

        private void LogError(Exception exp)
        {
            string appFullPath = Assembly.GetCallingAssembly().Location;
            string logPath = appFullPath.Substring(0, appFullPath.LastIndexOf("\\")) + ".log";
            StreamWriter writer = new StreamWriter(logPath, true);
            try
            {
                writer.WriteLine(logPath,
                    String.Format("Error in PolicySocketServer: "
                    + "{0} \r\n StackTrace: {1}", exp.Message, exp.StackTrace));
            }
            catch { }
            finally
            {
                writer.Close();
            }
        }
    }
}
