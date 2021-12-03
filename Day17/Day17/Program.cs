using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    public static class Constants
    {
        public const bool tracing = false;
    }

    class ConwayCube
    {
        public int neigbor_count;
        public bool is_active;

        public ConwayCube(int count, bool active)
        {
            neigbor_count = count;
            is_active = active;
        }

        public ConwayCube():this(0,false)
        {
        }

        public override string ToString()
        {
            return string.Format("Is active? {0}, active neighbors({1})", is_active, neigbor_count);
        }
    }
    class PocketDimension
    {
        public Dictionary<(int, int, int), ConwayCube> state;
        public HashSet<(int, int, int)> frontier;
        public int active_count;

        private void set((int, int, int) key, bool is_active, int neighbor_count)
        {
            if (state.ContainsKey(key))
            {
                state[key].neigbor_count = neighbor_count;
                state[key].is_active = is_active;
            }
            else
            {
                state.Add(key, new ConwayCube(neighbor_count, is_active));
            }

            frontier.Add(key);
        }

        private void increment((int, int, int) key, int amount)
        {
            if (state.ContainsKey(key))
            {
                state[key].neigbor_count += amount;
            }
            else
            {
                state.Add(key, new ConwayCube(amount, false));
            }

            frontier.Add(key);
        }

        private List<(int,int,int)> getNeighborKeys(int x, int y, int z)
        {
            List<(int, int, int)> neighbor_keys = new List<(int, int, int)>();

            for (int z_offset = -1; z_offset <= 1; ++z_offset)
            {
                for (int y_offset = -1; y_offset <= 1; ++y_offset)
                {
                    for (int x_offset = -1; x_offset <= 1; ++x_offset)
                    {
                        if ((x_offset != 0) || (y_offset != 0) || (z_offset != 0))
                        {
                            neighbor_keys.Add((x + x_offset, y + y_offset, z + z_offset));
                        }
                    }
                }
            }

            return neighbor_keys;
        }

        private List<(int,int,int)> getNeighborKeys((int,int,int) key)
        {
            return getNeighborKeys(key.Item1, key.Item2, key.Item3);
        }

        private void initializeActive(int x_coord, int y_coord)
        {
            var key = (x_coord, y_coord, 0);
            int new_neighbor_count = 0;
            foreach(var neighbor_key in getNeighborKeys(x_coord, y_coord, 0))
            {
                increment(neighbor_key, +1);

                if (state[neighbor_key].is_active)
                {
                    ++new_neighbor_count;
                }
            }

            set(key, true, new_neighbor_count);

            ++active_count;
        }

        private bool evolveInactive((int,int,int) key)
        {
            return (state[key].neigbor_count == 3);
        }

        private bool evolveActive((int,int,int) key)
        {
            int neighbors = state[key].neigbor_count;
            return ((neighbors != 2) && (neighbors != 3));
        }

        private List<(int,int,int)> getNeedsUpdate()
        {
            List<(int, int, int)> needs_update = new List<(int, int, int)>();

            foreach (var potential_key in frontier)
            {
                bool changed = false;

                if (state[potential_key].is_active)
                {
                    changed = evolveActive(potential_key);
                }
                else
                {
                    changed = evolveInactive(potential_key);
                }

                if (changed)
                {
                    needs_update.Add(potential_key);
                }
            }

            return needs_update;
        }

        private void updateNeighbors((int,int,int) key, int amount)
        {
            foreach (var neighbor in getNeighborKeys(key))
            {
                increment(neighbor, amount);
            }
        }

        private void update((int,int,int) key, bool is_active, int amount)
        {
            updateNeighbors(key, amount);
            state[key].is_active = is_active;
            frontier.Add(key);
            active_count += amount;
        }

        private void kill((int,int,int) key)
        {
            update(key, false, -1);
        }

        private void birth((int,int,int) key)
        {
            update(key, true, +1);
        }

        private void update((int,int,int) key)
        {
            if(state[key].is_active)
            {
                kill(key);
            }
            else
            {
                birth(key);
            }
        }

        public void evolve()
        {
            List<(int, int, int)> needs_update = getNeedsUpdate();
            frontier.Clear();
            if (Constants.tracing)
            {
                foreach (var update_key in needs_update)
                {
                    Console.WriteLine("Updating: ({0},{1},{2}), neighbor count({3})", update_key.Item1, update_key.Item2, update_key.Item3, state[update_key].neigbor_count);
                    foreach (var neighbor in getNeighborKeys(update_key))
                    {
                        if (state.ContainsKey(neighbor) && state[neighbor].is_active)
                        {
                            Console.WriteLine("    ({0},{1},{2})", neighbor.Item1, neighbor.Item2, neighbor.Item3);
                        }
                    }
                }
            }
            foreach (var update_key in needs_update)
            {
                update(update_key);
            }
        }

        public PocketDimension(List<string> initial_state)
        {
            state = new Dictionary<(int, int, int), ConwayCube>();
            frontier = new HashSet<(int, int, int)>();
            active_count = 0;

            for(int y = 0; y < initial_state.Count; ++y)
            {
                for(int x = 0; x < initial_state[y].Length; ++x)
                {
                    if(initial_state[y][x] == '#')
                    {
                        initializeActive(x, y);
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            int min_x = state.Keys.Min(x => x.Item1);
            int max_x = state.Keys.Max(x => x.Item1);

            int min_y = state.Keys.Min(y => y.Item2);
            int max_y = state.Keys.Max(y => y.Item2);

            int min_z = state.Keys.Min(z => z.Item3);
            int max_z = state.Keys.Max(z => z.Item3);

            for(int z = min_z + 1; z < max_z; ++z)
            {
                sb.AppendLine(string.Format("z={0}", z));
                int width = max_x - min_x - 1;
                sb.AppendLine(new string(' ', width) + " 0");
                for (int y = min_y + 1 ; y < max_y; ++y)
                {
                    sb.Append(string.Format("{0,2}: ", y));
                    for(int x = min_x + 1; x < max_x; ++x)
                    {
                        char c = '.';
                        var key = (x, y, z);
                        if(state.ContainsKey(key) && state[key].is_active)
                        {
                            c = '#';
                        }
                        sb.Append(c);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

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

        static int part1(string file_name)
        {
            PocketDimension dimension = new PocketDimension(readInput(file_name));

            if (Constants.tracing)
            {
                Console.WriteLine(dimension.ToString());
            }

            for (int i = 0; i < 6; ++i)
            {
                dimension.evolve();
                if (Constants.tracing)
                {
                    Console.WriteLine("After {0} cycle{1}:", i + 1, (i > 0 ? "s" : ""));
                    Console.WriteLine();
                    Console.WriteLine(dimension.ToString());
                }
            }

            return dimension.active_count;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("input.txt"));
        }
    }
}
