using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Day11
{
    class GameOfLife
    {
        public char[][] map;

        public GameOfLife(char[][] m)
        {
            map = m;
        }

        public bool boundsCheck(int x, int y)
        {
            return (y >= 0 && y < map.GetLength(0) && x >= 0 && x < map[y].GetLength(0));
        }

        public bool isOccupied(int x, int y)
        {
            return (boundsCheck(x, y) && (map[y][x] == '#'));
        }

        public bool isFloor(int x, int y)
        {
            return (boundsCheck(x, y) && (map[y][x] == '.'));
        }

        public int getNeighborCount(int seat_x, int seat_y)
        {
            int neighbors = 0;
            for (int y = seat_y - 1; y < seat_y + 2; ++y)
            {
                for (int x = seat_x - 1; x < seat_x + 2; ++x)
                {
                    if (x != seat_x || y != seat_y)
                    {
                        if (isOccupied(x, y))
                        {
                            ++neighbors;
                        }
                    }
                }
            }
            return neighbors;
        }

        public bool isOccupied(int seat_x, int seat_y, int dir_x, int dir_y)
        {
            bool occupied = false;
            int step_count = 1;
            int step_x = seat_x + (step_count * dir_x);
            int step_y = seat_y + (step_count * dir_y);
            while(boundsCheck(step_x,step_y))
            {
                if(!isFloor(step_x, step_y))
                {
                    occupied = isOccupied(step_x, step_y);
                    break;
                }
                ++step_count;
                step_x = seat_x + (step_count * dir_x);
                step_y = seat_y + (step_count * dir_y);
            }
            return occupied;
        }

        public int getVisibleCount(int seat_x, int seat_y)
        {
            int neighbors = 0;
            for (int y = -1; y < 2; ++y)
            {
                for (int x = -1; x < 2; ++x)
                {
                    if (x != 0 || y != 0)
                    {
                        if (isOccupied(seat_x, seat_y, x, y))
                        {
                            ++neighbors;
                        }
                    }
                }
            }
            return neighbors;
        }

        public bool evolve1()
        {
            bool changed = false;
            char[][] new_map = map.Select(item => item.ToArray()).ToArray();

            for (int y = 0; y < map.GetLength(0); ++y)
            {
                for (int x = 0; x < map[y].GetLength(0); ++x)
                {
                    if (!isFloor(x, y))
                    {
                        int neighbors = getNeighborCount(x, y);
                        if (isOccupied(x, y))
                        {
                            if (neighbors >= 4)
                            {
                                new_map[y][x] = 'L';
                                changed = true;
                            }
                        }
                        else
                        {
                            if (neighbors == 0)
                            {
                                new_map[y][x] = '#';
                                changed = true;
                            }
                        }
                    }
                }
            }
            map = new_map;
            return changed;
        }

        public bool evolve2()
        {
            bool changed = false;
            char[][] new_map = map.Select(item => item.ToArray()).ToArray();

            for (int y = 0; y < map.GetLength(0); ++y)
            {
                for (int x = 0; x < map[y].GetLength(0); ++x)
                {
                    if (!isFloor(x, y))
                    {
                        int visible = getVisibleCount(x, y);
                        if (isOccupied(x, y))
                        {
                            if (visible >= 5)
                            {
                                new_map[y][x] = 'L';
                                changed = true;
                            }
                        }
                        else
                        {
                            if (visible == 0)
                            {
                                new_map[y][x] = '#';
                                changed = true;
                            }
                        }
                    }
                }
            }
            map = new_map;
            return changed;
        }

        public override string ToString()
        {
            StringBuilder string_value = new StringBuilder();
            for (int y = 0; y < map.GetLength(0); ++y)
            {
                string_value.AppendLine(new string(map[y]));
            }
            string_value.AppendLine();
            return string_value.ToString();
        }

        public int getOccupiedCount()
        {
            int count = 0;
            for (int y = 0; y < map.GetLength(0); ++y)
            {
                for (int x = 0; x < map[y].GetLength(0); ++x)
                {
                    if (isOccupied(x, y))
                    {
                        ++count;
                    }
                }
            }
            return count;
        }
    }
    class Program
    {
        static char [][] readInput(string file_name)
        {
            List<string> string_input = new List<string>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    string_input.Add(line);
                }
            }
            return string_input.Select(item => item.ToArray()).ToArray();
        }

        static void Main(string[] args)
        {
            char[][] map = readInput("input.txt");
            GameOfLife game_of_life = new GameOfLife(map);

            //Console.WriteLine(game_of_life.ToString());
            while(game_of_life.evolve2())
            {
                //Console.WriteLine(game_of_life.ToString());
            }

            Console.WriteLine("Occupied Seats: " + game_of_life.getOccupiedCount());
        }
    }
}
