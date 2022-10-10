using BullyAlgorithm.Interfaces;
using BullyAlgorithm.Services;

namespace BullyTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ICommunicator communicator = new Communicator();
            IMessageWritter messageWritter = new ConsoleWriter();

            IProcess p0 = new Process(0, communicator, messageWritter);
            IProcess p1 = new Process(1, communicator, messageWritter);
            IProcess p2 = new Process(2, communicator, messageWritter);
            IProcess p3 = new Process(3, communicator, messageWritter);
            p0.Run();
            p3.Run();
            p1.Run();
            p2.Run();

            //Thread.Sleep(5000);
            //p3.ShutDown();
            //IProcess p0 = new Process(0, communicator, messageWritter);


            //var rand = new Random();
            //var prIds = Enumerable.Range(1, 4).OrderBy(x => rand.Next()).ToList();
            //foreach (int id in prIds)
            //{
            //    IProcess p = new Process(id, communicator, messageWritter);
            //    p.Run();
            //}


            Console.ReadLine();
        }
    }
}