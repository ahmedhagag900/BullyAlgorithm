using BullyAlgorithm.Helper;
using BullyAlgorithm.Interfaces;
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
    public class ProcessesRegisterService:IProcessRegisterService
    {
        private readonly List<int> _processes;
        private Socket _server;
        private const int _port = 1010;
        private IPAddress _ip;
        public ProcessesRegisterService()
        {
            _processes = new List<int>();
        }

        public void InitClusterServer()
        {
            var host=Dns.GetHostEntry(Dns.GetHostName());
            _ip = host.AddressList[0];
            var listenerEndpoint=new IPEndPoint(_ip, _port);
            _server = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(listenerEndpoint);
            _server.Listen();
            Accept();
        }
        private void Accept()
        {
            while (true)
            {
                try
                {
                    var handler = _server.Accept();
                    var buffer = new byte[50];
                    //will accept join message from process
                    var recieved = handler.Receive(buffer);
                    var recievedMessage = Encoding.ASCII.GetString(buffer, 0, recieved);

                    var message = recievedMessage.ToCommunicatorMessage();


                    //inform the joined process of other processes 
                    var processesMessage = string.Join('|', _processes.Where(p => p != message.From));

                    var sendBuffer = Encoding.ASCII.GetBytes(processesMessage);
                    handler.Send(sendBuffer);

                    //inform other process in the cluster of the new joined process
                    foreach (var process in _processes)
                    {
                        if(process!=message.From)
                            SendJoinMessage(message.From, process);
                    }

                    if (!_processes.Contains(message.From))
                        _processes.Add(message.From);

                    handler.Close();

                }
                catch (SocketException sx)
                { }
                catch (Exception ex) { }
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
