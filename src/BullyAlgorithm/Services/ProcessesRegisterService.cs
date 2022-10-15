using BullyAlgorithm.Helper;
using BullyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Services
{
    public class ProcessesRegisterService
    {
        private readonly List<int> _processes;
        private Socket _server;
        private Socket _sender;
        private const int _port = 1010;
        private IPAddress _ip;
        public ProcessesRegisterService()
        {
            _processes = new List<int>();
            InitClusterServer();
        }

        private void InitClusterServer()
        {
            var host=Dns.GetHostEntry(Dns.GetHostName());
            _ip = host.AddressList[0];
            var listenerEndpoint=new IPEndPoint(_ip, _port);
            _server = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sender = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(listenerEndpoint);
            _server.Listen();
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), SocketFlags.None);
        }

        private void AcceptCallBack(IAsyncResult Ar)
        {
            var handler = _server.EndAccept(Ar);

            var buffer = new byte[50];
            try
            {
                //will accept join message from process
                var recieved = handler.Receive(buffer);
                var recievedMessage = Encoding.ASCII.GetString(buffer, 0, recieved);

                var message = recievedMessage.ToCommunicatorMessage();
                
                //inform other process in the cluster of the new joined process
                foreach(var process in _processes)
                {
                   SendJoinMessage(message.From, process);
                }
                if(!_processes.Contains(message.From))
                    _processes.Add(message.From);
                

                //inform the joined process of other processes 
                var processesMessage = string.Join('|', _processes.Where(p => p != message.From));
                var sender = CreateSenderSocket(_port + message.From);

                var sendBuffer = Encoding.ASCII.GetBytes(processesMessage);

                sender.Send(sendBuffer);
                sender.Close();
                handler.Close();
            }
            catch(SocketException sx)
            {
                handler.Close();
            }
            finally
            {
                _server.BeginAccept(new AsyncCallback(AcceptCallBack), SocketFlags.None);
            }

        }

        private bool SendJoinMessage(int fromId,int toId)
        {

            var sender = CreateSenderSocket(_port + toId);
            try
            {
                var joinMessage = new CommunicatorMessage
                {
                    From = fromId,
                    Type = MessageTypes.Join
                };
                sender.Send(joinMessage.ToByte());
                sender.Close();
                return true;
            }catch(SocketException sx)
            {
                sender.Close();
                return false;
            }
        }

        private Socket CreateSenderSocket(int port)
        {
            var sender = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(_ip, port);
            return sender;
        }


    }
}
