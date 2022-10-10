using BullyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    public interface ICommunicator
    {
        IEnumerable<IProcess> GetProcesses { get; }
        bool AddProcess(IProcess process);

    }
}
