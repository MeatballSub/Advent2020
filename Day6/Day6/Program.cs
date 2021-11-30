using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Day6
{
    class Group
    {
        public List<string> people;

        public int getYesCount()
        {
            HashSet<char> yes = new HashSet<char>();
            foreach(string person in people)
            {
                foreach(char question in person)
                {
                    yes.Add(question);
                }    
            }
            return yes.Count;
        }

        public override string ToString()
        {
            StringBuilder string_value = new StringBuilder();
            foreach(string person in people)
            {
                string_value.AppendLine(person);
            }
            string_value.AppendLine("Yes Count: " + getYesCount());
            string_value.AppendLine();
            return string_value.ToString();
        }

        public Group(List<string> persons)
        {
            people = new List<string>(persons);
        }
    }
    class Program
    {
        static List<Group> readInput(string file_name)
        {
            List<Group> groups = new List<Group>();

            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                List<string> lines = new List<string>();
                while((line = reader.ReadLine()) != null)
                {
                    bool new_group = (line == String.Empty);
                    if(new_group)
                    {
                        groups.Add(new Group(lines));
                        lines.Clear();
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
                if(lines.Count > 0)
                {
                    groups.Add(new Group(lines));
                }
            }

            return groups;
        }

        static void Main(string[] args)
        {
            List<Group> groups = readInput("input.txt");
            int sum = 0;
            foreach(Group group in groups)
            {
                //Console.WriteLine(group.ToString());
                sum += group.getYesCount();
            }
            Console.WriteLine("Sum: " + sum);
        }
    }
}