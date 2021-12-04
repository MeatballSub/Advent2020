using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    using KeyType = System.ValueTuple<int, int, int, int>;

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
    }
    class PocketDimension
    {
        public Dictionary<KeyType, ConwayCube> state;
        public HashSet<KeyType> frontier;
        public int active_count;

        private void set(KeyType key, bool is_active, int neighbor_count)
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

        private void increment(KeyType key, int amount)
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

        private List<KeyType> getNeighborKeys(int x, int y, int z, int w)
        {
            List<KeyType> neighbor_keys = new List<KeyType>();

            for (int w_offset = -1; w_offset <= 1; ++w_offset)
            {
                for (int z_offset = -1; z_offset <= 1; ++z_offset)
                {
                    for (int y_offset = -1; y_offset <= 1; ++y_offset)
                    {
                        for (int x_offset = -1; x_offset <= 1; ++x_offset)
                        {
                            if ((x_offset != 0) || (y_offset != 0) || (z_offset != 0) || (w_offset != 0))
                            {
                                neighbor_keys.Add((x + x_offset, y + y_offset, z + z_offset, w + w_offset));
                            }
                        }
                    }
                }
            }

            return neighbor_keys;
        }

        private List<KeyType> getNeighborKeys(KeyType key)
        {
            return getNeighborKeys(key.Item1, key.Item2, key.Item3, key.Item4);
        }

        private void initializeActive(int x_coord, int y_coord)
        {
            var key = (x_coord, y_coord, 0, 0);
            int new_neighbor_count = 0;
            foreach(var neighbor_key in getNeighborKeys(x_coord, y_coord, 0, 0))
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

        private bool evolveInactive(KeyType key)
        {
            return (state[key].neigbor_count == 3);
        }

        private bool evolveActive(KeyType key)
        {
            int neighbors = state[key].neigbor_count;
            return ((neighbors != 2) && (neighbors != 3));
        }

        private List<KeyType> getNeedsUpdate()
        {
            List<KeyType> needs_update = new List<KeyType>();

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

        private void updateNeighbors(KeyType key, int amount)
        {
            foreach (var neighbor in getNeighborKeys(key))
            {
                increment(neighbor, amount);
            }
        }

        private void update(KeyType key, bool is_active, int amount)
        {
            updateNeighbors(key, amount);
            state[key].is_active = is_active;
            frontier.Add(key);
            active_count += amount;
        }

        private void kill(KeyType key)
        {
            update(key, false, -1);
        }

        private void birth(KeyType key)
        {
            update(key, true, +1);
        }

        private void update(KeyType key)
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
            List<KeyType> needs_update = getNeedsUpdate();
            frontier.Clear();
            if (Constants.tracing)
            {
                foreach (var update_key in needs_update)
                {
                    Console.WriteLine("Updating: ({0}), neighbor count({1})", update_key.ToString(), state[update_key].neigbor_count);
                    foreach (var neighbor in getNeighborKeys(update_key))
                    {
                        if (state.ContainsKey(neighbor) && state[neighbor].is_active)
                        {
                            Console.WriteLine("    ({0})", neighbor.ToString());
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
            state = new Dictionary<KeyType, ConwayCube>();
            frontier = new HashSet<KeyType>();
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

            int min_w = state.Keys.Min(w => w.Item3);
            int max_w = state.Keys.Max(w => w.Item3);

            for(int w = min_w + 1; w < max_w; ++w)
            {
                for (int z = min_z + 1; z < max_z; ++z)
                {
                    sb.AppendLine(string.Format("z={0}, w={1}", z, w));
                    int width = max_x - min_x - 1;
                    sb.AppendLine(new string(' ', width) + " 0");
                    for (int y = min_y + 1; y < max_y; ++y)
                    {
                        sb.Append(string.Format("{0,2}: ", y));
                        for (int x = min_x + 1; x < max_x; ++x)
                        {
                            char c = '.';
                            var key = (x, y, z, w);
                            if (state.ContainsKey(key) && state[key].is_active)
                            {
                                c = '#';
                            }
                            sb.Append(c);
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
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

        static int part2(string file_name)
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
            Console.WriteLine(part2("input.txt"));
        }
    }
}
