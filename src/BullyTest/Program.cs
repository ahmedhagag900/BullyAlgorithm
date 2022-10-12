using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace BullyTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ICommunicator communicator = new Communicator();
            //IMessageWritter messageWritter = new ConsoleWriter();

            //IProcess p0 = new Process(0, communicator, messageWritter);
            //IProcess p1 = new Process(1, communicator, messageWritter);
            //IProcess p2 = new Process(2, communicator, messageWritter);
            //IProcess p3 = new Process(3, communicator, messageWritter);
            //p0.Run();
            //p1.Run();
            //p2.Run();
            //p3.Run();


            var comm = new SocketCommunicator();
            var wirter = new ConsoleWriter();

            var p1 = new SocketProcess(1, comm, wirter);
            var p2 = new SocketProcess(2, comm, wirter);
            var p3 = new SocketProcess(3, comm, wirter);

            p3.Run();

            p2.Run();

            p1.Run();
            Thread.Sleep(5000);
            p3.ShutDown();
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            //var _connectAddress = new IPEndPoint(ipAddress, 1000);
            //var _client = new Socket(_connectAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //_client.Connect(_connectAddress);

            //_client.Send(Encoding.ASCII.GetBytes("data to all"));

            //Thread.Sleep(5000);
            //p3.ShutDown();
            //IProcess p0 = new Process(0, communicator, messageWritter);


            //var rand = new Random();
            //var prIds = Enumerable.Range(1, 3).OrderBy(x => rand.Next()).ToList();
            //foreach (int id in prIds)
            //{
            //    SocketProcess p = new SocketProcess(id, comm, wirter);
            //    p.Run();
            //}


            Console.ReadLine();
        }
    }
}