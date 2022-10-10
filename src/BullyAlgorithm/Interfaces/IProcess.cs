using BullyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    public interface IProcess
    {
        int ProcessId { get; }  
        string ProcessName { get; } 
        bool IsCorrdinator { get; set; }
        bool IsActive { get; set; }
        void Run(bool isRunning = false);
        void ShutDown();

        event EventHandler<ElectionMessageArgs> ElectionMessage;

        event EventHandler<CoordinatorMessageArgs> CorrdinatorMessage;
        
        void SendElectionMessage();
        void OnCoordinatorMessageRecieved(object sender, CoordinatorMessageArgs args);

    }
}
