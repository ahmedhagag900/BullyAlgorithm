using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Models
{
    public class CommunicatorMessage
    {
        public int From { get; set; }
        public MessageTypes Type { get; set; }

        public override string ToString()
        {
            var str = JsonConvert.SerializeObject(this);
            return str; 
        }

        public byte[] ToByte()
        {
            return Encoding.ASCII.GetBytes(ToString());
        }

    }
}
