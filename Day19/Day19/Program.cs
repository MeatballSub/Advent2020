using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day19
{ 
    class Program
    {
        static int version;

        static (int, string) parseRule(string line)
        {
            int index = -1;
            string rule = null;

            Regex rule_regex = new Regex(@"^(?<index>\d+):(?<rule>.*)$");
            Match match = rule_regex.Match(line);
            if (match.Success)
            {
                index = int.Parse(match.Groups["index"].Value);
                rule = match.Groups["rule"].Value;
            }

            return (index, rule);
        }

        static (Dictionary<int, string>, List<string>) readInput(string file_name)
        {
            Dictionary<int, string> rules = new Dictionary<int, string>();
            List<string> messages = new List<string>();

            bool parse_rules = true;

            using(TextReader reader = File.OpenText(file_name))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    // rules
                    if(parse_rules && line != string.Empty)
                    {
                        (int index, string rule) = parseRule(line);
                        rules.Add(index,rule);
                    }
                    // blank line
                    else if(line == string.Empty)
                    {
                        parse_rules = false;
                    }
                    // messages
                    else
                    {
                        messages.Add(line);
                    }
                }
            }

            return (rules, messages);
        }

        static Dictionary<int, string> getTerminalRules(Dictionary<int, string> rules)
        {
            return rules.Select(kvp => (kvp.Key, Regex.Match(kvp.Value, @"^\s+""(?<terminal>.)""$")))
                .Where(kvp => kvp.Item2.Success)
                .Select(kvp => (kvp.Key, kvp.Item2.Groups["terminal"].Value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        static HashSet<int> getSubRules(string rule)
        {
            Match match = Regex.Match(rule, @"^(((\s+(?<sub_rules>\d+))+)|(((\s+(?<sub_rules>\d+))+)\s+\|((\s+(?<sub_rules>\d+))+)))$");
            return match.Groups["sub_rules"].Captures.Select(c => int.Parse(c.Value)).ToHashSet();
        }

        static bool allSubRulesExpanded(string rule, Dictionary<int, string> expanded_rules)
        {
            HashSet<int> sub_rules = getSubRules(rule);
            int count = sub_rules.Count;
            sub_rules.IntersectWith(expanded_rules.Keys);
            return (count == sub_rules.Count);
        }

        static string replaceSimpleSubRule(CaptureCollection captures, Dictionary<int, string> expanded_rules)
        {
            StringBuilder expanded = new StringBuilder();

            foreach (Capture capture in captures)
            {
                expanded.Append(expanded_rules[int.Parse(capture.Value)]);
            }

            return expanded.ToString();
        }

        static string replaceComplexSubrule(CaptureCollection captures1, CaptureCollection captures2, Dictionary<int, string> expanded_rules)
        {
            StringBuilder expanded = new StringBuilder();

            expanded.Append(replaceSimpleSubRule(captures1, expanded_rules));
            expanded.Append("|");
            expanded.Append(replaceSimpleSubRule(captures2, expanded_rules));

            return expanded.ToString();
        }

        static string replaceSubRules(string rule, Dictionary<int, string> expanded_rules)
        {
            StringBuilder expanded = new StringBuilder();

            Match simple_match = Regex.Match(rule, @"^(\s+(?<sub_rules>\d+))+$");
            Match complex_match = Regex.Match(rule, @"^(\s+(?<expansion1>\d+))+\s+\|(\s+(?<expansion2>\d+))+$");

            expanded.Append("(");

            if (simple_match.Success)
            {
                expanded.Append(replaceSimpleSubRule(simple_match.Groups["sub_rules"].Captures, expanded_rules));
            }
            else if(complex_match.Success)
            {
                expanded.Append(replaceComplexSubrule(complex_match.Groups["expansion1"].Captures, complex_match.Groups["expansion2"].Captures, expanded_rules));
            }

            expanded.Append(")");

            return expanded.ToString();
        }

        static Dictionary<int, string> tryAddExpandRule(int index, string rule, Dictionary<int,string> expanded_rules)
        {
            if (allSubRulesExpanded(rule, expanded_rules))
            {
                if (version == 2 && index == 11)
                {
                    expanded_rules.Add(index, "((?<rule11_42s>" + expanded_rules[42] + ")+)((?<rule11_31s>" + expanded_rules[31] + ")+)");
                }
                else
                {
                    expanded_rules.Add(index, replaceSubRules(rule, expanded_rules));
                }
            }

            return expanded_rules;
        }

        static Dictionary<int, string> getExpandedRules(Dictionary<int, string> rules, Dictionary<int, string> expanded_rules)
        {
            foreach ((int index, string rule) in rules)
            {
                expanded_rules = tryAddExpandRule(index, rule, expanded_rules);
            }
            return expanded_rules;
        }

        static Dictionary<int, string> expand1pass(Dictionary<int, string> unexpanded_rules, Dictionary<int, string> expanded_rules)
        {
            return (expanded_rules == null) ? getTerminalRules(unexpanded_rules) : getExpandedRules(unexpanded_rules, expanded_rules);
        }

        static Dictionary<int, string> removeExpandedRules(Dictionary<int, string> unexpanded_rules, Dictionary<int, string> expanded_rules)
        {
            return unexpanded_rules.Where(kvp => !expanded_rules.ContainsKey(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        static string buildRegexString(Dictionary<int,string> unexpanded_rules)
        {
            Dictionary<int, string> expanded_rules = null;

            while(unexpanded_rules.Count > 0)
            {
                expanded_rules = expand1pass(unexpanded_rules, expanded_rules);
                unexpanded_rules = removeExpandedRules(unexpanded_rules, expanded_rules);
            }

            return "^" + expanded_rules[0] + "$";
        }

        static int part1(string file_name)
        {
            version = 1;
            (Dictionary<int, string> rules, List<string> messages) = readInput(file_name);
            string regex_string = buildRegexString(rules);
            return messages.Where(s => Regex.Match(s, regex_string).Success).Count();
        }

        static int part2(string file_name)
        {
            version = 2;
            (Dictionary<int, string> rules, List<string> messages) = readInput(file_name);
            string regex_string = buildRegexString(rules);

            return messages.Select(s => Regex.Match(s, regex_string))
                .Where(m => m.Success && m.Groups["rule11_42s"].Captures.Count >= m.Groups["rule11_31s"].Captures.Count).Count();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("input.txt"));
            Console.WriteLine(part2("input.txt"));
        }
    }
}
