using BullyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    public interface IProcess:IDisposable
    {
        /// <summary>
        /// basic process information
        /// </summary>
        int ProcessId { get; }  
        string ProcessName { get; } 
        bool IsCorrdinator { get; set; }
        bool IsActive { get; set; }

        /// <summary>
        /// operations to simultate the run and shutdown of a process
        /// </summary>
        void Run();
        void ShutDown();


        /// <summary>
        /// events used to implement the pub/sub for the communication between process
        /// </summary>
        //if invoked send election message response to subscribers
        event EventHandler<ElectionMessageArgs> ElectionMessage;
        //if invoked sends coordinator heartbeat response to subscribers
        event EventHandler<CoordinatorMessageArgs> CoordinatorMessage;
        
        /// <summary>
        /// send election messages to subsicribers 
        /// </summary>
        void BeginElectionMessage();

        /// <summary>
        /// recive coordinator message that indicates that coordinator is alive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCoordinatorMessageRecieved(object sender, CoordinatorMessageArgs args);

    }
}
