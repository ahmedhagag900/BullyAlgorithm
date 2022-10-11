using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Models
{
    internal class Message
    {
        public int From { get; set; }
        public int To { get; set; }
        public MessageTypes MessageType { get; set; }
    }
}
