using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace BullyTest
{
    internal class Program
    {
        private static CancellationTokenSource cts=new CancellationTokenSource();
        static void Main(string[] args)
        {
            //var path= Environment.ProcessPath;

            //string processPath = path.Substring(0, path.IndexOf("src") + 3);
            //processPath += "\\ProcessConsole\\bin\\Debug\\net6.0\\ProcessConsole.exe";

            //AppDomain.CurrentDomain.ProcessExit += ShutDownServer;


            ProcessesRegisterService _registerServies = new ProcessesRegisterService();

            Task.Run(() => _registerServies.InitClusterServer());

            Thread.Sleep(1000);
            var p1 = new BullyAlgorithm.Services.Process(1, new ConsoleWriter());
            var p2 = new BullyAlgorithm.Services.Process(2, new ConsoleWriter());
            var p3 = new BullyAlgorithm.Services.Process(3, new ConsoleWriter());
            Task.Run(() => p1.Run());
            Thread.Sleep(1000);
            Task.Run(() => p2.Run());
            Thread.Sleep(2000);
            Task.Run(() => p3.Run());

            Thread.Sleep(5000);
            p1.ShutDown();
            Thread.Sleep(15000);
            Task.Run(() => p1.Run());
            Thread.Sleep(1000);
            p3.ShutDown();

            Console.ReadLine();

        }
        private static void ShutDownServer(object Sender,EventArgs args)
        {
        }
    }
}