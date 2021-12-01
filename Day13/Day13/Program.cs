using System;
using System.IO;

namespace Day13
{
    class Program
    {
        static (int, int[]) readInput(string file_name)
        {
            int earliest_departure;
            int[] bus_ids;
            using (TextReader reader = File.OpenText(file_name))
            {
                earliest_departure = int.Parse(reader.ReadLine());
                string bus_id_string = reader.ReadLine();
                bus_ids = Array.ConvertAll(bus_id_string.Split(new char[] { ',', 'x' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            }
            return (earliest_departure, bus_ids);
        }

        static (int, int) findBestBus(int earliest_departure, int [] bus_ids)
        {
            int best_id = 0;
            int best_wait_time = int.MaxValue;

            foreach(int bus_id in bus_ids)
            {
                int wait_time = earliest_departure % bus_id;
                if(wait_time != 0)
                {
                    wait_time = bus_id - wait_time;
                }
                if(wait_time < best_wait_time)
                {
                    best_id = bus_id;
                    best_wait_time = wait_time;
                }
            }

            return (best_id, best_wait_time);
        }
        static void Main(string[] args)
        {
            (int earliest_departure, int [] bus_ids) = readInput("input.txt");
            (int best_bus_id, int best_wait_time) = findBestBus(earliest_departure, bus_ids);
            Console.WriteLine("Best Bus({0}), Wait Time({1}), Answer: {2}", best_bus_id, best_wait_time, best_bus_id * best_wait_time);
        }
    }
}
