using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        static Tuple<Int64, Int64> findSumInRange(List<Int64> numbers, Int64 target_sum, int start_index, int stop_index)
        {
            Tuple<Int64, Int64> answer = null;
            bool[] numbers_seen = new bool[target_sum + 1];
            for (int i = start_index; i < stop_index; ++i)
            {
                Int64 number = numbers[i];
                Int64 match = target_sum - number;
                if (match >= 0)
                {
                    if (numbers_seen[match])
                    {
                        answer = new Tuple<Int64, Int64>(number, match);
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

        static List<Int64> readInput(string file_name)
        {
            List<Int64> numbers = new List<Int64>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Int64 number = Int64.Parse(line);
                    numbers.Add(number);
                }
            }
            return numbers;
        }

        static (int index, Int64 value) findFirstMismatch(List<Int64> input, int preamble_length)
        {
            int index = -1;
            Int64 value = -1;
            for(int i = preamble_length; i < input.Count; ++i)
            {
                Tuple<Int64, Int64> foundSum = findSumInRange(input, input[i], i - preamble_length, i);
                if(foundSum == null)
                {
                    index = i;
                    value = input[i];
                    break;
                }
            }
            return (index, value);
        }

        static (int, int) findContiguousSum(List<Int64> input, Int64 target)
        {
            int start = 0;
            int end = 0;
            Int64 sum = 0;

            while(sum != target)
            {
                if(sum < target)
                {
                    sum += input[end];
                    ++end;
                }
                else
                {
                    sum -= input[start];
                    ++start;
                }
            }

            return (start, end);
        }

        static Int64 findMinMaxSum(List<Int64> input, int start_index, int stop_index)
        {
            Int64 min = input.GetRange(start_index, stop_index - start_index).Min();
            Int64 max = input.GetRange(start_index, stop_index - start_index).Max();

            return min + max;
        }

        static void Main(string[] args)
        {
            List<Int64> input = readInput("input.txt");
            (int index, Int64 value) = findFirstMismatch(input, 25);
            (int start, int end) = findContiguousSum(input, value);
            Console.WriteLine("Answer: " + findMinMaxSum(input, start, end));
        }
    }
}
