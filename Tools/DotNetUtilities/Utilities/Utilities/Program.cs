using System;

namespace Utilities
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var joiner = new TrafficDataJoiner();
            joiner.Run();
        }
    }
}
