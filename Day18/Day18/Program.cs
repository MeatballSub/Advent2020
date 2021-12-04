using System;
using System.Collections.Generic;
using System.IO;

// 13632 + 12240 + 437 + 26 + 71 + 51 = 26457
namespace Day18
{
    class Program
    {
        static Stack<char> operators;
        static Stack<long> numbers;

        static List<string> readInput(string file_name)
        {
            List<string> expressions = new List<string>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    expressions.Add(line);
                }
            }
            return expressions;
        }

        static bool isOperator(char c)
        {
            return ((c == '+') || (c == '*'));
        }

        static void evalOp()
        {
            char op = operators.Pop();
            long num2 = numbers.Pop();
            long num1 = numbers.Pop();
            switch (op)
            {
                case '+':
                    numbers.Push(num1 + num2);
                    break;
                case '*':
                    numbers.Push(num1 * num2);
                    break;
            }
        }

        static void handleOpStack(Func<bool> condition)
        {
            while(condition())
            {
                evalOp();
            }
        }

        static int getPrecedence1(char c)
        {
            int precedence = 0;
            switch(c)
            {
                case '+':
                    precedence = 1;
                    break;
                case '*':
                    precedence = 1;
                    break;
                case '(':
                    precedence = 0;
                    break;
            }
            return precedence;
        }

        static int getPrecedence2(char c)
        {
            int precedence = 0;
            switch (c)
            {
                case '+':
                    precedence = 2;
                    break;
                case '*':
                    precedence = 1;
                    break;
                case '(':
                    precedence = 0;
                    break;
            }
            return precedence;
        }

        static long evaluate(string expression)
        {
            bool value_mode = false;
            long value = 0;
            operators = new Stack<char>();
            numbers = new Stack<long>();
            foreach(char c in expression)
            {
                if(c >= '0' && c <= '9')
                {
                    value_mode = true;
                    value *= 10;
                    value += (c - '0');
                }
                else
                {
                    if (value_mode)
                    {
                        numbers.Push(value);
                        value = 0;
                        value_mode = false;
                    }

                    if (c == '(')
                    {
                        operators.Push('(');
                    }
                    else if (c == ')')
                    {
                        handleOpStack(() => (operators.Peek() != '('));
                        operators.Pop();
                    }
                    else if (isOperator(c))
                    {
                        handleOpStack(() => (operators.Count > 0 && getPrecedence2(operators.Peek()) >= getPrecedence2(c)));
                        operators.Push(c);
                    }
                }
            }
            if (value_mode)
            {
                numbers.Push(value);
                value = 0;
                value_mode = false;
            }

            handleOpStack(() => (operators.Count > 0));
            return numbers.Pop();
        }

        static long part1(string file_name)
        {
            long result = 0;
            List<string> expressions = readInput(file_name);
            foreach(string expression in expressions)
            {
                result += evaluate(expression);
            }
            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("sample_input.txt"));
            Console.WriteLine(part1("input.txt"));
        }
    }
}
