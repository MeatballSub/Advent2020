using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day14
{
    class Instruction
    {
        public string type;
        public ulong address;
        public string operand;

        public Instruction(string line)
        {
            type = null;
            address = 0;
            operand = null;

            Regex instruction_regex = new Regex(@"(?:(?<type>mask) = (?<operand>[X01]{36}))|(?:(?<type>mem)\[(?<address>\d+)\] = (?<operand>\d+))");
            Match match = instruction_regex.Match(line);
            if(match.Success)
            {
                type = match.Groups["type"].Value;
                if (type == "mem")
                {
                    address = ulong.Parse(match.Groups["address"].Value);
                }
                operand = match.Groups["operand"].Value;
            }
        }
    }

    class Computer
    {
        public Dictionary<ulong, ulong> memory;
        public string mask;
        public ulong and_mask;
        public ulong or_mask;

        public Computer()
        {
            memory = new Dictionary<ulong, ulong>();
            mask = null;
            and_mask = 0;
            or_mask = 0;
        }

        public void execute(List<Instruction> instructions)
        {
            foreach(Instruction instruction in instructions)
            {
                execute(instruction);
            }
        }

        public void execute(Instruction instruction)
        {
            switch(instruction.type)
            {
                case "mask":
                    mask = instruction.operand;
                    and_mask = Convert.ToUInt64(mask.Replace('X', '1'), 2);
                    or_mask = Convert.ToUInt64(mask.Replace('X', '0'), 2);
                    break;
                case "mem":
                    memory[instruction.address] = (ulong.Parse(instruction.operand) & and_mask) | or_mask;
                    break;
            }
        }

        public void execute2(List<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                execute2(instruction);
            }
        }

        public void execute2(Instruction instruction)
        {
            switch (instruction.type)
            {
                case "mask":
                    mask = instruction.operand;
                    and_mask = Convert.ToUInt64(mask.Replace('X', '1'), 2);
                    or_mask = Convert.ToUInt64(mask.Replace('X', '0'), 2);
                    break;
                case "mem":
                    foreach (ulong address in applyMask(instruction.address))
                    {
                        memory[address] = ulong.Parse(instruction.operand);
                    }
                    break;
            }
        }

        // Don't look at me, I'm hideous
        public List<ulong> applyMask(ulong address)
        {
            List<ulong> addresses = new List<ulong>();

            string address_str = addressString(address);
            StringBuilder masked_address_str = new StringBuilder();
            for (int i = 0; i < mask.Length; ++i)
            {
                char c = mask[i];
                switch (c)
                {
                    case '0':
                        masked_address_str.Append(address_str[i]);
                        break;
                    case '1':
                        masked_address_str.Append(1);
                        break;
                    case 'X':
                        masked_address_str.Append('X');
                        break;
                }
            }

            Stack<string> address_strings = new Stack<string>();
            address_strings.Push(masked_address_str.ToString());
            while(address_strings.Count > 0)
            {
                address_str = address_strings.Pop();
                int x_index = address_str.IndexOf('X');
                if(x_index == -1)
                {
                    addresses.Add(Convert.ToUInt64(address_str, 2));
                }
                else
                {
                    masked_address_str = new StringBuilder(address_str);
                    masked_address_str[x_index] = '0';
                    address_strings.Push(masked_address_str.ToString());
                    masked_address_str[x_index] = '1';
                    address_strings.Push(masked_address_str.ToString());
                }
            }

            return addresses;
        }

        public string addressString(ulong address)
        {
            Stack<char> stack = new Stack<char>();
            for(int i = 0; i < 36; ++i)
            {
                stack.Push((address % 2).ToString()[0]);
                address >>= 1;
            }
            return new string(stack.ToArray());
        }

        public ulong getMemorySum()
        {
            ulong sum = 0;
            foreach(ulong value in memory.Values)
            {
                sum += value;
            }
            return sum;
        }
    }

    class Program
    {
        static List<Instruction> read(string file_name)
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

        static void part1(string file_name)
        {
            Computer computer = new Computer();
            var instructions = read(file_name);
            computer.execute(instructions);
            Console.WriteLine("Memory sum: " + computer.getMemorySum());
        }

        static void part2(string file_name)
        {
            Computer computer = new Computer();
            var instructions = read(file_name);
            computer.execute2(instructions);
            Console.WriteLine("Memory sum: " + computer.getMemorySum());
        }

        static void Main(string[] args)
        {
            part2("input.txt");
        }
    }
}
