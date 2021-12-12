using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day21
{
    public class Food
    {
        public HashSet<string> ingredients;
        public HashSet<string> allergens;

        public Food(string line)
        {
            Match match = Regex.Match(line, @"^(?<ingredient_list>.*)?\(contains (?<allergen_list>.*?)\)$");

            ingredients = match.Groups["ingredient_list"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            allergens = match.Groups["allergen_list"].Value.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        }
    }

    public class Program
    {
        static List<Food> readInput(string file_name)
        {
            return File.ReadAllLines(file_name).Select(l => new Food(l)).ToList();
        }

        static SortedDictionary<string, HashSet<string>> initWithIntersections(List<Food> foods)
        {
            SortedDictionary<string, HashSet<string>> allergens_to_food_source = new SortedDictionary<string, HashSet<string>>();
            HashSet<string> allergens = foods.SelectMany(f => f.allergens).ToHashSet();
            HashSet<string> ingredients = foods.SelectMany(f => f.ingredients).ToHashSet();

            foreach (var allergen in allergens)
            {
                HashSet<string> source_ingredients = new HashSet<string>(ingredients);
                List<Food> source_foods = foods.Where(f => f.allergens.Contains(allergen)).ToList();
                foreach (Food food in source_foods)
                {
                    source_ingredients.IntersectWith(food.ingredients);
                }
                allergens_to_food_source.Add(allergen, source_ingredients);
            }

            return allergens_to_food_source;
        }

        static bool reduce(SortedDictionary<string, HashSet<string>> allergens_to_food_source)
        {
            bool changed = false;

            var solved = allergens_to_food_source.Where(kvp => kvp.Value.Count == 1).Select(kvp => kvp.Value.First());
            var unsolved = allergens_to_food_source.Where(kvp => kvp.Value.Count > 1);
            for (int i = 0; i < unsolved.Count(); ++i)
            {
                var unsolved_allergen = unsolved.ElementAt(i);
                var new_source_ingredients = unsolved_allergen.Value.Except(solved);
                if (new_source_ingredients.Count() != unsolved_allergen.Value.Count)
                {
                    changed = true;
                    allergens_to_food_source[unsolved_allergen.Key] = new_source_ingredients.ToHashSet();
                }
            }

            return changed;
        }

        static void fullReduce(SortedDictionary<string, HashSet<string>> allergens_to_food_source)
        {
            while (reduce(allergens_to_food_source)) { };
        }

        static SortedDictionary<string, HashSet<string>> createAllergenToSourceIngredientMap(List<Food> foods)
        {
            SortedDictionary<string, HashSet<string>> allergens_to_food_source = initWithIntersections(foods);
            fullReduce(allergens_to_food_source);
            return allergens_to_food_source;
        }

        public static long part1(string file_name)
        {
            List<Food> foods = readInput(file_name);
            SortedDictionary<string, HashSet<string>> allergens_to_food_source = createAllergenToSourceIngredientMap(foods);

            var possible_allergen_sources = allergens_to_food_source.SelectMany(kvp => kvp.Value).ToHashSet();

            long sum = 0;

            foreach(Food food in foods)
            {
                sum += food.ingredients.Except(possible_allergen_sources).Count();
            }

            return sum;
        }

        public static string part2(string file_name)
        {
            List<Food> foods = readInput(file_name);
            SortedDictionary<string, HashSet<string>> allergens_to_food_source = createAllergenToSourceIngredientMap(foods);

            var canonical_list = allergens_to_food_source.SelectMany(kvp => kvp.Value);
            return string.Join(',', canonical_list);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("sample_input.txt"));
            Console.WriteLine(part1("input.txt"));
            Console.WriteLine(part2("sample_input.txt"));
            Console.WriteLine(part2("input.txt"));
        }
    }
}
