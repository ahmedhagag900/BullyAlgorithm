using BullyAlgorithm.Models;
using BullyAlgorithm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    public interface IProcessCommunicator
    {
        CommunicatorMessage Send(int to, CommunicatorMessage message, int timeOut = 0);
        //bool Send(CommunicatorMessage message);
    }
}
