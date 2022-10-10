using BullyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    /// <summary>
    /// Acts as cluster the process join to 
    /// allow process to know which process is in the cluster
    /// </summary>
    public interface ICommunicator
    {
        /// <summary>
        /// return active and runing process
        /// </summary>
        IEnumerable<IProcess> GetProcesses { get; }
        /// <summary>
        /// add process to the communicator
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        bool AddProcess(IProcess process);

    }
}
