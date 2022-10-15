using BullyAlgorithm.Helper;
using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace BullyAlgorithm.Services
{
    public class Process:IProcess
    {
        private TcpListener _listener;
        private readonly IPAddress _ip;
        private readonly List<int> _clustrProcesses;
        private readonly IMessageWritter _messageWritter;
        private readonly int _processId;
        private const int _port = 1010;
        private bool _isCoordinator;
        private bool _heartBeatRecieved;
        private bool _isActive;
        private const int _recieveTimeOut = 200;  
        private const int _aliveMessageTimeOut = 1000;
        private int _heartBeatCheckTimeOut = 3000;


        /// <summary>
        /// beacuse the process discovery port is the defaul and we add the process id to it
        /// to make it the specific port of this process
        /// </summary>
        /// <param name="processId">should be greater than 0</param>
        /// <param name="messageWritter"></param>

        public Process(int processId,IMessageWritter messageWritter)
        {
            //init process data 
            _isActive = false;
            _processId = processId;
            _clustrProcesses = new List<int>();
            _isCoordinator = false;
            _messageWritter = messageWritter;
            _heartBeatRecieved = false;

            //inti process listener to messages
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            _ip = host.AddressList[0];
            //InitListener();
        }

        private void InitListener()
        {
            _listener = new TcpListener(_ip, _port + _processId); 
            _listener.Start();
        }
        /// <summary>
        /// run the process
        /// </summary>
        public void Run()
        {
            InitListener();
            JoinToCluster();
            _isActive = true;
            //if there is no other processes in the cluster the the current process is the coordinator
            if (_clustrProcesses.Count == 0)
            {
                _isCoordinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({_processId}) won the election and became the coordinator :)");
            }
            while (_isActive)
            {
                //if the current process is coordinator
                while (_isCoordinator&&_isActive)
                {
                    //send heart beat message [coordinator is alive] periodically 
                    var heartBeatMessage = new CommunicatorMessage
                    {
                        From = _processId,
                        Type = MessageTypes.HeartBeat
                    };
                    foreach(var process in _clustrProcesses)
                    {
                        Send(process, heartBeatMessage,false);
                    }
                    //expect to recieve other messages like
                    // join messages 
                    // shutdown messages etc...
                    RecieveMessages(_recieveTimeOut);
                    Thread.Sleep(_aliveMessageTimeOut);
                }
                //if not coordinator
                if (_isActive)
                {
                    //expect to recieve heart beat message for the defined time
                    RecieveMessages(_heartBeatCheckTimeOut);
                    //if did not recieve heart beat message for the defined time then coordinator is down and start new election
                    if (!_heartBeatRecieved && _isActive)
                    {
                        _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Coordinator failuer detected");
                        StartBullyElection();
                    }
                    _heartBeatRecieved = false;
                }

            }

        }
        /// <summary>
        /// shut down the process
        /// </summary>
        public void ShutDown()
        {
            
            _isActive = false;
            foreach (var p in _clustrProcesses)
            {
                Send(p, new CommunicatorMessage { From = _processId, Type = MessageTypes.Shutdown }, false, 100);
            }

            _clustrProcesses.Clear();
            _isCoordinator = false;
            _listener.Stop();

        }
        /// <summary>
        /// join to the cluster and send join messages to other process in the cluster 
        /// get the other processe in the cluster and save it in the processes list
        /// </summary>
        private void JoinToCluster()
        {
            var sender = CreateSenderSocket(_port);
            if (sender == null)
                return;
            try
            {
                //send join message to the process register 
                var messege = new CommunicatorMessage
                {
                    From = _processId,
                    Type = MessageTypes.Join
                };
                byte[] buffer = messege.ToByte();
                sender.Send(buffer, 0, buffer.Length, SocketFlags.None);


                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Processe ({_processId}) is Joining the cluster...");
                var handler = _listener.AcceptSocket();
                var responseBuffer = new byte[50];
                var recieved = handler.Receive(responseBuffer);
                //get the other processes and add them to processes list
                if (recieved > 0)
                {
                    string message = Encoding.ASCII.GetString(responseBuffer, 0, recieved);
                    var processes = message.Split('|').Select(x => int.Parse(x)).ToList();
                    _clustrProcesses.AddRange(processes);
                    
                }
                sender.Close();
                handler.Close();
                

            }catch (SocketException sx)
            {
                sender.Close();
            }catch(Exception ex) { }

        }
        
        /// <summary>
        /// starts the bully election algorithm
        /// </summary>
        private void StartBullyElection()
        {
            _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({_processId}) is starting an election");
            _isCoordinator = false;
            var electionMessage = new CommunicatorMessage
            {
                From = _processId,
                Type = MessageTypes.Election
            };
            bool recievedOk = false;
            foreach(var process in _clustrProcesses)
            {
                if(process>_processId)
                {
                    //send election message to other processes with heigher id 
                    var response = Send(process, electionMessage,true);
                    //if recieved an ok response then break
                    if(response?.Type==MessageTypes.Ok)
                    {
                        recievedOk = true;
                        break;
                    }
                }
            }

            //if didn't recieve an ok response 
            //pormote my self as coordinator
            if(!recievedOk)
            {
                var coordinatorMessage = new CommunicatorMessage
                {
                    From = _processId,
                    Type = MessageTypes.Coordinator
                };
                _isCoordinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({_processId}) Won the election and became the coordinator :)");
                int sz = _clustrProcesses.Count;
                //sending coordinator message to other processes
                for(int i=0;i<sz ;++i)
                {
                    Send(_clustrProcesses[i], coordinatorMessage, false);
                }
            }else
            {
                //if recieved ok response wait for coordinator message to resieve
                bool recievedCoordinatorMessage = Recieve(MessageTypes.Coordinator, _recieveTimeOut);
                //if didn't recieve the coordinator message start the election again
                if (!recievedCoordinatorMessage)
                    StartBullyElection();
               
            }

        }

        /// <summary>
        /// creates a socket to send to 
        /// the socket created with the default ip address and the port is the  default port + processid
        /// this desinged to distinguish between diffrent processes ports and which process to send to
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private Socket CreateSenderSocket(int port)
        {
            try
            {
                var sender = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(_ip, port);
                return sender;
            }catch(Exception ex) 
            { 
                return null; 
            }
        }

        /// <summary>
        /// send message to specific process
        /// </summary>
        /// <param name="toProcessId"></param>
        /// <param name="message"></param>
        /// <param name="recieve"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        private CommunicatorMessage Send(int toProcessId, CommunicatorMessage message, bool recieve = true, int timeOut = 0)
        {
            //send the message to the wanted process
            var sender = CreateSenderSocket(_port + toProcessId);
            if (sender == null)
                return null;
            try
            {


                var messageString = message.ToString();
                byte[] buffer = Encoding.ASCII.GetBytes(messageString);
                sender.SendTimeout = timeOut;
                sender.Send(buffer, 0, buffer.Length, SocketFlags.None);

                //expect to recieve message
                if (recieve)
                {
                    //recieve respones of the sent message
                    var handler = _listener.AcceptSocket();
                    handler.ReceiveTimeout = timeOut;
                    handler.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    string recievedMessage = Encoding.ASCII.GetString(buffer);
                    handler.Close();
                    sender.Close();
                    return recievedMessage.ToCommunicatorMessage();
                }
                sender.Close();
                return null;

            }
            catch (SocketException sx)
            {
                sender.Close();
                return null;
            }
            catch (Exception ex)
            {
                sender.Close();
                return null;
            }
        }
        
        /// <summary>
        /// recieve specific message type and returns true if recieved and false if not
        /// </summary>
        /// <param name="type"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        private bool Recieve(MessageTypes type, int timeOut = 0)
        {
            try
            {
                var handler = _listener.AcceptSocket();
                var buffer = new byte[33];
                handler.ReceiveTimeout = timeOut;
                try
                {
                    while (true)
                    {
                        handler.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        var recievedMessage = Encoding.ASCII.GetString(buffer);
                        var msg = recievedMessage.ToCommunicatorMessage();
                        if (msg.Type == type)
                        {
                            handler.Close();
                            return true;
                        }
                    }

                }
                catch (SocketException sx)
                {
                    handler.Close();
                    return false;
                }
            }catch(Exception ex)
            { return false; }
        }
       
        /// <summary>
        /// recieve different messages with the specified tim out
        /// </summary>
        /// <param name="timeOut"></param>
        private void RecieveMessages(int timeOut)
        {
            if (!_isActive)
                return;
            //if there is no pending connection wait for the timeout
            if(!_listener.Pending())
            {
                Thread.Sleep(timeOut);
            }

            while(_isActive&&_listener.Pending())
            {
                var handler = _listener.AcceptSocket();
                try
                {
                    byte[] buffer = new byte[50];
                    handler.ReceiveTimeout = timeOut;
                    int recieved = handler.Receive(buffer);
                    var message = Encoding.ASCII.GetString(buffer, 0, recieved);
                    HandleMessage(message.ToCommunicatorMessage());
                    handler.Close();
                    
                }
                catch (SocketException sx)
                {
                    handler.Close();
                }
                catch (Exception ex)
                { }
            }
            return;
        }

        /// <summary>
        /// handle different types of recieved messages
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessage(CommunicatorMessage message)
        {
            //if join message then add the join process to the list of processes discoverd
            if (message.Type == MessageTypes.Join)
            {
                _clustrProcesses.Add(message.From);
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Processe ({message.From}) Joined the cluster");
            }
            //if coordinator message 
            else if (message.Type == MessageTypes.Coordinator)
            {
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({message.From}) is the coordinator ");
                _isCoordinator = false;

            }
            //if election message send ok response to the process and start the election
            else if (message.Type == MessageTypes.Election)
            {

                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Election Message received from Process ({message.From})");
                var okMessage = new CommunicatorMessage
                {
                    From = _processId,
                    Type = MessageTypes.Ok
                };
                var sender = CreateSenderSocket(_port + message.From);
                if (sender != null)
                {
                    sender.Send(okMessage.ToByte());
                    sender.Close();
                }
            }
            //if heartbeat [coordinator is alive] then set the coordinator is alive flag with true  
            else if (message.Type == MessageTypes.HeartBeat)
            {
                _heartBeatRecieved = true;
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] HeartBeat received from Process ({message.From})");
            }
            //if shutdown remove the process from discoverd processes
            else if (message.Type == MessageTypes.Shutdown)
            {
                _clustrProcesses.Remove(message.From);
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({message.From}) is Shuting down");
            }
        }

    }
}
