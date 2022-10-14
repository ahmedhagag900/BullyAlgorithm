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
            var path= Environment.ProcessPath;

            string processPath = path.Substring(0, path.IndexOf("src") + 3);
            processPath += "\\ProcessConsole\\bin\\Debug\\net6.0\\ProcessConsole.exe";

            AppDomain.CurrentDomain.ProcessExit += ShutDownServer;


            ServerSocket sreverSocket = new ServerSocket();

            SyncProcess p0 = new SyncProcess(0, new ConsoleWriter());
            SyncProcess p1 = new SyncProcess(1, new ConsoleWriter());

            Task.Run(() => p1.Run());
            p0.Run();



            Console.ReadLine();

        }
        private static void ShutDownServer(object Sender,EventArgs args)
        {
        }
    }
}