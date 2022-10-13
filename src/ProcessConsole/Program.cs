using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System.Security.Cryptography;
namespace ProcessConsole
{
    internal class Program
    {

        private static SyncProcess _process ;
        private static SyncProcess _process2 ;
        static void Main(string[] args)
        {
            //if (args.Length > 0)
            //{
                //var pId = int.Parse(args[0]);
                var messageWritter = new ConsoleWriter();
                _process = new SyncProcess(0, messageWritter);
                _process2 = new SyncProcess(1, messageWritter);
                AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
                Task.Run(() => _process2.Run());
            Thread.Sleep(5000);
            _process.Run();

            Console.ReadLine();
           // }
        }

        private static void ProcessExitHandler(object sender,EventArgs args)
        {
            _process?.ShutDown();
        }
    }
}