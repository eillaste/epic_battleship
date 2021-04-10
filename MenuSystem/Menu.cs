using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    // in C# enum is just integer
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus
    }
    
    public enum Color
    {
        Red,
        Green,
        Blue,
        Yellow,
        Magenta,
    }

    public class Menu
    {
        public string defaultChoice = "n";
        public Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();
        
        private readonly MenuLevel _menuLevel;
        
        // bad, needs to be customizable by user not predefined
        //private readonly string[] _reservedActions = new[] {"x", "m", "r"};
        List<String> reservedKeys = new List<string>();
        List<String> reservedValues = new List<string>();
        
        public Menu(MenuLevel level, Dictionary<string, string>reservedItems)
        {
            _menuLevel = level;
            reservedKeys = reservedItems.Keys.ToList();
            reservedValues = reservedItems.Values.ToList();
        }

        public void AddMenuItem(MenuItem item)
        {

            if (item.UserChoice == "")
            {
                throw new ArgumentException("UserChoice cannot be empty");
            } 
            if (reservedKeys.Contains(item.UserChoice))
            {
                throw new ArgumentException("This UserChoice is already taken");
            }
            MenuItems.Add(item.UserChoice, item);
        }
        

        public string RunMenu()
        {
            //Console.Clear();
            var userChoice = "";
            do
            {

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine($"{reservedKeys[2]}) {reservedValues[2]}");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine($"{reservedKeys[1]}) {reservedValues[1]}");
                        Console.WriteLine($"{reservedKeys[2]}) {reservedValues[2]}");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine($"{reservedKeys[0]}) {reservedValues[0]}");
                        Console.WriteLine($"{reservedKeys[1]}) {reservedValues[1]}");
                        Console.WriteLine($"{reservedKeys[2]}) {reservedValues[2]}");
                        break;
                    default:
                        throw new Exception("Unknown menu depth!");
                }
                //Console.WriteLine(_menuLevel);
                
                Console.WriteLine("--------------------------------------------------");
                
                
                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";
                
                if (userChoice != "d" && userChoice != "p")
                {
                    Console.Clear();
                }
                if (userChoice.Length <= 0)
                {
                    userChoice = defaultChoice;
                }

                // is it a reserved keyword
                if (!reservedKeys.Contains(userChoice))
                {
                    // Returns boolean, was it found or not and if true also usermenuitem
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        string capitalizedLabel = char.ToUpper(userMenuItem.Label[0]) + userMenuItem.Label.Substring(1);
                        if (Enum.IsDefined(typeof(Color), capitalizedLabel))
                        {
                            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), capitalizedLabel);
                        }
                        // this gets coordinates
                        userChoice = userMenuItem.MethodToExecute();
                        
                    } 
                    
                    else
                    {
                        Console.WriteLine("I don't have this option!");
                    }
                }

                if (userChoice == reservedKeys[2])
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.WriteLine("Closing down!");
                    }
                    break;
                }
                
                if (_menuLevel != MenuLevel.Level0 && userChoice == reservedKeys[1])
                {
                    break;
                }
                if (_menuLevel == MenuLevel.Level2Plus && userChoice == reservedKeys[0])
                {
                    // Console.WriteLine(_menuLevel);
                    break;
                }

            } while (true);
            return userChoice == reservedKeys[0] ? "" : userChoice;
        }
    }
}
