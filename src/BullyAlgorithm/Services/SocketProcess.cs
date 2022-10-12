using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool IsActive { get; set; }
        public bool IsCorrdinator
        {
            get { return _isCorrdinator; }
            set
            {
                if (value == true)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
                _isCorrdinator = value;
            }
        }
            




        private Socket _client;
        private readonly SocketCommunicator _communicator;
        private readonly IMessageWritter _messageWritter;
        IPEndPoint _connectEndpoint;
        private bool _electionMessageRecieved;
        private bool _coordinatorMessageRecieved;
        private byte[] _buffer = new byte[1024];
        private bool _isCorrdinator;
        System.Timers.Timer _timer = new System.Timers.Timer();
        
        public SocketProcess(int processId,SocketCommunicator communicator,IMessageWritter messageWritter)
        {
            _communicator = communicator;
            _messageWritter = messageWritter;
            _electionMessageRecieved = false;
            _coordinatorMessageRecieved = false;
            _isCorrdinator= false;
            IsActive = false;
            ProcessId = processId;
            InitSocketClient();
            _communicator.AddProcess(this);

        }




        private void InitSocketClient()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            _connectEndpoint = new IPEndPoint(ipAddress, 1000);
            _client = new Socket(_connectEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _client.ReceiveTimeout = 1000;
            _client.SendTimeout = 1000;
        }

        private void RecieveCallBack(IAsyncResult Ar)
        {
            if (!IsActive)
                return;
            
            Socket socket = (Socket)Ar.AsyncState;
            var byts = socket.EndReceive(Ar);
            var data = new byte[byts];
            Array.Copy(_buffer, data, byts);
            var text = Encoding.ASCII.GetString(data);
            if (text != null)
            {
                var message = JsonConvert.DeserializeObject<Message>(text);
                if (message.To == ProcessId)
                {
                    HandleRecievedMessages(message.From, message.MessageType);
                }
            }
            
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);
        }



        public void ShutDown()
        {
            _messageWritter.Write($"Shuting down [{this.ProcessId}]");
            IsActive = false;
            _timer.Stop();
            _client.Disconnect(true);
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }

        public void HandleRecievedMessages(int from,MessageTypes type)
        {
            if (!IsActive)
                return;

            string messageFormat = $"[{DateTime.UtcNow}] Process [{ProcessId}] Recieved ";
            if(type==MessageTypes.Coordinator)
            {
                _messageWritter.Write(messageFormat + $" Coordinator is Alive from [{from}]");
                _coordinatorMessageRecieved = true;
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
        public void Run()
        {
            IsActive = true;
            StartBullyElection(true);
            if(IsCorrdinator)
            {
                _timer.Interval = 1000;
                _timer.Elapsed += (s, e) => BoadCastCoordinatorHeartBeate();
                _timer.Start();
            }else
            {
                _timer.Interval = 1100;
                _timer.Elapsed += (s, e) =>
                {
                    if (IsActive)
                    {
                        if(!_coordinatorMessageRecieved)
                        {
                            StartBullyElection();
                            _timer.Stop();
                        }
                        _coordinatorMessageRecieved = false;
                    }
                };
                _timer.Start();
            }
            RecieveInCommingMessages();
        }

        private void BoadCastCoordinatorHeartBeate()
        {
            if (IsActive && IsCorrdinator)
            {
                foreach (var p in _communicator.GetProcesses)
                {
                    if (p.ProcessId != this.ProcessId)
                    {
                        SendMessage(new Message { From = this.ProcessId, To = p.ProcessId, MessageType = MessageTypes.Coordinator }, false);
                    }
                }
            }
        }

        private void StartBullyElection(bool firstElection=false)
        {
            if (firstElection)
                _messageWritter.Write($"[{DateTime.UtcNow}] Process [{this.ProcessId}] Joined the cluster...");

            _messageWritter.Write($"[{DateTime.UtcNow}] Process [{this.ProcessId}] Starts an Election....");

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
                    var response=SendMessage(message);
                    //if there is a response break the loop
                    if (response?.MessageType == MessageTypes.Ok)
                    {
                        _electionMessageRecieved = true;
                        break;
                    }
                }
            }

            //if there is no ok response from sent election messages the current process elect it self as coordinator
            if (_electionMessageRecieved==false)
            {
                _communicator.GetProcesses.ToList().ForEach(p => p.IsCorrdinator = false);
                IsCorrdinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow}] Process [{this.ProcessId}] is Coordinator");
            }
        }

        private void RecieveInCommingMessages()
        { 
            if (!IsActive)
                return;

            ConnectClient();
            _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), _client);
        }

        private Message SendMessage(Message message,bool recieveResponse=true)
        {
            try
            {
                ConnectClient();
                var serializedMessage = JsonConvert.SerializeObject(message);
                _client.Send(Encoding.ASCII.GetBytes(serializedMessage));
                if (recieveResponse)
                {
                    var data = new byte[512];
                    var size = _client.Receive(data);
                    string msgStr = Encoding.ASCII.GetString(data);
                    var msg = JsonConvert.DeserializeObject<Message>(msgStr);

                    return msg;
                }
                return null;
            }catch(SocketException ex)
            {
                return null;
            }
        }
        private void ConnectClient()
        {
            if (!_client.Connected)
            {
                _client.Connect(_connectEndpoint);
                _client.Send(Encoding.ASCII.GetBytes(ProcessId.ToString()));
                var ack = new byte[10];
                _client.Receive(ack);
            }
        }

    }
}
