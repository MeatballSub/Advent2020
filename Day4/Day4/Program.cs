using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Day4
{
    class Passport
    {
        public Dictionary<string, string> data;

        public Passport(List<string> lines)
        {
            data = new Dictionary<string, string>();
            string one_line = String.Join(" ", lines);
            string [] fields = one_line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach(string field in fields)
            {
                string[] kvp = field.Split(':');
                data.Add(kvp[0], kvp[1]);
            }
        }

        public override string ToString()
        {
            StringBuilder string_value = new StringBuilder();
            foreach(KeyValuePair<string, string> kvp in data)
            {
                string_value.AppendLine(kvp.Key + ":" + kvp.Value);
            }
            string_value.AppendLine();
            return string_value.ToString();
        }
    }

    class Program
    {
        static bool isValid(Passport passport)
        {
            bool fully_valid = (passport.data.Count == 8);
            bool north_pole_credentials = ((passport.data.Count == 7) && !passport.data.ContainsKey("cid"));
            return fully_valid || north_pole_credentials;
        }

        static List<Passport> readInput(string file_name)
        {
            List<Passport> passports = new List<Passport>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                List<string> lines = new List<string>();
                while((line = reader.ReadLine()) != null)
                {
                    bool new_passport = (line == String.Empty);
                    if(new_passport)
                    {
                        passports.Add(new Passport(lines));
                        lines.Clear();
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
                if(lines.Count > 0)
                {
                    passports.Add(new Passport(lines));
                }
            }
            return passports;
        }

        static void Main(string[] args)
        {
            List<Passport> passports = readInput("input.txt");
            int count = 0;
            foreach(Passport passport in passports)
            {
                if (isValid(passport))
                {
                    ++count;
                    //Console.WriteLine(passport.ToString());
                }
            }
            Console.WriteLine("Valid passports: " + count);
        }
    }
}
