using System;
using System.Collections.Generic;
using System.IO;

namespace Day13
{
    class Program
    {
        static (int, Int64[]) readInput(string file_name)
        {
            int earliest_departure;
            Int64[] bus_ids;
            using (TextReader reader = File.OpenText(file_name))
            {
                earliest_departure = int.Parse(reader.ReadLine());
                string bus_id_string = reader.ReadLine();
                bus_ids = Array.ConvertAll(bus_id_string.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), s => (s == "x") ? 0 : Int64.Parse(s));
            }
            return (earliest_departure, bus_ids);
        }

        static (Int64, int) findBestBus(int earliest_departure, Int64[] bus_ids)
        {
            int best_id = 0;
            int best_wait_time = int.MaxValue;

            foreach(int bus_id in bus_ids)
            {
                if (bus_id != 0)
                {
                    int wait_time = earliest_departure % bus_id;
                    if (wait_time != 0)
                    {
                        wait_time = bus_id - wait_time;
                    }
                    if (wait_time < best_wait_time)
                    {
                        best_id = bus_id;
                        best_wait_time = wait_time;
                    }
                }
            }

            return (best_id, best_wait_time);
        }

        class ChineseRemainderTheorem
        {
            public static Int64 solve(List<(Int64,Int64)> equations)
            {
                Int64 M = 1;
                foreach((Int64 congruence, Int64 modulus) in equations)
                {
                    M *= modulus;
                }
                Int64 M_k;
                Int64 x = 0;

                foreach((Int64 congruence, Int64 modulus) in equations)
                {
                    M_k = M / modulus;
                    x += congruence * getInverse(M_k, modulus) * M_k;
                }

                return x % M;
            }

            private static Int64 getInverse(Int64 congruence, Int64 modulus)
            {
                Int64 b = congruence % modulus;
                for(int x = 1; x < modulus; ++x)
                {
                    if((b * x) % modulus == 1)
                    {
                        return x;
                    }
                }
                return 1;
            }
        }

        static List<(Int64, Int64)> getEquations(Int64 [] bus_ids)
        {
            List<(Int64, Int64)> equations = new List<(Int64, Int64)>();

            for (int i=0; i < bus_ids.Length; ++i)
            {
                if(bus_ids[i] != 0)
                {
                    equations.Add((bus_ids[i]-i,bus_ids[i]));
                }
            }

            foreach((Int64 congruence, Int64 modulus) in equations)
            {
                Console.WriteLine("x = {0} mod {1}", congruence, modulus);
            }

            return equations;
        }

        static void Main(string[] args)
        {
            (int earliest_departure, Int64[] bus_ids) = readInput("input.txt");
            (Int64 best_bus_id, int best_wait_time) = findBestBus(earliest_departure, bus_ids);
            Console.WriteLine("Best Bus({0}), Wait Time({1}), Answer: {2}", best_bus_id, best_wait_time, best_bus_id * best_wait_time);
            List<(Int64, Int64)> equations = getEquations(bus_ids);
            Int64 t = ChineseRemainderTheorem.solve(equations);
            Console.WriteLine("Staggered departures start at: " + t);
        }
    }
}
