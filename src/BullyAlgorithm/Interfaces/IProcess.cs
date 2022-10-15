using BullyAlgorithm.Models;
using BullyAlgorithm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    public interface IProcess
    {
        public void Run();
        public void ShutDown();
    }
}
