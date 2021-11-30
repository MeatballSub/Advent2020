using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        static bool isValidPart1(Passport passport)
        {
            bool fully_valid = (passport.data.Count == 8);
            bool north_pole_credentials = ((passport.data.Count == 7) && !passport.data.ContainsKey("cid"));
            return fully_valid || north_pole_credentials;
        }

        static bool validYr(int year, int min, int max)
        {
            return ((year >= min) && (year <= max));
        }

        static bool validByr(Passport passport)
        {
            int byr = int.Parse(passport.data["byr"]);
            return validYr(byr, 1920, 2002);
        }

        static bool validIyr(Passport passport)
        {
            int iyr = int.Parse(passport.data["iyr"]);
            return validYr(iyr, 2010, 2020);
        }

        static bool validEyr(Passport passport)
        {
            int eyr = int.Parse(passport.data["eyr"]);
            return validYr(eyr, 2020, 2030);
        }

        static bool validHgt(Passport passport)
        {
            bool valid = false;

            Regex height_regex = new Regex(@"^(?<height>\d+)(?<unit>in|cm)$");
            Match match = height_regex.Match(passport.data["hgt"]);
            if(match.Success)
            {
                int height = int.Parse(match.Groups["height"].Value);
                if(match.Groups["unit"].Value == "cm")
                {
                    valid = ((height >= 150) && (height <= 193));
                }
                else
                {
                    valid = ((height >= 59) && (height <= 76));
                }
            }

            return valid;
        }

        static bool validHcl(Passport passport)
        {
            Regex hair_color_regex = new Regex(@"^#[a-f0-9]{6}$");
            Match match = hair_color_regex.Match(passport.data["hcl"]);
            return match.Success;
        }

        static bool validEcl(Passport passport)
        {
            Regex eye_color_regex = new Regex(@"^(amb|blu|brn|gry|grn|hzl|oth)$");
            Match match = eye_color_regex.Match(passport.data["ecl"]);
            return match.Success;
        }

        static bool validPid(Passport passport)
        {
            Regex pid_regex = new Regex(@"^\d{9}$");
            Match match = pid_regex.Match(passport.data["pid"]);
            return match.Success;
        }

        static bool isValidPart2(Passport passport)
        {
            bool valid = isValidPart1(passport);
            if (valid)
            {
                valid = validByr(passport) && validIyr(passport) && validEyr(passport) && validHgt(passport) && validHcl(passport) && validEcl(passport) && validPid(passport);
            }
            return valid;
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
                if (isValidPart2(passport))
                {
                    ++count;
                    //Console.WriteLine(passport.ToString());
                }
            }
            Console.WriteLine("Valid passports: " + count);
        }
    }
}
