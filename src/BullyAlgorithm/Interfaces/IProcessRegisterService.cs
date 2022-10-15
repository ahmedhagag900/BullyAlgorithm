using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullyAlgorithm.Interfaces
{
    /// <summary>
    /// this interface acts as the processes register serviec which allow processes to discover each other in the cluster
    /// refer to https://www.nginx.com/blog/service-discovery-in-a-microservices-architecture/#:~:text=The%20service%20registry%20is%20a,obtained%20from%20the%20service%20registry.
    /// </summary>
    public interface IProcessRegisterService
    {
        void InitClusterServer();
    }
}
