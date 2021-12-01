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

        static Point rotate(Point point, int left_rotate_index)
        {
            List<List<int>> rotations = new List<List<int>>() { new List<int>(){1,0,0,1}, new List<int>() { 0, -1, 1, 0 }, new List<int>() { -1, 0, 0, -1 }, new List<int>() { 0, 1, -1, 0 } };
            List<int> rotation = rotations[left_rotate_index];
            return new Point(rotation[0] * point.X + rotation[1] * point.Y, rotation[2] * point.X + rotation[3] * point.Y);
        }

        static Point execute1(List<Instruction> instructions)
        {
            List<Point> directions = new List<Point>() { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
            Point offset = new Point(0, 0);
            int facing = 0;
            foreach (Instruction instruction in instructions)
            {
                switch (instruction.type)
                {
                    case 'N':
                        offset.Offset(0, instruction.operand);
                        break;
                    case 'S':
                        offset.Offset(0, -instruction.operand);
                        break;
                    case 'E':
                        offset.Offset(instruction.operand, 0);
                        break;
                    case 'W':
                        offset.Offset(-instruction.operand, 0);
                        break;
                    case 'L':
                        facing = (facing + (instruction.operand / 90)) % directions.Count;
                        break;
                    case 'R':
                        facing = (facing - (instruction.operand / 90)) % directions.Count;
                        if (facing < 0)
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

        static Point execute2(List<Instruction> instructions)
        {
            Point offset = new Point(0, 0);
            Point facing = new Point(10, 1);
            int left_rotate_index = 0;
            foreach (Instruction instruction in instructions)
            {
                switch (instruction.type)
                {
                    case 'N':
                        facing.Offset(0, instruction.operand);
                        break;
                    case 'S':
                        facing.Offset(0, -instruction.operand);
                        break;
                    case 'E':
                        facing.Offset(instruction.operand, 0);
                        break;
                    case 'W':
                        facing.Offset(-instruction.operand, 0);
                        break;
                    case 'L':
                        left_rotate_index = (instruction.operand / 90) % 4;
                        facing = rotate(facing, left_rotate_index);
                        break;
                    case 'R':
                        left_rotate_index = (-instruction.operand / 90) % 4;
                        if (left_rotate_index < 0)
                        {
                            left_rotate_index += 4;
                        }
                        facing = rotate(facing, left_rotate_index);
                        break;
                    case 'F':
                        offset.Offset(facing.X * instruction.operand, facing.Y * instruction.operand);
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
            //Point p = new Point(1, 0);
            //Console.WriteLine(rotate(p, 1));
            //Console.WriteLine(rotate(p, 2));
            //Console.WriteLine(rotate(p, 3));
            //Console.WriteLine(rotate(p, 0));

            List<Instruction> instructions = readInput("input.txt");
            //foreach(Instruction instruction in instructions)
            //{
            //    Console.WriteLine(instruction.ToString());
            //}
            Point offset = execute2(instructions);
            Console.WriteLine(offset.ToString());
            int manhattan_distance = getManhattanDistance(offset);
            Console.WriteLine("Manhattan Distance: " + manhattan_distance);
        }
    }
}
