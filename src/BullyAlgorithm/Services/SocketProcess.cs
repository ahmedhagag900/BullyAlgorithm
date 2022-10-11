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
    public class SocketProcess
    {
        public int ProcessId { get; }
        public string ProcessName { get; }
        public bool IsCorrdinator { get; set; }
        public bool IsActive { get; set; }



        private readonly Socket _client;
        private readonly SocketCommunicator _communicator;
        private readonly IMessageWritter _messageWritter;
        private bool _electionMessageRecieved;
        private IPEndPoint _connectEndpoint;
        public SocketProcess(int processId,SocketCommunicator communicator,IMessageWritter messageWritter)
        {
            _communicator = communicator;
            _messageWritter = messageWritter;
            _electionMessageRecieved = false;
            IsActive = false;
            ProcessId = processId;


            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            _connectEndpoint = new IPEndPoint(ipAddress, 1000);
            _client = new Socket(_connectEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            _communicator.AddProcess(this);

        }

        public void Run()
        {
            IsActive = true;
            StartBullyElection();
        }

        public void ShutDown()
        {
            IsActive = false;
        }

        public void RecieveMessage(int from,MessageTypes type)
        {
            if (!IsActive)
                return;

            string messageFormat = $"[{DateTime.UtcNow}] Process [{ProcessId}] Recieved ";
            if(type==MessageTypes.Coordinator)
            {
                _messageWritter.Write(messageFormat + $" Coordinator is Alive from [{from}]");
            }
            if (type == MessageTypes.Ok)
            {
                _messageWritter.Write(messageFormat + $" Ok message from [{from}]");
                _electionMessageRecieved = true;
            }
            if (type == MessageTypes.Election)
            {
                _messageWritter.Write(messageFormat + $" Election message from [{from}]");
                
                var okMessage = new Message
                {
                    From = ProcessId,
                    To = from,
                    MessageType = MessageTypes.Ok
                };
                //send ok response to election message
                SendMessage(okMessage);
                StartBullyElection();
            }
        }

        private void StartBullyElection()
        {
            _messageWritter.Write($"[{DateTime.UtcNow}] Process {this.ProcessId} Starts an Election....");

            _electionMessageRecieved = false;
            foreach (var p in _communicator.GetProcesses)
            {
                if (p.ProcessId > this.ProcessId)
                {
                    //prepare election message
                    var message = new Message
                    {
                        From = this.ProcessId,
                        To = p.ProcessId,
                        MessageType = MessageTypes.Election
                    };

                    //send election message to process [p]
                    SendMessage(message);
                }
            }

            //if there is no ok response from sent election messages the current process elect it self as coordinator
            if (_electionMessageRecieved==false)
            {
                _communicator.GetProcesses.ToList().ForEach(p => p.IsCorrdinator = false);
                IsCorrdinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow}] Process {this.ProcessId} is Coordinator");
            }
        }

        private void SendMessage(Message message)
        {
            _client.Connect(_connectEndpoint);
            var serializedMessage = JsonConvert.SerializeObject(message);
            _client.Send(Encoding.UTF8.GetBytes(serializedMessage));
            _client.Close();
        }

    }
}
