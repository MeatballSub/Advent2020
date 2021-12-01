using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Day12
{
    class Instruction
    {
        public char type;
        public int operand;

        public Instruction(string line)
        {
            type = line[0];
            operand = int.Parse(line.Substring(1));
        }

        public override string ToString()
        {
            return string.Format("type({0}) operand({1})", type, operand);
        }
    }

    class Program
    {
        static List<Instruction> readInput(string file_name)
        {
            List<Instruction> instructions = new List<Instruction>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    instructions.Add(new Instruction(line));
                }
            }
            return instructions;
        }

        static Point execute(List<Instruction> instructions)
        {
            List<Point> directions = new List<Point>() { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
            Point offset = new Point(0, 0);
            int facing = 0;
            foreach(Instruction instruction in instructions)
            {
                switch(instruction.type)
                {
                    case 'N':
                        offset.Offset(0, instruction.operand);
                        break;
                    case 'S':
                        offset.Offset(0, -instruction.operand);
                        break;
                    case 'E':
                        offset.Offset(instruction.operand,0);
                        break;
                    case 'W':
                        offset.Offset(-instruction.operand, 0);
                        break;
                    case 'L':
                        facing = (facing + (instruction.operand / 90)) % directions.Count;
                        break;
                    case 'R':
                        facing = (facing - (instruction.operand / 90)) % directions.Count;
                        if(facing < 0)
                        {
                            facing += directions.Count;
                        }
                        break;
                    case 'F':
                        offset.Offset(directions[facing].X * instruction.operand, directions[facing].Y * instruction.operand);
                        break;
                }
            }

            return offset;
        }

        static int getManhattanDistance(Point point)
        {
            return Math.Abs(point.X) + Math.Abs(point.Y);
        }

        static void Main(string[] args)
        {
            List<Instruction> instructions = readInput("input.txt");
            //foreach(Instruction instruction in instructions)
            //{
            //    Console.WriteLine(instruction.ToString());
            //}
            Point offset = execute(instructions);
            Console.WriteLine(offset.ToString());
            int manhattan_distance = getManhattanDistance(offset);
            Console.WriteLine("Manhattan Distance: " + manhattan_distance);
        }
    }
}
