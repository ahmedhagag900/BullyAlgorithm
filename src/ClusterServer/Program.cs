using BullyAlgorithm.Services;

namespace ClusterServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var regestirService = new ProcessesRegisterService();
            regestirService.InitClusterServer();
        }
    }
}