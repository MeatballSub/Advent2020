using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day7
{
    class Program
    {
        public class Bag
        {
            public string color_name;
            public Dictionary<string, int> contains;
            public List<string> placeable;

            public Bag(string color, Dictionary<string, int> contents)
            {
                color_name = color;
                contains = new Dictionary<string, int>(contents);
                placeable = new List<string>();
            }

            public Bag(string color)
            {
                color_name = color;
                contains = new Dictionary<string, int>();
                placeable = new List<string>();
            }

            public override string ToString()
            {
                StringBuilder string_value = new StringBuilder();
                string_value.AppendLine(string.Format("color({0})", color_name));
                string_value.AppendLine("Contains:");
                foreach(KeyValuePair<string,int> kvp in contains)
                {
                    string_value.AppendLine(string.Format("    Color({0}) Count({1})", kvp.Key, kvp.Value));
                }
                string_value.AppendLine("Placeable in:");
                foreach(string color in placeable)
                {
                    string_value.AppendLine("    " + color);
                }
                string_value.AppendLine();
                return string_value.ToString();
            }
        }

        public static Dictionary<string, Bag> bags;

        static void AddBag(string rule)
        {
            Dictionary<string, int> contains = new Dictionary<string, int>();
            string color_name = null;

            Regex bag_regex = new Regex(@"(?<color>.*) bags contain (?<content>\d+ .*? bags*[,.]\s*)*");
            Match matches = bag_regex.Match(rule);
            if (matches.Success)
            {
                color_name = new string(matches.Groups["color"].Value);
                foreach (Capture capture in matches.Groups["content"].Captures)
                {
                    Regex content_regex = new Regex(@"^(?<count>\d+) (?<color>.*?) bags*[,.]\s*$");
                    Match content_matches = content_regex.Match(capture.Value);
                    if (content_matches.Success)
                    {
                        contains[content_matches.Groups["color"].Value] = int.Parse(content_matches.Groups["count"].Value);
                    }
                }
            }

            if(bags.ContainsKey(color_name))
            {
                bags[color_name].contains = contains;
            }
            else
            {
                Bag bag = new Bag(color_name, contains);
                bags.Add(color_name, bag);
            }

            foreach(string color in contains.Keys)
            {
                if(!bags.ContainsKey(color))
                {
                    Bag placeable_bag = new Bag(color);
                    bags.Add(color, placeable_bag);
                }
                bags[color].placeable.Add(color_name);
            }
        }

        static void readInput(string file_name)
        {
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    AddBag(line);
                }
            }
        }

        static int canEventuallyContain(string start_color)
        {
            HashSet<string> placeable = new HashSet<string>();
            Queue<string> frontier = new Queue<string>();

            frontier.Enqueue(start_color);
            while (frontier.Count > 0)
            {
                string front = frontier.Dequeue();
                foreach (string color in bags[front].placeable)
                {
                    try
                    {
                        placeable.Add(color);
                        frontier.Enqueue(color);
                    }
                    catch
                    {

                    }
                }
            }

            return placeable.Count;
        }

        static int mustContain(string color)
        {
            int count = 0;

            foreach(KeyValuePair<string,int> kvp in bags[color].contains)
            {
                count += kvp.Value;
                count += kvp.Value * mustContain(kvp.Key);
            }

            return count;
        }

        static void Main(string[] args)
        {
            string color = "shiny gold";
            bags = new Dictionary<string, Bag>();
            readInput("input.txt");

            //foreach (KeyValuePair<string, Bag> kvp in bags)
            //{
            //    Console.WriteLine(kvp.Value.ToString());
            //}
            Console.WriteLine("Can eventually contain '{0}': {1}", color, canEventuallyContain(color));
            Console.WriteLine("Can eventually contain '{0}': {1}", color, mustContain(color));
        }
    }
}
