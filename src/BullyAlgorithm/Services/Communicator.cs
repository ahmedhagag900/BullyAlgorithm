using BullyAlgorithm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Services
{
    public class Communicator : ICommunicator
    {
        private Dictionary<int, IProcess> _map;
        public Communicator()
        {
            _map = new Dictionary<int, IProcess>();
        }
        public IEnumerable<IProcess> GetProcesses
        {
            get
            {
                foreach(var process in _map.Values)
                {
                    yield return process;
                }
            }
        }


        public bool AddProcess(IProcess process)
        {
            if (!_map.ContainsKey(process.ProcessId))
                _map.Add(process.ProcessId, process);
            else
                return false;

            return true;

        }
    }
}
