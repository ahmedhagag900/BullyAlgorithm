using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace BullyAlgorithm.Services
{
    public class SyncProcess
    {
        public int ProcessId { get; }
        public bool IsActive { get; set; }
        public bool IsCorrdinator { get; set; }





        private Socket _client;
        private readonly IMessageWritter _messageWritter;
        IPEndPoint _connectEndpoint;
        private bool _receivedOkResponseFromElectionMessage;
        private bool _receivedHeartBeatResponseFromCoordinator;
        public SyncProcess(int processId, IMessageWritter messageWritter)
        {
            _messageWritter = messageWritter;
            IsActive = false;
            ProcessId = processId;
            _receivedOkResponseFromElectionMessage = false;
            _receivedHeartBeatResponseFromCoordinator = false;
            InitSocketClient();

        }




        private void InitSocketClient()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            _connectEndpoint = new IPEndPoint(ipAddress, 1000);
            _client = new Socket(_connectEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        private void RecieveCallBack(IAsyncResult Ar)
        {
            byte[] data = (byte[])Ar.AsyncState;
            int rec=_client.EndReceive(Ar);

            var messageString = Encoding.ASCII.GetString(data);
            Message message=JsonConvert.DeserializeObject<Message>(messageString);
            HandleRecievedMessages(message.From, message.MessageType);

            var buffer = new byte[33];
            _client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), buffer);

        }



        public void ShutDown()
        {
             IsActive = false;
            _client.Shutdown(SocketShutdown.Both);
            _client?.Close();
            _client?.Dispose();
        }

        public void HandleRecievedMessages(int from, MessageTypes type)
        {
            if (from == ProcessId)
                return;

            var mesg = $"[{DateTime.UtcNow} ({ProcessId})] ";
            if (type == MessageTypes.Coordinator && ProcessId<from)
            {
                _messageWritter.Write($"{mesg} Process {from} is Coordinator");
                IsCorrdinator = false;
            }
            else if (type == MessageTypes.HeartBeat)
            {
                if (from < ProcessId)
                {
                    StartBullyElection();
                    return;
                }
                _messageWritter.Write($"{mesg} Recieved HeartBeat Message from {from}");
            }
            else if(type==MessageTypes.Shutdown)
            {
                _messageWritter.Write($"{mesg} Process {from} is Shuting down...");
            }else if(type==MessageTypes.Ok)
            {
                _receivedOkResponseFromElectionMessage = true;
            }
            else if (type == MessageTypes.Election)
            {

                if (ProcessId>from)
                {
                    _messageWritter.Write($"{mesg} Recieved Election Message from [{from}]");
                    var okMessage = new Message
                    {
                        From = ProcessId,
                        To = from,
                        MessageType = MessageTypes.Ok
                    };
                    //send ok response to election message
                    SendMessage(okMessage);
                    StartBullyElection();
                }else
                {
                    IsCorrdinator = false;
                }
            }
        }
        public void Run()
        {
            IsActive = true;
            ConnectClient();
            StartBullyElection();
            while (IsActive)
            {
                //Coordinator
                while (IsCorrdinator)
                {
                    var heartBeatMessage = new Message
                    {
                        From = ProcessId,
                        MessageType = MessageTypes.HeartBeat
                    };
                    SendMessage(heartBeatMessage);
                    Thread.Sleep(1000);
                    //RecieveMessages();
                }
                _receivedHeartBeatResponseFromCoordinator = false;
                Thread.Sleep(1200);
                if (!_receivedHeartBeatResponseFromCoordinator)
                    StartBullyElection();
            }
            
        }

        private bool RecieveMessages(int timeOut=0)
        {
            try
            {
                _client.ReceiveTimeout = timeOut;
                byte[] recievedBuffer = new byte[33];
                int recievedBytes = _client.Receive(recievedBuffer);
                var receivedMessageString = Encoding.ASCII.GetString(recievedBuffer);
                var recievedMessage = JsonConvert.DeserializeObject<Message>(receivedMessageString);
                HandleRecievedMessages(recievedMessage.From, recievedMessage.MessageType);
                _client.ReceiveTimeout = 0;
                return true;
            }
            catch (SocketException ex)
            {
                _client.ReceiveTimeout = 0;
                //did not recieve messages 
                return false;
            }
        }


        private void StartBullyElection()
        {
            _messageWritter.Write($"[{DateTime.UtcNow} ({ProcessId})] Satrting Election...");
            _receivedOkResponseFromElectionMessage = false;
             //prepare election message
             var electionMessage = new Message
             {
                 From = ProcessId,
                 MessageType = MessageTypes.Election
             };

            //send election message to oher process
            SendMessage(electionMessage);

            Thread.Sleep(1000);
            //_client.ReceiveTimeout = 1000;

            //if there is no ok response from sent election messages the current process elect it self as coordinator
            if (_receivedOkResponseFromElectionMessage == false)
            {
                _messageWritter.Write($"[{DateTime.UtcNow} ({ProcessId})] Winngin Election I am Coordinator :) ");
                var coordinatorMessage = new Message
                {
                    From = ProcessId,
                    MessageType = MessageTypes.Coordinator
                };
                IsCorrdinator = true;
                SendMessage(coordinatorMessage);
            }
        }

        private void SendMessage(Message message,int sendTimeOut=0,int recieveTimeout=0)
        {
            try
            {
                //ConnectClient();
                var serializedMessage = JsonConvert.SerializeObject(message);
                //_client.SendTimeout = sendTimeOut;
                _client.Send(Encoding.ASCII.GetBytes(serializedMessage));
                //RecieveMessages(recieveTimeout);
                //_client.SendTimeout = 0;
                
            }
            catch (SocketException ex)
            {
                _client.SendTimeout = 0;
                //_client.ReceiveTimeout = 0;
            }
        }
        private void ConnectClient()
        {
            if (!_client.Connected)
            {
                Thread.Sleep(1000);
                _client.Connect(_connectEndpoint);
                byte[] buffer = new byte[33];
                _client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), buffer);
            }
        }

    }
}
