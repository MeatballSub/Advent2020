using System;
using System.Collections.Generic;
using System.IO;

namespace Day10
{
    class Program
    {
        static List<int> readInput(string file_name)
        {
            List<int> joltages = new List<int>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    joltages.Add(int.Parse(line));
                }
            }
            return joltages;
        }
        static void Main(string[] args)
        {
            int [] counts = new int[4] { 0,0,0,0 };
            List<int> joltages = readInput("input.txt");
            joltages.Add(0);
            joltages.Sort();
            for(int i = 1; i < joltages.Count; ++i)
            {
                int difference = (joltages[i] - joltages[i - 1]);
                ++counts[difference];
            }
            ++counts[3];
            Console.WriteLine("Differences: 0({0}) 1({1}) 2({2}) 3({3})", counts[0], counts[1], counts[2], counts[3]);
            Console.WriteLine("Answer: {0} * {1} = {2}", counts[1], counts[3], counts[1] * counts[3]);
        }
    }
}
