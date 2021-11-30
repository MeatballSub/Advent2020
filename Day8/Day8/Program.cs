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

    static class MockProgram
    {
        static public List<Instruction> instructions = new List<Instruction>();
    }

    class CPUState
    {
        public int accumulator;
        public int instruction_counter;
        public bool terminated;
        public bool modified;

        public CPUState()
        {
            accumulator = 0;
            instruction_counter = 0;
            terminated = false;
            modified = false;
        }

        public CPUState(CPUState state)
        {
            accumulator = state.accumulator;
            instruction_counter = state.instruction_counter;
            terminated = state.terminated;
            modified = state.modified;
        }

        void execute()
        {
            if (instruction_counter == MockProgram.instructions.Count)
            {
                terminated = true;
            }
            else
            {
                Instruction instruction = MockProgram.instructions[instruction_counter];
                //Console.WriteLine("Executing: " + instruction.ToString());
                if (instruction.type == "nop")
                {
                    ++instruction_counter;
                }
                else if (instruction.type == "acc")
                {
                    accumulator += instruction.operand;
                    ++instruction_counter;
                }
                else if (instruction.type == "jmp")
                {
                    instruction_counter += instruction.operand;
                }
            }
        }

        void toggleExecute()
        {
            if (instruction_counter == MockProgram.instructions.Count)
            {
                terminated = true;
            }
            else
            {
                Instruction instruction = MockProgram.instructions[instruction_counter];
                if (instruction.type == "nop")
                {
                    instruction_counter += instruction.operand;
                }
                else if (instruction.type == "acc")
                {
                    accumulator += instruction.operand;
                    ++instruction_counter;
                }
                else if (instruction.type == "jmp")
                {
                    ++instruction_counter;
                }
            }
        }

        public Tuple<CPUState,CPUState> nonDeterministicExecute()
        {
            CPUState noChange = new CPUState(this);
            CPUState toggle = (!modified) ? new CPUState(this) : null;
            noChange.execute();
            if (toggle != null)
            {
                toggle.modified = true;
                toggle.toggleExecute();
            }
            return new Tuple<CPUState, CPUState>(noChange, toggle);
        }
    }

    class Program
    {
        static void readInput(string file_name)
        {
            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    MockProgram.instructions.Add(new Instruction(line));
                }
            }
        }

        static void Main(string[] args)
        {
            int final_accumulator = 0;
            HashSet<int> executed_instructions = new HashSet<int>();
            readInput("input.txt");
            CPUState start = new CPUState();
            Queue<CPUState> states = new Queue<CPUState>();
            states.Enqueue(start);
            while(states.Count > 0)
            {
                CPUState state = states.Dequeue();
                Tuple<CPUState, CPUState> new_states = state.nonDeterministicExecute();
                if (new_states.Item1.terminated)
                {
                    final_accumulator = new_states.Item1.accumulator;
                    break;
                }
                else if (new_states.Item2 != null && new_states.Item2.terminated)
                {
                    final_accumulator = new_states.Item2.accumulator;
                    break;
                }
                else
                {
                    states.Enqueue(new_states.Item1);
                    if (new_states.Item2 != null)
                    {
                        states.Enqueue(new_states.Item2);
                    }
                }
            }
            Console.WriteLine("Accumulator: " + final_accumulator);
        }
    }
}
