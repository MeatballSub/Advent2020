using System;
using System.IO;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            using(TextReader reader = File.OpenText("input.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int number = int.Parse(reader.ReadLine());
                    Console.WriteLine("Read: " + number);
                }
            }
        }
    }
}
