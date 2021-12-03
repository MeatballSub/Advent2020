using System;
using System.Collections.Generic;
using System.IO;

namespace Day15
{
    public class SpokenMemory
    {
        private int index;
        private int last;
        private Dictionary<int, List<int>> memory;

        public SpokenMemory(List<int> starting_numbers)
        {
            index = 0;
            memory = new Dictionary<int, List<int>>();
            foreach(int number in starting_numbers)
            {
                insert(number);
            }
        }

        private int getNextNumber()
        {
            int next_number = 0;
            var spoken_list = memory[last];
            if (spoken_list.Count > 1)
            {
                next_number = spoken_list[spoken_list.Count - 1] - spoken_list[spoken_list.Count - 2];
            }
            return next_number;
        }

        private void insert(int number)
        {
            ++index;
            if (!memory.ContainsKey(number))
            {
                memory[number] = new List<int>();
            }
            memory[number].Add(index);
            last = number;
        }

        public (int, int) next()
        {
            int next_number = getNextNumber();
            insert(next_number);
            return (index, next_number);
        }
    }
    class Program
    {
        static List<int> readFile(string file_name)
        {
            List<int> starting_numbers = new List<int>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var numbers = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string number in numbers)
                    {
                        starting_numbers.Add(int.Parse(number));
                    }
                }
            }
            return starting_numbers;
        }

        static int getNth(string file_name, int n)
        {
            List<int> starting_numbers = readFile(file_name);
            SpokenMemory spoken_memory = new SpokenMemory(starting_numbers);
            int index = 0;
            int number = 0;
            while (((index, number) = spoken_memory.next()).index < n) ;
            return number;
        }

        static int part1(string file_name)
        {
            return getNth(file_name, 2020);
        }

        static int part2(string file_name)
        {
            return getNth(file_name, 30000000);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part2("sample_input1.txt"));
            Console.WriteLine(part2("sample_input2.txt"));
            Console.WriteLine(part2("sample_input3.txt"));
            Console.WriteLine(part2("sample_input4.txt"));
            Console.WriteLine(part2("sample_input5.txt"));
            Console.WriteLine(part2("sample_input6.txt"));
            Console.WriteLine(part2("sample_input7.txt"));
            Console.WriteLine(part2("input.txt"));
        }
    }
}
