using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    /// <summary>
    /// used to write messages between process
    /// </summary>
    public interface IMessageWritter
    {
        void Write(string data);
    }
}
