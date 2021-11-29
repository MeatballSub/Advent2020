using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Day2
{
    public struct InputCase
    {
        public int min;
        public int max;
        public char letter;
        public string password;

        public InputCase(string line)
        {
            min = 0;
            max = 0;
            letter = Char.MinValue;
            password = null;

            Regex input_format = new Regex(@"(?<min>\d+)-(?<max>\d+)\s+(?<letter>.):\s+(?<password>.*)");
            Match matches = input_format.Match(line);
            if(matches.Success)
            {
                min = int.Parse(matches.Groups["min"].Value);
                max = int.Parse(matches.Groups["max"].Value);
                letter = matches.Groups["letter"].Value[0];
                password = matches.Groups["password"].Value;
            }
        }
    }

    class Program
    {
        static List<InputCase> readInput(string file_name)
        {
            List<InputCase> cases = new List<InputCase>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    cases.Add(new InputCase(line));
                }
            }
            return cases;
        }

        static void Main(string[] args)
        {
            List<InputCase> cases = readInput("sample_input.txt");
            foreach(InputCase input_case in cases)
            {
                Console.WriteLine("Input Case: min({0}), max({1}), letter({2}), password({3})", input_case.min, input_case.max, input_case.letter, input_case.password);
            }
        }
    }
}
