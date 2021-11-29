using System;
using System.Collections.Generic;
using System.IO;

namespace Day1
{
    class Program
    {
        static Tuple<int, int> findSumInRange(List<int> numbers, int target_sum, int start_index, int stop_index)
        {
            Tuple<int, int> answer = null;
            bool[] numbers_seen = new bool[target_sum + 1];
            for(int i=start_index; i < stop_index; ++i)
            {
                int number = numbers[i];
                int match = target_sum - number;
                if(match >= 0)
                {
                    if (numbers_seen[match])
                    {
                        answer = new Tuple<int, int>(number, match);
                        break;
                    }
                    else
                    {
                        numbers_seen[number] = true;
                    }
                }
            }
            return answer;
        }

        static List<int> readNumbers(string file_name)
        {
            List<int> numbers = new List<int>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int number = int.Parse(line);
                    numbers.Add(number);
                }
            }
            return numbers;
        }

        static void Main(string[] args)
        {
            List<int> numbers = readNumbers("input.txt");
            for (int i = 0; i < numbers.Count; ++i)
            {
                int target = 2020 - numbers[i];
                Tuple<int, int> pair = findSumInRange(numbers, target, 0, i);
                if (pair != null)
                {
                    Console.WriteLine("Answer: {0} * {1} * {2} = {3}", pair.Item1, pair.Item2, numbers[i], pair.Item1 * pair.Item2 * numbers[i]);
                    break;
                }
            }
        }
    }
}
