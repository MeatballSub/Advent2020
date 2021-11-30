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

        public static Dictionary<int, Int64> recurrence = new Dictionary<int, Int64>() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 1 } };

        static Int64 getRecurrence(int index)
        {
            // a_n = (2 * a_(n-1)) + (2^(n-4)) - a_(n-4)
            if (!recurrence.ContainsKey(index))
            {
                Int64 value = (2 * getRecurrence(index - 1)) + (1 << (index - 4)) - getRecurrence(index - 4);
                recurrence.Add(index, value);
            }

            return recurrence[index];
        }

        static Int64 getMultiplier(int index)
        {
            // multiplier = 2^n - a_n
            return ((1 << index) - getRecurrence(index));
        }

        static Int64 getArrangements(List<int> differences)
        {
            Int64 arrangements = 1;
            int consecutive = 0;
            for(int i = 0; i < differences.Count; ++i)
            {
                if(differences[i] == 1)
                {
                    ++consecutive;
                }
                else
                {
                    --consecutive;
                    if (consecutive > 0)
                    {
                        Int64 multiplier = getMultiplier(consecutive);
                        //Console.WriteLine("Consecutive: " + consecutive + ", Multiplier: " + multiplier);
                        arrangements *= multiplier;
                    }
                    consecutive = 0;
                }
            }

            return arrangements;
        }

        static void Main(string[] args)
        {
            int [] counts = new int[4] { 0,0,0,0 };
            List<int> joltages = readInput("input.txt");
            List<int> differences = new List<int>();
            joltages.Add(0);
            joltages.Sort();
            for(int i = 1; i < joltages.Count; ++i)
            {
                int difference = (joltages[i] - joltages[i - 1]);
                differences.Add(difference);
                ++counts[difference];
            }
            ++counts[3];
            differences.Add(3);
            Console.WriteLine("Differences: 0({0}) 1({1}) 2({2}) 3({3})", counts[0], counts[1], counts[2], counts[3]);
            Console.WriteLine("Answer: {0} * {1} = {2}", counts[1], counts[3], counts[1] * counts[3]);
            Console.WriteLine("Arrangements: " + getArrangements(differences));
        }
    }
}
