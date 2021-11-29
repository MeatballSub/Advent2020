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

        static int testSlope(List<string> map, Point slope)
        {
            Point location = new Point(0, 0);

            int count = 0;

            while (location.Y < map.Count)
            {
                if (isTree(map, location))
                {
                    ++count;
                }
                location.Offset(slope);
            }

            return count;
        }

        static void Main(string[] args)
        {
            List<string> input = readInput("input.txt");
            Point [] slopes = { new Point(1, 1), new Point(3, 1), new Point(5, 1), new Point(7, 1), new Point(1, 2), };

            int product = 1;
            foreach (Point slope in slopes)
            {
                int count = testSlope(input, slope);
                product *= count;
                Console.WriteLine("Trees hit: " + count);
            }

            Console.WriteLine("Answer: " + product);
        }
    }
}
