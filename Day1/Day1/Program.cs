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
                bool[] numbers = new bool[2021];
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int number = int.Parse(reader.ReadLine());
                    numbers[number] = true;
                }
                for(int i = 0; i < 2021; ++i)
                {
                    if(numbers[i])
                    {
                        Console.WriteLine("Found: " + i);
                    }
                }
            }
        }
    }
}
