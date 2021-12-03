using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Day16
{
    class Field
    {
        public string name;
        public int range1_start;
        public int range1_end;
        public int range2_start;
        public int range2_end;

        public Field(string line)
        {
            name = null;
            range1_start = 0;
            range1_end = 0;
            range2_start = 0;
            range2_end = 0;

            Regex field_regex = new Regex(@"^(?<name>.*?): (?<range1_start>\d+)-(?<range1_end>\d+) or (?<range2_start>\d+)-(?<range2_end>\d+)$");
            Match match = field_regex.Match(line);
            if(match.Success)
            {
                name = match.Groups["name"].Value;
                range1_start = int.Parse(match.Groups["range1_start"].Value);
                range1_end = int.Parse(match.Groups["range1_end"].Value);
                range2_start = int.Parse(match.Groups["range2_start"].Value);
                range2_end = int.Parse(match.Groups["range2_end"].Value);
            }
        }
    }

    class Ticket
    {
        public List<int> field_values;

        public Ticket(string line)
        {
            field_values = new List<int>();

            var values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach(string value in values)
            {
                field_values.Add(int.Parse(value));
            }
        }
    }

    class ParsedInput
    {
        public List<Field> fields;
        public Ticket my_ticket;
        public List<Ticket> nearby_tickets;

        private enum ParseMode
        {
            Fields,
            MyTicket,
            NearbyTickets,
        }

        public ParsedInput(string file_name)
        {
            ParseMode mode = ParseMode.Fields;
            fields = new List<Field>();
            nearby_tickets = new List<Ticket>();

            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if(line == "your ticket:")
                    {
                        mode = ParseMode.MyTicket;
                    }
                    else if(line == "nearby tickets:")
                    {
                        mode = ParseMode.NearbyTickets;
                    }
                    else if (line == String.Empty)
                    {

                    }
                    else if(mode == ParseMode.Fields)
                    {
                        fields.Add(new Field(line));
                    }
                    else if(mode == ParseMode.MyTicket)
                    {
                        my_ticket = new Ticket(line);
                    }
                    else
                    {
                        nearby_tickets.Add(new Ticket(line));
                    }
                }
            }
        }
    }

    class Program
    {
        static HashSet<int> getValidFieldValues(ParsedInput input)
        {
            HashSet<int> valid_values = new HashSet<int>();

            foreach(Field field in input.fields)
            {
                for(int value = field.range1_start; value <= field.range1_end; ++value)
                {
                    valid_values.Add(value);
                }

                for (int value = field.range2_start; value <= field.range2_end; ++value)
                {
                    valid_values.Add(value);
                }
            }

            return valid_values;
        }

        static int getInvalidSum(ParsedInput input)
        {
            int invalid_sum = 0;
            HashSet<int> valid_values = getValidFieldValues(input);

            foreach(Ticket ticket in input.nearby_tickets)
            {
                foreach(int value in ticket.field_values)
                {
                    if(!valid_values.Contains(value))
                    {
                        invalid_sum += value;
                    }
                }
            }

            return invalid_sum;
        }

        static bool validate(Field field, int value)
        {
            bool range1_valid = ((value >= field.range1_start) && (value <= field.range1_end));
            bool range2_valid = ((value >= field.range2_start) && (value <= field.range2_end));

            return (range1_valid || range2_valid);
        }

        static List<Ticket> onlyValid(List<Ticket> tickets, ParsedInput input)
        {
            List<Ticket> valid_tickets = new List<Ticket>();
            HashSet<int> valid_values = getValidFieldValues(input);

            foreach (Ticket ticket in input.nearby_tickets)
            {
                bool include = true;
                foreach (int value in ticket.field_values)
                {
                    if (!valid_values.Contains(value))
                    {
                        include = false;
                        break;
                    }
                }
                if(include)
                {
                    valid_tickets.Add(ticket);
                }
            }

            return valid_tickets;
        }

        static bool isUnique(List<HashSet<Field>> possible_fields)
        {
            int counts = 0;
            foreach(HashSet<Field> hash_set in possible_fields)
            {
                counts += hash_set.Count;
            }
            return (counts == possible_fields.Count);
        }

        static List<Field> makeUnique(List<HashSet<Field>> possible_fields)
        {
            List<Field> unique = new List<Field>();

            while(!isUnique(possible_fields))
            {
                for(int i = 0; i < possible_fields.Count; ++i)
                {
                    if(possible_fields[i].Count == 1)
                    {
                        foreach(Field field in possible_fields[i])
                        {
                            for(int j = 0; j < possible_fields.Count; ++j)
                            {
                                if(i != j)
                                {
                                    possible_fields[j].Remove(field);
                                }
                            }
                        }
                    }
                }
            }

            foreach(HashSet<Field> hash_set in possible_fields)
            {
                foreach(Field field in hash_set)
                {
                    unique.Add(field);
                }
            }

            return unique;
        }

        static List<string> decodeFields(ParsedInput input)
        {
            List<string> decoded_fields = new List<string>();
            List<HashSet<Field>> possible_fields = new List<HashSet<Field>>();

            foreach(int value in input.my_ticket.field_values)
            {
                HashSet<Field> valid_fields = new HashSet<Field>();
                foreach(Field field in input.fields)
                {
                    if(validate(field, value))
                    {
                        valid_fields.Add(field);
                    }
                }
                possible_fields.Add(valid_fields);
            }

            foreach(Ticket ticket in onlyValid(input.nearby_tickets, input))
            {
                for (int i = 0; i < ticket.field_values.Count; ++i)
                {
                    List<Field> field_to_remove = new List<Field>();
                    foreach (Field field in possible_fields[i])
                    {
                        if (!validate(field, ticket.field_values[i]))
                        {
                            field_to_remove.Add(field);
                        }
                    }
                    foreach(Field field in field_to_remove)
                    {
                        possible_fields[i].Remove(field);
                    }
                }
            }

            var ordered_fields = makeUnique(possible_fields);

            foreach(Field field in ordered_fields)
            {
                decoded_fields.Add(field.name);
            }

            return decoded_fields;
        }

        static Dictionary<string, int> decodeMyTicket(ParsedInput input)
        {
            Dictionary<string, int> my_decoded_ticket = new Dictionary<string, int>();
            List<string> decoded_fields = decodeFields(input);
            for(int i = 0; i < decoded_fields.Count; ++i)
            {
                my_decoded_ticket.Add(decoded_fields[i], input.my_ticket.field_values[i]);
            }
            foreach ((string name, int value) in my_decoded_ticket)
            {
                Console.WriteLine("{0}({1})", name, value);
            }
            return my_decoded_ticket;
        }

        static Int64 getDepartureProduct(ParsedInput input)
        {
            Int64 product = 1;

            Dictionary<string, int> my_decoded_ticket = decodeMyTicket(input);

            foreach((string field_name, int value) in my_decoded_ticket)
            {
                if(field_name.StartsWith("departure"))
                {
                    product *= value;
                }
            }

            return product;
        }

        static int part1(string file_name)
        {
            ParsedInput input = new ParsedInput(file_name);
            return getInvalidSum(input);
        }

        static Int64 part2(string file_name)
        {
            ParsedInput input = new ParsedInput(file_name);
            return getDepartureProduct(input);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Part 1, sample_input: " + part1("sample_input.txt"));
            Console.WriteLine("Part 1, input: " + part1("input.txt"));
            Console.WriteLine("Part 2, sample_input: " + part2("sample_input2.txt"));
            Console.WriteLine("Part 2, input: " + part2("input.txt"));
        }
    }
}
