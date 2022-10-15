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
        /// <summary>
        /// convert the string to message
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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
