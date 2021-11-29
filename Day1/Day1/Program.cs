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
                    int number = int.Parse(line);
                    int match = 2020 - number;
                    if (numbers[match])
                    {
                        Console.WriteLine("Answer: {0} * {1} = {2}", number, match, number * match);
                    }
                    else
                    {
                        numbers[number] = true;
                    }
                }
            }
        }
    }
}
