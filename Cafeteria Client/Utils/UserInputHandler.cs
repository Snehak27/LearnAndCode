using System;

namespace CafeteriaClient.Utils
{
    public static class UserInputHandler
    {
        public static void DisplayOptions(Dictionary<int, string> options)
        {
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}) {option.Value}");
            }
        }

        public static int GetValidSelection(int maxOption, int currentSelection = -1)
        {
            int selection;
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && currentSelection != -1)
                {
                    return currentSelection;
                }
                if (int.TryParse(input, out selection) && selection > 0 && selection <= maxOption)
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a number between 1 and {maxOption}.");
                }
            }
            return selection;
        }

        public static bool GetBooleanInput(string prompt, bool currentValue = false)
        {
            bool result = currentValue;
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return result;
                }
                if (bool.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
                }
            }
            return result;
        }

        //public static void DisplayOptions(Dictionary<int, string> options)
        //{
        //    foreach (var option in options)
        //    {
        //        Console.WriteLine($"{option.Key}) {option.Value}");
        //    }
        //}

        //public static int GetValidSelection(int maxOption, int currentSelection = -1)
        //{
        //    int selection;
        //    while (true)
        //    {
        //        string input = Console.ReadLine();
        //        if (int.TryParse(input, out selection) && selection > 0 && selection <= maxOption)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Invalid input. Please enter a number between 1 and {maxOption}.");
        //        }
        //    }
        //    return selection;
        //}

        //public static bool GetBooleanInput(string prompt)
        //{
        //    bool result = false;
        //    while (true)
        //    {
        //        Console.WriteLine(prompt);
        //        string input = Console.ReadLine();
        //        if (string.IsNullOrEmpty(input) || bool.TryParse(input, out result))
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
        //        }
        //    }
        //    return result;
        //}
    }
}
