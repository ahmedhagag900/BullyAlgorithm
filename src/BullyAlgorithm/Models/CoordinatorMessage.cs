using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Models
{
    public class CoordinatorMessageArgs :EventArgs
    {
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
    }
}
