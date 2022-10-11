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
        public SocketCommunicator()
        {
            _map = new Dictionary<int, SocketProcess>();
            Task.Factory.StartNew(() => InitSocketServer());
        }


        private void InitSocketServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            var _bindingAddress = new IPEndPoint(ipAddress, 1000);
            var listener = new Socket(_bindingAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(_bindingAddress);
            listener.Listen(1);
            while (true)
            {
                var handler = listener.Accept();
                var buffer = new byte[1_024];
                while (true)
                {
                    var received = handler.Receive(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    var message = JsonConvert.DeserializeObject<Message>(response);
                    if (message.MessageType == MessageTypes.Ok)
                    {
                        bool dd = true;
                    }
                    if (message?.To != null && message?.From != null)
                    {
                        _map[message.To].RecieveMessage(message.From, message.MessageType);
                    }
                }
            }
            
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
