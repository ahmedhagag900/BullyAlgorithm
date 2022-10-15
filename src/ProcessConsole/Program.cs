using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System.Security.Cryptography;
namespace ProcessConsole
{
    internal class Program
    {
        private static IProcess _process;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += ShutDown;
            if(args.Length>0)
            {
                int.TryParse(args[0], out int processId);
                if(processId>0)
                {
                    _process = new Process(processId, new ConsoleWriter());
                    _process.Run();
                }
            }
        }

        private static void ShutDown(object? sender, EventArgs e)
        {
            _process?.ShutDown();
        }
    }
}