using BullyAlgorithm.Services;

namespace ClusterServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var register = new ProcessesRegisterService();
            register.InitClusterServer();
        }
    }
}