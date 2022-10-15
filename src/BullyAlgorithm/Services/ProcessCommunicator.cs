using BullyAlgorithm.Helper;
using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace BullyAlgorithm.Services
{
    public class ProcessCommunicator
    {
        private readonly Socket _listener;
        private readonly IPAddress _ip;
        private readonly List<int> _clustrProcesses;
        private readonly IMessageWritter _messageWritter;
        private readonly int _processId;
        private const int _port = 1010;
        private bool _isCoordinator;
        private bool _heartBeatRecieved;
        private bool _isActive;
        private const int _recieveTimeOut = 1000;
        private const int _aliveMessageTimeOut = 1000;
        private const int _heartBeatCheckTimeOut = 3000;
        public ProcessCommunicator(int processId,IMessageWritter messageWritter)
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
            _listener = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(_ip, _port + _processId));
            _listener.Listen();
        }
        public void Run()
        {
            JoinToCluster();
            _isActive = true;
            if (_clustrProcesses.Count == 0)
            {
                _isCoordinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({_processId}) won the election and became the coordinator :)");
            }
            while (_isActive)
            {
                RecieveMessages(_recieveTimeOut);
                while (_isCoordinator)
                {
                    var heartBeatMessage = new CommunicatorMessage
                    {
                        From = _processId,
                        Type = MessageTypes.HeartBeat
                    };
                    foreach(var process in _clustrProcesses)
                    {
                        Send(process, heartBeatMessage,false);
                    }
                    Thread.Sleep(_aliveMessageTimeOut);
                }


                //wait for heart beat to recieve
                Thread.Sleep(_heartBeatCheckTimeOut);
                if (!_heartBeatRecieved&&_isActive)
                {
                    _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Coordinator failuer detected");
                    StartBullyElection();
                }
                _heartBeatRecieved = false;

            }

        }
        public void ShutDown()
        {
            foreach (var p in _clustrProcesses)
            {
                Send(p, new CommunicatorMessage { From = _processId, Type = MessageTypes.Shutdown }, false, 100);
            }

            _isActive = false;
            _clustrProcesses.Clear();
            _isCoordinator = false;
            _listener.Close();

        }
        private void JoinToCluster()
        {
            var sender = CreateSenderSocket(_port);
            if (sender == null)
                return;
            try
            {
                var messege = new CommunicatorMessage
                {
                    From = _processId,
                    Type = MessageTypes.Join
                };
                byte[] buffer = messege.ToByte();
                sender.Send(buffer, 0, buffer.Length, SocketFlags.None);


                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Processe ({_processId}) is Joining the cluster...");

                var handler = _listener.Accept();
                var responseBuffer = new byte[50];
                var recieved=handler.Receive(responseBuffer);
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
                    var response = Send(process, electionMessage,true);
                    if(response?.Type==MessageTypes.Ok)
                    {
                        recievedOk = true;
                        break;
                    }
                }
            }

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
                for(int i=0;i<sz ;++i)
                {
                    Send(_clustrProcesses[i], coordinatorMessage, false);
                }
            }else
            {
                bool recievedCoordinatorMessage = Recieve(MessageTypes.Coordinator, _recieveTimeOut);
                if (!recievedCoordinatorMessage)
                    StartBullyElection();
               
            }

        }
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
                    var handler = _listener.Accept();
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
        private bool Recieve(MessageTypes type, int timeOut = 0)
        {
            try
            {
                var handler = _listener.Accept();
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
        private void RecieveMessages(int timeOut)
        {
            try
            {
                var acceptResult = _listener.BeginAccept(new AsyncCallback(AcceptCallBack), timeOut);
                acceptResult.AsyncWaitHandle.WaitOne(timeOut);
            }
            catch (SocketException sx)
            {
            }
            catch (Exception ex)
            {
            }
        }
        private void AcceptCallBack(IAsyncResult Ar)
        {
            var timeOut = (int)Ar.AsyncState;
            if (!_isActive)
                return;
            var handler = _listener.EndAccept(Ar);
            try
            {
                byte[] buffer = new byte[50];
                handler.ReceiveTimeout = timeOut;
                while (true)
                {
                    int recieved = handler.Receive(buffer);
                    var message = Encoding.ASCII.GetString(buffer, 0, recieved);
                    HandleMessage(message.ToCommunicatorMessage());
                    handler.Close();
                }
            }
            catch (SocketException sx)
            {
                handler.Close();
            }
            catch (Exception ex)
            { }
            finally
            {
                var acceptResult = _listener.BeginAccept(new AsyncCallback(AcceptCallBack), timeOut);
                acceptResult.AsyncWaitHandle.WaitOne(timeOut);
            }

        }
        private void HandleMessage(CommunicatorMessage message)
        {
            if (message.Type == MessageTypes.Join)
            {
                _clustrProcesses.Add(message.From);
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Processe ({message.From}) Joined the cluster");
            }
            else if (message.Type == MessageTypes.Coordinator)
            {
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({message.From}) is the coordinator ");
                _isCoordinator = false;

            }
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
            else if (message.Type == MessageTypes.HeartBeat)
            {
                _heartBeatRecieved = true;
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] HeartBeat received from Process ({message.From})");
            }
            else if (message.Type == MessageTypes.Shutdown)
            {
                _clustrProcesses.Remove(message.From);
                _messageWritter.Write($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} - [{_processId}] ] Process ({message.From}) is Shuting down");
            }
        }

    }
}
