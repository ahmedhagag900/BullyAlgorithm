using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Models
{
    public enum MessageTypes
    {
        Election=1,
        Coordinator=2,
        HeartBeat=3,
        Ok=4,
        Join=5,
        Shutdown=6,
    }
}
