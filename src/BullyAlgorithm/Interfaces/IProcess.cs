using BullyAlgorithm.Models;
using BullyAlgorithm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    /// <summary>
    /// the process to initate
    /// </summary>
    public interface IProcess
    {
        public void Run();
        public void ShutDown();
    }
}
