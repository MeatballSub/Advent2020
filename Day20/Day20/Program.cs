using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day20
{
    class Tile
    {
        public int id;
        public char[,] image_data;
        public const int TILE_SIZE = 10;

        public Tile(TextReader reader)
        {
            image_data = new char[TILE_SIZE, TILE_SIZE];
            string line = reader.ReadLine();
            id = int.Parse(Regex.Match(line, @"^Tile (?<id>\d+):$").Groups["id"].Value);

            for(int row = 0; row < TILE_SIZE; ++row)
            {
                line = reader.ReadLine();
                for(int col = 0; col < TILE_SIZE; ++col)
                {
                    image_data[row, col] = line[col];
                }
            }
            reader.ReadLine();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Tile {0}:", id));

            for (int row = 0; row < TILE_SIZE; ++row)
            {
                for (int col = 0; col < TILE_SIZE; ++col)
                {
                    sb.Append(image_data[row, col]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private int reverse(int edge_code)
        {
            int reversed = 0;
            for(int i = 0; i < TILE_SIZE; ++i)
            {
                reversed <<= 1;
                if (edge_code % 2 == 1) ++reversed;
                edge_code >>= 1;
            }
            return reversed;
        }

        public List<int> getEdgeCodes()
        {
            List<int> edge_codes = new List<int>();

            int top = 0;
            int bottom = 0;
            int left = 0;
            int right = 0;

            for(int pos = 0; pos < TILE_SIZE; ++pos)
            {
                top <<= 1;
                bottom <<= 1;
                left <<= 1;
                right <<= 1;

                if (image_data[0, pos] == '#') ++top;
                if (image_data[TILE_SIZE - 1, pos] == '#') ++bottom;
                if (image_data[pos, 0] == '#') ++left;
                if (image_data[pos, TILE_SIZE - 1] == '#') ++right;
            }

            edge_codes.Add(top);
            edge_codes.Add(bottom);
            edge_codes.Add(left);
            edge_codes.Add(right);

            edge_codes.Add(reverse(top));
            edge_codes.Add(reverse(bottom));
            edge_codes.Add(reverse(left));
            edge_codes.Add(reverse(right));

            return edge_codes;
        }

        public int getMatchedEdgeCount(Dictionary<int,int> edge_code_count)
        {
            int matched_edge_codes = 0;

            List<int> edge_codes = getEdgeCodes();
            foreach(int edge_code in edge_codes)
            {
                if (edge_code_count[edge_code] > 1) ++matched_edge_codes;
            }

            return (matched_edge_codes / 2);
        }
    }
    class Program
    {
        static List<Tile> readInput(string file_name)
        {
            List<Tile> tiles = new List<Tile>();
            using(TextReader reader = File.OpenText(file_name))
            {
                while(reader.Peek() != -1)
                {
                    tiles.Add(new Tile(reader));
                }
            }
            return tiles;
        }

        static Int64 part1(string file_name)
        {
            Console.WriteLine("Processing: " + file_name);
            Dictionary<int, int> edge_code_counts = new Dictionary<int, int>();

            List<Tile> tiles = readInput(file_name);
            foreach(Tile tile in tiles)
            {
                foreach(int edge_code in tile.getEdgeCodes())
                {
                    if(!edge_code_counts.ContainsKey(edge_code))
                    {
                        edge_code_counts[edge_code] = 0;
                    }
                    ++edge_code_counts[edge_code];
                }
            }

            Int64 product = 1;
            foreach(Tile tile in tiles)
            {
                int matched_edge_count = tile.getMatchedEdgeCount(edge_code_counts);
                if (matched_edge_count == 2)
                {
                    Console.WriteLine("Tile: {0}", tile.id);
                    product *= tile.id;
                }
            }

            return product;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("sample_input.txt"));
            Console.WriteLine(part1("input.txt"));
            //Console.WriteLine(part1("sampel_input.txt"));
            //Console.WriteLine(part1("sampel_input.txt"));
        }
    }
}
