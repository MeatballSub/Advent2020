using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day8
{
    class Instruction
    {
        public string type;
        public int operand;

        public override string ToString()
        {
            StringBuilder string_value = new StringBuilder();
            string_value.Append(type + " ");
            if(operand >= 0)
            {
                string_value.Append('+');
            }
            string_value.Append(operand);
            return string_value.ToString();
        }

        public Instruction(string line)
        {
            type = "";
            operand = 0;
            Regex instruction_regex = new Regex(@"^(?<code>\S+)\s+(?<sign>[+-])(?<value>\d+)$");
            Match matches = instruction_regex.Match(line);
            if(matches.Success)
            {
                type = matches.Groups["code"].Value;
                if(matches.Groups["sign"].Value == "-")
                {
                    operand = -int.Parse(matches.Groups["value"].Value);
                }
                else
                {
                    operand = int.Parse(matches.Groups["value"].Value);
                }
            }
        }
    }
    class Program
    {
        static List<Instruction> readInput(string file_name)
        {
            List<Instruction> program = new List<Instruction>();
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    program.Add(new Instruction(line));
                }
            }
            return program;
        }

        static public int accumulator = 0;
        static public int instruction_counter = 0;

        static void execute(Instruction instruction)
        {
            //Console.WriteLine("Executing: " + instruction.ToString());
            if(instruction.type == "nop")
            {
                ++instruction_counter;
            }
            else if(instruction.type == "acc")
            {
                accumulator += instruction.operand;
                ++instruction_counter;
            }
            else if(instruction.type == "jmp")
            {
                instruction_counter += instruction.operand;
            }
        }

        static void Main(string[] args)
        {
            HashSet<int> executed_instructions = new HashSet<int>();
            List<Instruction> program = readInput("input.txt");
            while(true)
            {
                if (executed_instructions.Add(instruction_counter))
                {
                    execute(program[instruction_counter]);
                }
                else
                {
                    Console.WriteLine("Accumulator: " + accumulator);
                    break;
                }
            }
        }
    }
}
