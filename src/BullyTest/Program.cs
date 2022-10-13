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
            ServerSocket sreverSocket = new ServerSocket(cts.Token);

            //for (int i = 0; i < 3; ++i)
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    startInfo.CreateNoWindow = false;
            //    startInfo.UseShellExecute = false;
            //    startInfo.FileName = processPath;
            //    startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //    System.Diagnostics.Process.Start(startInfo);
            //    startInfo.Arguments = "\\K";
            //}
            Console.ReadLine();

        }
        private static void ShutDownServer(object Sender,EventArgs args)
        {
            cts.Cancel();
        }
    }
}