using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Day20
{
    class Tile
    {
        static readonly Vector2 CENTER_POINT = new Vector2(4.5f, 4.5f);
        public static float degreesToRadians(int d) => (float)(d * Math.PI / 180);

        public int id;
        public char[,] image_data;
        public const int TILE_SIZE = 10;
        public (int rotation, bool flipped) orientation;

        public Tile(TextReader reader)
        {
            orientation = (0,false);
            image_data = new char[TILE_SIZE, TILE_SIZE];
            string line = reader.ReadLine();
            id = int.Parse(Regex.Match(line, @"^Tile (?<id>\d+):$").Groups["id"].Value);

            for(int row = 0; row < TILE_SIZE; ++row)
            {
                line = reader.ReadLine();
                for(int col = 0; col < TILE_SIZE; ++col)
                {
                    image_data[col, row] = line[col];
                }
            }
            reader.ReadLine();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Tile {0}:", id));
            sb.AppendLine("Normal orientation -");

            for (int row = 0; row < TILE_SIZE; ++row)
            {
                for (int col = 0; col < TILE_SIZE; ++col)
                {
                    sb.Append(image_data[col, row]);
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            sb.AppendLine(string.Format("Orientation: rotation({0}), flipped({1}) -", orientation.rotation, orientation.flipped));
            for (int row = 0; row < TILE_SIZE; ++row)
            {
                for (int col = 0; col < TILE_SIZE; ++col)
                {
                    sb.Append(getOrientedImageData(col, row));
                }
                sb.AppendLine();
            }

            sb.AppendLine(string.Format("Top: {0}",getTopEdgeCode()));
            sb.AppendLine(string.Format("Left: {0}", getLeftEdgeCode()));
            sb.AppendLine(string.Format("Right: {0}", getRightEdgeCode()));
            sb.AppendLine(string.Format("Bottom: {0}", getBottomEdgeCode()));

            return sb.ToString();
        }

        private int reverseEdgeCode(int edge_code)
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
            return new List<int>()
            {
                getEdgeCode(0, EdgeType.Row),
                getEdgeCode(TILE_SIZE - 1, EdgeType.Row),
                getEdgeCode(0, EdgeType.Column),
                getEdgeCode(TILE_SIZE - 1, EdgeType.Column),
                reverseEdgeCode(getEdgeCode(0, EdgeType.Row)),
                reverseEdgeCode(getEdgeCode(TILE_SIZE - 1, EdgeType.Row)),
                reverseEdgeCode(getEdgeCode(0, EdgeType.Column)),
                reverseEdgeCode(getEdgeCode(TILE_SIZE - 1, EdgeType.Column)),
            };
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

        public char getOrientedImageData(int col, int row)
        {
            Vector2 oriented_point = new Vector2(col, row);
            if (orientation.flipped)
            {
                oriented_point.X = (TILE_SIZE - 1) - oriented_point.X;
            }
            if (orientation.rotation != 0)
            {
                Matrix3x2 rotation = Matrix3x2.CreateRotation(degreesToRadians(orientation.rotation), CENTER_POINT);
                oriented_point = Vector2.Transform(oriented_point, rotation);
            }

            return image_data[(int)oriented_point.X, (int)oriented_point.Y];
        }

        private enum EdgeType
        {
            Row,
            Column
        };

        private int getEdgeCode(int index, EdgeType edge_type)
        {
            int edge_code = 0;

            for (int pos = 0; pos < TILE_SIZE; ++pos)
            {
                edge_code <<= 1;
                char c = (edge_type == EdgeType.Row) ? getOrientedImageData(pos, index) : getOrientedImageData(index, pos);
                if (c == '#') ++edge_code;
            }

            return edge_code;

        }

        public int getTopEdgeCode()
        {
            return getEdgeCode(0, EdgeType.Row);
        }

        public int getBottomEdgeCode()
        {
            return getEdgeCode(TILE_SIZE - 1, EdgeType.Row);
        }

        public int getLeftEdgeCode()
        {
            return getEdgeCode(0, EdgeType.Column);
        }

        public int getRightEdgeCode()
        {
            return getEdgeCode(TILE_SIZE - 1, EdgeType.Column);
        }

    }

    class Image
    {
        Tile[,] tile_map;
        char[,] image_data;
        Dictionary<int, List<Tile>> matches;
        Dictionary<int, int> edge_code_counts;
        HashSet<int> used;

        public static float degreesToRadians(int d) => (float)(d * Math.PI / 180);

        private void generateStats(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                foreach (int edge_code in tile.getEdgeCodes())
                {
                    if (!edge_code_counts.ContainsKey(edge_code))
                    {
                        edge_code_counts[edge_code] = 0;
                    }
                    ++edge_code_counts[edge_code];

                    if (!matches.ContainsKey(edge_code))
                    {
                        matches[edge_code] = new List<Tile>();
                    }
                    matches[edge_code].Add(tile);
                }
            }
        }

        private bool matchesUp(int row, int col, Tile tile)
        {
            bool match = false;

            if(row == 0)
            {
                match = (tile.getMatchedEdgeCount(edge_code_counts) < 4);
            }
            else
            {
                Tile up_tile = tile_map[row - 1, col];
                int code_to_match = up_tile.getBottomEdgeCode();
                match = matches[code_to_match].Contains(tile);
            }

            return match;
        }

        private bool matchesLeft(int row, int col, Tile tile)
        {
            bool match = false;

            if (col == 0)
            {
                match = (tile.getMatchedEdgeCount(edge_code_counts) < 4);
            }
            else
            {
                Tile left_tile = tile_map[row, col - 1];
                int code_to_match = left_tile.getRightEdgeCode();
                match = matches[code_to_match].Contains(tile);
            }

            return match;
        }

        private void orientAppropriately(int row, int col, Tile tile)
        {
            if(row == 0)
            {
                for (tile.orientation.rotation = 0; true; tile.orientation.rotation += 90)
                {
                    if (matches[tile.getTopEdgeCode()].Count < 2)
                    {
                        break;
                    }
                }

                if(col == 0)
                {
                    tile.orientation.flipped = false;
                    int left_edge_code = tile.getLeftEdgeCode();
                    tile.orientation.flipped = (matches[left_edge_code].Count >= 2);
                }
                else
                {
                    int code_to_match = tile_map[row, col - 1].getRightEdgeCode();
                    tile.orientation.flipped = false;
                    int left_edge_code = tile.getLeftEdgeCode();
                    tile.orientation.flipped = (left_edge_code != code_to_match);
                }
            }
            else
            {
                int code_to_match = tile_map[row - 1, col].getBottomEdgeCode();

                for (tile.orientation.rotation = 0; true; tile.orientation.rotation += 90)
                {
                    tile.orientation.flipped = false;
                    if(tile.getTopEdgeCode() == code_to_match)
                    {
                        break;
                    }
                    tile.orientation.flipped = true;
                    if (tile.getTopEdgeCode() == code_to_match)
                    {
                        break;
                    }
                }
            }
        }

        private Tile chooseTile(int row, int col, List<Tile> tiles)
        {
            Tile chosen_tile = null;

            if (row == 0 && col == 0)
            {
                chosen_tile = tiles.Where(t => t.getMatchedEdgeCount(edge_code_counts) == 2).First();
            }
            else
            {
                chosen_tile = tiles.Where(t => matchesUp(row, col, t)).Where(t => matchesLeft(row, col, t)).Where(t => !used.Contains(t.id)).First();
            }

            orientAppropriately(row, col, chosen_tile);
            used.Add(chosen_tile.id);

            return chosen_tile;
        }

        void init(List<Tile> tiles)
        {
            matches = new Dictionary<int, List<Tile>>();
            edge_code_counts = new Dictionary<int, int>();
            used = new HashSet<int>();

            int map_dimension = (int)Math.Sqrt(tiles.Count);
            tile_map = new Tile[map_dimension, map_dimension];
        }

        void assembleTiles(List<Tile> tiles)
        {
            generateStats(tiles);

            for (int row = 0; row < tile_map.GetLength(0); ++row)
            {
                for (int col = 0; col < tile_map.GetLength(1); ++col)
                {
                    tile_map[row, col] = chooseTile(row, col, tiles);
                }
            }
        }

        void stripBorders()
        {
            int tile_map_size = tile_map.GetLength(0);
            int borderless_tile_size = tile_map[0,0].image_data.GetLength(0) - 2;
            int image_data_size = tile_map_size * borderless_tile_size;
            image_data = new char[image_data_size,image_data_size];
            for (int row = 0; row < tile_map.GetLength(0); ++row)
            {
                for (int col = 0; col < tile_map.GetLength(1); ++col)
                {
                    Tile tile = tile_map[row, col];
                    for(int y = 0; y < borderless_tile_size; ++y)
                    {
                        for (int x = 0; x < borderless_tile_size; ++x)
                        {
                            int image_data_x = col * borderless_tile_size + x;
                            int image_data_y = row * borderless_tile_size + y;
                            char value = tile.getOrientedImageData(x + 1, y + 1);
                            image_data[image_data_x, image_data_y] = value;
                        }
                    }
                }
            }
        }

        void hiliteSeaMonsters(int rotation, bool flipped)
        {
            List<string> seaMonsterPattern = new List<string>();
            seaMonsterPattern.Add("                  # ");
            seaMonsterPattern.Add("#    ##    ##    ###");
            seaMonsterPattern.Add(" #  #  #  #  #  #   ");

            bool boundsCheck(Point image_point)
            {
                return (image_point.X > 0 && image_point.Y > 0 && image_point.X < image_data.GetLength(0) && image_point.Y < image_data.GetLength(1));
            }

            char getOrientedImageData(int col, int row)
            {
                int IMAGE_SIZE = image_data.GetLength(0);
                float CENTER_VALUE = (float)(IMAGE_SIZE - 1) / 2;
                Vector2 CENTER_POINT = new Vector2(CENTER_VALUE, CENTER_VALUE);
                Vector2 oriented_point = new Vector2(col, row);
                if (flipped)
                {
                    oriented_point.X = (IMAGE_SIZE - 1) - oriented_point.X;
                }
                if (rotation != 0)
                {
                    Matrix3x2 rotation_matrix = Matrix3x2.CreateRotation(degreesToRadians(rotation), CENTER_POINT);
                    oriented_point = Vector2.Transform(oriented_point, rotation_matrix);
                }

                return image_data[(int)oriented_point.X, (int)oriented_point.Y];
            }

            void setOrientedImageData(int col, int row, char value)
            {
                int IMAGE_SIZE = image_data.GetLength(0);
                float CENTER_VALUE = (float)(IMAGE_SIZE - 1) / 2;
                Vector2 CENTER_POINT = new Vector2(CENTER_VALUE, CENTER_VALUE);
                Vector2 oriented_point = new Vector2(col, row);
                if (flipped)
                {
                    oriented_point.X = (IMAGE_SIZE - 1) - oriented_point.X;
                }
                if (rotation != 0)
                {
                    Matrix3x2 rotation_matrix = Matrix3x2.CreateRotation(degreesToRadians(rotation), CENTER_POINT);
                    oriented_point = Vector2.Transform(oriented_point, rotation_matrix);
                }

                image_data[(int)oriented_point.X, (int)oriented_point.Y] = value;
            }

            bool found = false;

            for(int row = 0; row < image_data.GetLength(1); ++row)
            {
                for (int col = 0; col < image_data.GetLength(0); ++col)
                {
                    Point image_base_point = new Point(col, row);
                    bool is_sea_monster = true;
                    for(int y = 0; is_sea_monster && y < seaMonsterPattern.Count; ++y)
                    {
                        for(int x = 0; is_sea_monster && x < seaMonsterPattern[y].Length; ++x)
                        {
                            Point sea_monster_point = new Point(x, y);
                            Point image_point = image_base_point;
                            image_point.Offset(sea_monster_point);

                            if(seaMonsterPattern[y][x] == '#' && (!boundsCheck(image_point) || getOrientedImageData(image_point.X, image_point.Y) != '#'))
                            {
                                is_sea_monster = false;
                            }
                        }
                    }
                    if(is_sea_monster)
                    {
                        found = true;
                        for (int y = 0; is_sea_monster && y < seaMonsterPattern.Count; ++y)
                        {
                            for (int x = 0; is_sea_monster && x < seaMonsterPattern[y].Length; ++x)
                            {
                                Point sea_monster_point = new Point(x, y);
                                Point image_point = image_base_point;
                                image_point.Offset(sea_monster_point);

                                if (seaMonsterPattern[y][x] == '#')
                                {
                                    setOrientedImageData(image_point.X, image_point.Y, 'O');
                                }
                            }
                        }
                    }
                }
            }
            if(found)
            {
                StringBuilder sb = new StringBuilder();
                for (int row = 0; row < image_data.GetLength(1); ++row)
                {
                    for (int col = 0; col < image_data.GetLength(0); ++col)
                    {
                        sb.Append(getOrientedImageData(col, row));
                    }
                    sb.AppendLine();
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public long getRoughness()
        {
            long count = 0;
            for (int row = 0; row < image_data.GetLength(1); ++row)
            {
                for (int col = 0; col < image_data.GetLength(0); ++col)
                {
                    if (image_data[col, row] == '#') ++count;
                }
            }
            return count;
        }

        public Image(List<Tile> tiles)
        {
            init(tiles);
            assembleTiles(tiles);
            stripBorders();
            for(int rotation = 0; rotation < 4; ++rotation)
            {
                for(int flip = 0; flip < 2; ++flip)
                {
                    hiliteSeaMonsters(rotation * 90, (flip == 1));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int row = 0; row < tile_map.GetLength(0); ++row)
            {
                for(int col = 0; col < tile_map.GetLength(1); ++col)
                {
                    sb.Append(string.Format("{0} ", tile_map[row, col].id));
                }
                sb.AppendLine();
            }

            sb.AppendLine();

            for(int row = 0; row < image_data.GetLength(1); ++row)
            {
                for (int col = 0; col < image_data.GetLength(0); ++col)
                {
                    sb.Append(image_data[col, row]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
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
                    product *= tile.id;
                }
            }

            return product;
        }

        static long part2(string file_name)
        {
            List<Tile> tiles = readInput(file_name);
            Image image = new Image(tiles);
            return image.getRoughness();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("sample_input.txt"));
            Console.WriteLine(part1("input.txt"));
            Console.WriteLine(part2("sample_input.txt"));
            Console.WriteLine(part2("input.txt"));
        }
    }
}
