using BullyAlgorithm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Helper
{
    internal static class StringHelper
    {
        public static CommunicatorMessage ToCommunicatorMessage(this string str)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<CommunicatorMessage>(str);
                return message;
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}
