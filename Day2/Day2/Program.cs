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

        public override string ToString()
        {
            return string.Format("min({0}), max({1}), letter({2}), password({3})", min, max, letter, password);
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

        static int getLetterCount(char letter, string s)
        {
            int count = 0;
            foreach (char c in s)
            {
                if (c == letter)
                {
                    ++count;
                }
            }
            return count;
        }

        static bool isValidPart1(InputCase input_case)
        {
            int count = getLetterCount(input_case.letter, input_case.password);
            return ((count >= input_case.min) && (count <= input_case.max));
        }

        static bool isValidPart2(InputCase input_case)
        {
            return ((input_case.password[input_case.min-1] == input_case.letter) ^ (input_case.password[input_case.max-1] == input_case.letter));
        }

        static void Main(string[] args)
        {
            int valid_passwords = 0;
            List<InputCase> cases = readInput("input.txt");
            foreach(InputCase input_case in cases)
            {
                if(isValidPart2(input_case))
                {
                    ++valid_passwords;
                    //Console.WriteLine("Input Case: " + input_case.ToString());
                }
            }
            Console.WriteLine("Valid passwords: " + valid_passwords);
        }
    }
}
