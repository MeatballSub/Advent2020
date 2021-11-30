using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    class BoardingPass
    {
        public string code;
        public int row;
        public int column;
        public int seat;

        static int decode(string c, int first, int size, char one_char)
        {
            int value = 0;

            for(int i = first; i < (first + size); ++i)
            {
                value = (value << 1);
                if(c[i] == one_char)
                {
                    ++value;
                }
            }

            return value;
        }

        public BoardingPass(string c)
        {
            code = c;

            row = decode(c, 0, 7,'B');
            column = decode(c, 7, 3, 'R');
            seat = row * 8 + column;
        }

        public override string ToString()
        {
            return String.Format("code({0}), row({1}), column({2}), seat({3})", code, row, column, seat);
        }
    }
    class Program
    {
        static List<BoardingPass> readInput(string file_name)
        {
            List<BoardingPass> boarding_passes = new List<BoardingPass>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    boarding_passes.Add(new BoardingPass(line));
                }
            }
            return boarding_passes;
        }

        static void Main(string[] args)
        {
            List<BoardingPass> boarding_passes = readInput("input.txt");
            //foreach(BoardingPass pass in boarding_passes)
            //{
            //    Console.WriteLine(pass.ToString());
            //}
            Console.WriteLine("Max seat: " + boarding_passes.Max(t => t.seat));
        }
    }
}
