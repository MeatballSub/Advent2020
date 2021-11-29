using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Day3
{
    class Program
    {
        static List<string> readInput(string file_name)
        {
            List<string> lines = new List<string>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        static bool isTree(List<string> map, Point location)
        {
            int width = map[location.Y].Length;
            return (map[location.Y][location.X % width] == '#');
        }

        static void Main(string[] args)
        {
            List<string> input = readInput("input.txt");
            Point location = new Point(0, 0);
            Point movement = new Point(3, 1);

            int count = 0;

            while(location.Y < input.Count)
            {
                if(isTree(input, location))
                {
                    ++count;
                }
                location.Offset(movement);
            }

            Console.WriteLine("Trees hit: " + count);
        }
    }
}
