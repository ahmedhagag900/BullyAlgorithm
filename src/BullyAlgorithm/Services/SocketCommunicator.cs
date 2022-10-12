using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Services
{
    public class SocketCommunicator 
    {
        private Dictionary<int, SocketProcess> _map;
        Dictionary<int, Socket> _clients;
        private Socket _server;
        private byte[] _buffer = new byte[1024];
        public SocketCommunicator()
        {
            _map = new Dictionary<int, SocketProcess>();
            _clients = new Dictionary<int, Socket>();
            InitSocketServer();
        }


        private void InitSocketServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            var _bindingAddress = new IPEndPoint(ipAddress, 1000);
            _server = new Socket(_bindingAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(_bindingAddress);
            _server.Listen(1);
            _server.ReceiveTimeout = 1000;
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            
        }

        private void AcceptCallBack(IAsyncResult Ar)
        {
            var socket = _server.EndAccept(Ar);
            var buffer = new byte[300];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), new { socket, buffer });
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);

        }

        private void ReceiveCallBack(IAsyncResult Ar)
        {
            dynamic state = Ar.AsyncState;
            Socket socket = (Socket)state.socket;
            var recieved= socket.EndReceive(Ar);
            byte[] data = new byte[recieved];
            Array.Copy((byte[])state.buffer,data,recieved);

            string msgStr = Encoding.ASCII.GetString(data);
            bool idMessage = int.TryParse(msgStr, out int spId);
            if (idMessage)
            {
                if (!_clients.ContainsKey(spId))
                    _clients.Add(spId, socket);

                _clients[spId] = socket;
                _clients[spId].Send(Encoding.ASCII.GetBytes("<ACK>"));
            }else if(!string.IsNullOrEmpty(msgStr))
            {
                var message=JsonConvert.DeserializeObject<Message>(msgStr);
                if (message?.From != null && message?.To != null)
                {
                    if (_clients.ContainsKey(message.From) && _clients.ContainsKey(message.To))
                    {
                        _clients[message.To].Send(data);
                    }
                }   
            }
            var buffer = new byte[300];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), new { socket, buffer });

        }

        public IEnumerable<SocketProcess> GetProcesses
        {
            get
            {
                foreach(var process in _map.Values)
                {
                    yield return process;
                }
            }
        }


        public bool AddProcess(SocketProcess process)
        {
            if (!_map.ContainsKey(process.ProcessId))
                _map.Add(process.ProcessId, process);
            else
                return false;

            return true;

        }
    }
}
