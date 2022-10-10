using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Models;

namespace BullyAlgorithm.Services
{
    public class Process : IProcess
    {
        //used to group process together acts as a cluster of process 
        private readonly ICommunicator _communicator;
        private readonly IMessageWritter _messageWritter;

        //events used to allow processes to subscribe to the current process events
        public event EventHandler<ElectionMessageArgs>? ElectionMessage;
        public event EventHandler<CoordinatorMessageArgs>? CoordinatorMessage;

        //set when message recieved election/corrdinator
        private bool _electionMessageRecieved;
        private bool _coordinatorHeartBeatRecieved;
        //cancel tokens used to terminate the listen and boadcast tasks used by corredinator and regular processes respectivly
        private CancellationTokenSource _coordinatorBoadcastCancelTokenSource;
        private CancellationTokenSource _regularProcessListenerCancelTokenSource;

        private bool _isCorrdinator;
        public Process(int processId,ICommunicator communicator,IMessageWritter messageWritter,string processName=null)
        {
            ProcessId = processId; ;
            ProcessName = processName ?? Guid.NewGuid().ToString();
            IsCorrdinator = false;
            IsActive = false;
            _communicator = communicator;
            _messageWritter = messageWritter;
            _communicator.AddProcess(this);
            _electionMessageRecieved = false;
            _coordinatorHeartBeatRecieved = false;
            _coordinatorBoadcastCancelTokenSource = new CancellationTokenSource();
            _regularProcessListenerCancelTokenSource = new CancellationTokenSource();

        }


        public int ProcessId { get; private set; }
        public string ProcessName { get; private set; }
        public bool IsCorrdinator 
        { 
            get { return _isCorrdinator; } set 
            {
                if (value == true)
                {
                    _coordinatorBoadcastCancelTokenSource = new CancellationTokenSource();
                    _regularProcessListenerCancelTokenSource?.Cancel();
                }
                else
                {
                    _regularProcessListenerCancelTokenSource=new CancellationTokenSource();
                    _coordinatorBoadcastCancelTokenSource?.Cancel();
                }
                //this process was corrdinator and another corrdinator has been selected
                if(_isCorrdinator==true&&value==false)
                {
                    RegularProcessListen();
                }
                _isCorrdinator = value;
            } 
        }
        public bool IsActive {get; set; }


        public void Run(bool isRunning=false)
        {
            IsActive = true;
            StartBullyElection();
            if (IsCorrdinator)
            {
                BoadCastCoordinatorMessage();
            }
            else
            {
                RegularProcessListen();
            }

        }
        public void ShutDown()
        {
            this.IsActive = false;
            _coordinatorBoadcastCancelTokenSource?.Cancel();
            _regularProcessListenerCancelTokenSource?.Cancel();
            ElectionMessage = null;
            CoordinatorMessage = null;
        }

        private void StartBullyElection()
        {
            _messageWritter.Write($"[{DateTime.UtcNow}] Process {this.ProcessId} Starts an Election....");

            _electionMessageRecieved = false;
            foreach(var p in _communicator.GetProcesses)
            {
                if(p.ProcessId>this.ProcessId)
                {
                   p.ElectionMessage += OnElectionMessageRecieved;
                   p.SendElectionMessage();
                }
            }
           
            //if there is no response the current process is coordinator
            if (_electionMessageRecieved == false)
            {
                _communicator.GetProcesses.ToList().ForEach(p => p.IsCorrdinator = false);
                IsCorrdinator = true;
                _messageWritter.Write($"[{DateTime.UtcNow}] Process {this.ProcessId} is Coordinator");
            }
            

        }



        #region send and recieve process messages pub / sub using events and event handler
        private void SendCorrdinatorMessage()
        {
            if (IsActive && IsCorrdinator)
            {
                CoordinatorMessage?.Invoke(this, new CoordinatorMessageArgs { ProcessId = this.ProcessId });
            }
        }
        public void SendElectionMessage()
        {
            if (IsActive)
                ElectionMessage?.Invoke(this, new ElectionMessageArgs { ProcessId = this.ProcessId });
        }
        public void OnCoordinatorMessageRecieved(object sender,CoordinatorMessageArgs args)
        {
            if (IsActive)
            {
                _coordinatorHeartBeatRecieved = true;
                _messageWritter.Write($"[{DateTime.UtcNow} Process: {this.ProcessId}] (Corrdinator ({args.ProcessId}) is Alive)");
            }
        }
        private void OnElectionMessageRecieved(object sender, ElectionMessageArgs args)
        {
            if (IsActive)
            {
                _messageWritter.Write($"[{DateTime.UtcNow}] Process ({this.ProcessId}) recived election message from ({args.ProcessId})");
                _electionMessageRecieved = true;
            }
        }

        #endregion

        private void BoadCastCoordinatorMessage()
        {
            var boadCastTask = Task.Run(async () =>
            {
                while (!_coordinatorBoadcastCancelTokenSource.IsCancellationRequested)
                {
                    SendCorrdinatorMessage();
                    await Task.Delay(1000);
                }
            }, _coordinatorBoadcastCancelTokenSource.Token);
        }
        private void RegularProcessListen()
        {
            foreach (var p in _communicator.GetProcesses)
            {
                if (p.IsCorrdinator)
                {
                    p.CoordinatorMessage += OnCoordinatorMessageRecieved;
                }
            }

            var listener = Task.Run(async () =>
            {

                while (!_regularProcessListenerCancelTokenSource.IsCancellationRequested)
                {
                    _coordinatorHeartBeatRecieved = false;
                    await Task.Delay(1100);
                    if (!_coordinatorHeartBeatRecieved && IsActive)
                    {
                        Run();
                        break;
                    }
                }
            }, _regularProcessListenerCancelTokenSource.Token);
        }

        public void Dispose()
        {
            _coordinatorBoadcastCancelTokenSource?.Dispose();
            _regularProcessListenerCancelTokenSource?.Dispose();
        }
    }
}
