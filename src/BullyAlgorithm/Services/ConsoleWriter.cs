using BullyAlgorithm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Services
{
    public class ConsoleWriter : IMessageWritter
    {
        public void Write(string data)
        {
            Console.WriteLine(data);    
        }
    }
}
