using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DAL;
using Domain;
using GameBrain;
using GameConsoleUI;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    public class Program
    {
        
        private static BattleShip _theGame = null!;
        private static readonly Dictionary<string, string> ReservedItems = new Dictionary<string, string>();


        private static readonly char[] Letters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V'
        };

        private static int _shipCount = 1;
        private static int _cellsLeft;
        private static int _numberOfShipsPerPlayer;
        private static Game _game = null!;

        private static void Main()
        {
            BattleShipConsoleUi.Greet();

            ReservedItems.Add("r", "Return to previous");
            ReservedItems.Add("m", "Return to main");
            ReservedItems.Add("x", "Exit");

            var menuNewGame = new Menu(MenuLevel.Level1, ReservedItems);
            menuNewGame.AddMenuItem(new MenuItem("Human vs human", "1", () => { BattleShip();
                return "m";
            }));
            menuNewGame.AddMenuItem(new MenuItem("Human vs AI", "2", () => { BattleShip("AI");
                return "m";
            }));
            //menuNewGame.AddMenuItem(new MenuItem("AI vs AI", "3", DefaultMenuAction));

            var menuColor = new Menu(MenuLevel.Level1, ReservedItems);
            menuColor.AddMenuItem(new MenuItem("red", "1", ColorOptionsAction));
            menuColor.AddMenuItem(new MenuItem("green", "2", ColorOptionsAction));
            menuColor.AddMenuItem(new MenuItem("blue", "3", ColorOptionsAction));
            menuColor.AddMenuItem(new MenuItem("yellow", "4", ColorOptionsAction));
            menuColor.AddMenuItem(new MenuItem("magenta", "5", ColorOptionsAction));

            var mainMenu = new Menu(MenuLevel.Level0, ReservedItems);
            mainMenu.AddMenuItem(new MenuItem("New Game", "n", menuNewGame.RunMenu));
            mainMenu.AddMenuItem(new MenuItem("Load Game", "l", () =>
            {
                // create dummy game
                var game = new BattleShip(5, 5, "Player 1", "Player 2");
                _theGame = game;
                
                if (mainMenu.MenuItems.ContainsKey("d"))
                {
                    mainMenu.MenuItems.Remove("d");
                    mainMenu.AddMenuItem(new MenuItem($"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!", "d", () => BombAction(game, mainMenu)));
                }
                else
                {
                    mainMenu.AddMenuItem(new MenuItem($"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!", "d", () => BombAction(game, mainMenu)));
                }
                return LoadGameAction(game, mainMenu);
            }));
            mainMenu.AddMenuItem(new MenuItem("Color Options", "c", menuColor.RunMenu));
            mainMenu.RunMenu();
        }

        // MAIN FUNCTION ENDS HERE//f

        static string BattleShip(string optionalstr = "")
        {
            //I PRIMARY SETTINGS, NAME1, NAME2, DIMENSIONS, TOUCHING OPTIONS
            var player1Name = ValidateName("Please specify player name for player 1:", 1);
            string player2Name;
            if (optionalstr == "AI")
            {
                player2Name = optionalstr;
            }
            else
            {
                player2Name = ValidateName("Please specify player name for player 2:", 2);
            }
            
            var dimensions = ValidateBoardDimensions("Please specify board dimensions X (1-20), Y (1-20):");
            var x = dimensions.x;
            var y = dimensions.y;
            var touchingOptionInt =
                ValidateTouchingOption(
                    "Please specify touching option: 1 - can touch, 2 - cant touch, 3 - corners can touch");
            var touchingOptionForGame = (ETouchingRule) touchingOptionInt;
            Console.WriteLine(
                $"Starting new game between {player1Name} & {player2Name}, on a {x}x{y} sized board, with touching option {touchingOptionForGame}");

            //II BOAT COUNT SETTINGS
            Console.Clear();
            Console.WriteLine("Please place your boats:");
            var game = new BattleShip(x, y, player1Name, player2Name) {TouchingRule = touchingOptionForGame};
            BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
            SetShipCount(game);
            game.ShipCountPerPlayer = _numberOfShipsPerPlayer;


            // III BOAT PLACEMENT LOOPS
            //Console.WriteLine($"NEXT MOVE BY X? {game.NextMoveByX}");
            // THIS NEXT LINE IS FOR WHATEVER REASON REALLLY REALLLY IMPORTANT...
            game._nextMoveByPlayer2 = true;
            PlaceShipsLoop(game, true);
            Console.Clear();
            game._nextMoveByPlayer2 = !game._nextMoveByPlayer2;
            BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
            // TODO PlaceShipsLoop for AI
            // false means player2turn
            PlaceShipsLoop(game, false);
            
            Console.Clear();
            game._nextMoveByPlayer2 = !game._nextMoveByPlayer2;
            BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);

            //Console.WriteLine($"NEXT MOVE BY X? {game.NextMoveByPlayer2}");
            Console.WriteLine("ALL SHIPS PLACED BY PLAYERS!");
            Console.WriteLine();
            Console.WriteLine("START BOMBING!");
            Console.WriteLine();
            // create DOMAINGAME
            _game = new Game()
            {
                CreationDT = DateTime.Now,
                PlayerA = game.Player1Name,
                PlayerB = game.Player2Name
            };
            /*using var db = new ApplicationDbContext();
            Game.Id = db.Games.OrderBy(g => g.CreationDT).Last().Id + 1;
            Console.WriteLine($"WHAT IS THE GAME ID?? {Game.Id}");*/
            
            var gameLoopMenu = new Menu(MenuLevel.Level1, ReservedItems);
            if (gameLoopMenu.MenuItems.ContainsKey("d"))
            {
                gameLoopMenu.MenuItems.Remove("d");
                gameLoopMenu.AddMenuItem(new MenuItem($"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!", "d", () => BombAction(game, gameLoopMenu)));
            }
            else
            {
                gameLoopMenu.AddMenuItem(new MenuItem($"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!", "d", () => BombAction(game, gameLoopMenu)));
            }

            gameLoopMenu.AddMenuItem(new MenuItem(
                $"Save game",
                userChoice: "s",
                () => SaveGameAction(game))
            );

            gameLoopMenu.AddMenuItem(new MenuItem(
                $"Load game",
                userChoice: "l",
                () => LoadGameAction(game, gameLoopMenu))
            );
            
            var userChoice = gameLoopMenu.RunMenu();

            return userChoice;
        }
//END OF BATTLESHIP() FUNCTION

        static void PlaceShipsLoop(BattleShip game, bool player1Turn)
        {
            
            ICollection<Ship> pShips = player1Turn ? game.Player1Ships : game.Player2Ships;
            ICollection<ECellState> gShips = player1Turn ? game.Ships1 : game.Ships2;
            ECellState[,] board = player1Turn ? game._board2 : game._board1;
            while (pShips.Count < game.ShipCountPerPlayer)
            {
                if (!game.NextMoveByPlayer2 && game.Player2Name == "AI")
                {
                    PlaceShipsAI(game, gShips, board);
                }
                else
                {
                   Console.Clear();
                   BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
                   Console.WriteLine("Place boat, enter start and end square i.e a1-a5 to place boat with length 5");
                   Console.WriteLine();
                   Console.WriteLine(
                       $"Now placing {gShips.ToArray()[0]}, it requires {(int) gShips.ToArray()[0]} Cells");
                   PlaceShips(game, gShips, board, Console.ReadLine() ?? "", true);
                   pShips = player1Turn ? game.Player1Ships : game.Player2Ships;
                   gShips = player1Turn ? game.Ships1 : game.Ships2;
                   board = player1Turn ? game._board2 : game._board1; 
                }
            }
            //Thread.Sleep(800);
            
        }

        private static void PlaceShipsAI(BattleShip game, ICollection<ECellState> ships, ECellState[,] board)
        {
            int shiplength = (int) ships.ToArray()[0] - 1;
                Random r = new Random();
                int randomStartRow = r.Next(0, game._y);
                int randomStartCol = r.Next(0, game._x);
                // min 0 0 max 19 19
                bool endRowHigher = true;
                bool endRowLower = true;
                bool endColLeft = true;
                bool endColRight = true;

                int end_x1;
                int end_y2;
                int end_x2;
                int end_y3;
                int end_x3;
                int end_y4;
                int end_x4;
                
                List<(int, int)> endpoints = new List<(int, int)>();
                if (randomStartRow - shiplength < 0)
                {
                    endRowHigher = false;
                }
                if (randomStartRow + shiplength >= game._y)
                {
                    endRowLower = false;
                }
                if (randomStartCol - shiplength < 0)
                {
                    endColLeft = false;
                }
                if (randomStartCol + shiplength >= game._x)
                {
                    endColRight = false;
                }

                if (endRowHigher)
                {
                    var end_y1 = randomStartRow - shiplength;
                    end_x1 = randomStartCol;
                    if (endpoints != null) endpoints.Add((end_y1, end_x1));
                }

                if (endRowLower)
                {
                    end_y2 = randomStartRow + shiplength;
                    end_x2 = randomStartCol;
                    if (endpoints != null) endpoints.Add((end_y2, end_x2));
                }

                if (endColLeft)
                {
                    end_y3 = randomStartRow;
                    end_x3 = randomStartCol - shiplength;
                    if (endpoints != null) endpoints.Add((end_y3, end_x3));
                }

                if (endColRight)
                {
                    end_y4 = randomStartRow;
                    end_x4 = randomStartCol + shiplength;
                    if (endpoints != null) endpoints.Add((end_y4, end_x4));
                }
                if (endpoints!.Count == 0)
                {
                   // If table is very small, then there's high probability AI will self-sabotage by placing a ship somewhere where it blocks subsequent ships, effectively breaking the game.
                }
                else
                {
                    Random rnd = new Random();
                    int index = rnd.Next(0, endpoints!.Count - 1);
                    (int endRow, int endCol) endpoint = endpoints![index];
                    game.PlaceShip(randomStartRow, endpoint.endRow, randomStartCol, endpoint.endCol, ships, board);
                }
        }

        static void SetShipCount(BattleShip game)
        {
            _shipCount = 1;
            _cellsLeft = game._x * game._y;
            _numberOfShipsPerPlayer = 0;

            ECellState[] shipNames =
            {
                ECellState.Carrier, ECellState.Battleship, ECellState.Submarine, ECellState.Cruiser, ECellState.Patrol
            };
            
            for (int i = 0; i < 5; i++)
            {
                ECellState shipName = shipNames[i];
                if ((int) shipName <= game._x && (int) shipName <= game._y)
                {
                    while (_cellsLeft > 0)
                    {
                        Console.WriteLine(
                            $"Enter number of {shipName}, {_cellsLeft} cells left. 1 x {shipName} = {(int) shipName} cells");
                        _shipCount = GetShipCountInput(Console.ReadLine()!);
                        if (DefineShipCount(shipName, _shipCount, game))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static int GetShipCountInput(string inputFromReadline)
        {
            const int playerInput = 1;
            if (inputFromReadline.Length == 0)
            {
                return playerInput;
            }

            if (int.TryParse(inputFromReadline.Trim(), out var realInput) && realInput < 80)
            {
                return realInput;
            }

            return playerInput;
        }

        static bool DefineShipCount(ECellState shipType, int count, BattleShip game)
        {
            if (count * (int) shipType > _cellsLeft)
            {
                Console.WriteLine("There are not enough cells to place so many ships");
                return false;
            }

            for (var i = 0; i < count; i++)
            {
                game.Ships1.Add(shipType);
                game.Ships2.Add(shipType);
            }

            _numberOfShipsPerPlayer += count;
            _cellsLeft -= count * (int) shipType;
            return true;
        }

        public static void PlaceShips(BattleShip game, ICollection<ECellState> ships, ECellState[,] board, string coordstring, bool consoleMode)
        {
            int startRow;
            int startCol;
            int endRow;
            int endCol;

            if (consoleMode == true && (int) ships.ToArray()[0] == 1 && coordstring.Length < 5)
            {
                (startRow, startCol) = GetSingleCoordinate(game, coordstring ?? "");
                endRow = startRow;
                endCol = startCol;
            }
            else
            {
              (startRow, startCol, endRow, endCol) = GetPlacementCoordinates(game, coordstring);  
            }
            
            if (startRow == -1 || startCol == -1 || endRow == -1 || endCol == -1)
            {
                Console.WriteLine(
                    $"Sorry, {ships.ToArray()[0]} requires {(int) ships.ToArray()[0]} squares, the specified placement coordinates are no good, try placing the boat again");
                return;
            }
            int[] rows = {startRow, endRow};
            Array.Sort(rows);
            startRow = rows[0];
            endRow = rows[1];
            int[] cols = {startCol, endCol};
            Array.Sort(cols);
            startCol = cols[0];
            endCol = cols[1];
            bool canPlaceShip = game.PlaceShip(startRow, endRow, startCol, endCol, ships, board);

            if (!canPlaceShip)
            {
                Console.WriteLine(
                    $"Sorry, {ships.ToArray()[0]} requires {(int) ships.ToArray()[0]} squares, the specified placement coordinates are no good, try placing the boat again");
                return;
            }

            if (consoleMode)
            {
                 BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
            }
           
            //return "";

        }

        private static string LoadGameAction(BattleShip game, Menu menu)
        {
            
            using var db = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
            db.Database.Migrate();
            var jsonString = "";
            Console.WriteLine("From db");
            foreach (var gs in db.GameStates.Include(g=>g.Game).OrderBy(gs=>gs.Id))
            {
                Console.WriteLine($"{gs} between {gs.Game!.PlayerA} & {gs.Game!.PlayerB}");
            }
            Console.Write("Please specify the GameState id you would like to load: ");
            var gsNo = int.Parse(Console.ReadLine()!);
            foreach (var gs in db.GameStates
                .Where(g=>g.Id == gsNo).Include(g=> g.Game))
            {
                jsonString = gs.Json;
                _game = gs.Game!;
            }

            game.SetGameStateFromJsonString(jsonString);
            
            // LOAD FROM FILESYSTEM
            /*var files = Directory.EnumerateFiles(".", "*.json").ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }
            var fileNo = Console.ReadLine();
            var fileName = files[int.Parse(fileNo!.Trim())];
            var jsonString = File.ReadAllText(fileName);
            game.SetGameStateFromJsonString(jsonString);*/
            
            BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
            menu.MenuItems["d"].Label = $"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!";
            return "";
        }


        private static string SaveGameAction(BattleShip game)
        {
            using var db = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
            db.Database.Migrate();
            
            string gamestate = game.GetSerializedGameState();
            GameState gs =new GameState()
            {
                Json = gamestate,
                DT = DateTime.Now,
                Game = _game
            };
            Console.WriteLine("Before add");
            Console.WriteLine(gs);
            var a = db.GameStates.Add(gs);
            Console.WriteLine("After add");
            db.SaveChanges();
            int id = gs.GameId;
            _game.Id = id;
            
            // SAVE TO FILESYSTEM
            /*var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.WriteLine($"File name ({defaultName}):");
            var fileName = Console.ReadLine();
            fileName = string.IsNullOrWhiteSpace(fileName) ? defaultName : $"{fileName}.json";

            var serializedGame = game.GetSerializedGameState();
            game.GetSerializedGameState();
            Console.WriteLine(serializedGame);
            File.WriteAllText(fileName, serializedGame);*/
            return "";
        }

        private static string BombAction(BattleShip game, Menu menu)
        {
            Console.Clear();
            BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
            Console.WriteLine("Upper left corner is (A,1)!");
            Console.Write($"Give X (A-{Letters[game._y - 1]}) Y (1-{game._x}): i.e a1: ");
            string input = Console.ReadLine()!;
            var (x, y) = GetSingleCoordinate(game, input);
            if ((x, y) == (-1, -1))
            {
                //var (board1, board2) = game.GetBoards();
                BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
                Console.WriteLine(
                    "Invalid move! Please enter a move in format: Letter,Number for example a1");
                menu.MenuItems["d"].Label = $"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!";
                return "";
            }

            if (game.MakeAMove(x, y))
            {
                BattleShipConsoleUi.DrawBombDrop();
                BattleShipConsoleUi.DrawLatestMoveResult(game);
            }
            menu.MenuItems["d"].Label = $"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!";
            if (game.GameOver == false)
            {
                // PLAYER VS AI GAME
                if (!game.NextMoveByPlayer2 && game.Player2Name == "AI")
                {
                    Thread.Sleep(750);
                    Console.WriteLine("CPU preparing to bomb...");
                    Thread.Sleep(2500);
                    Random r = new Random();
                    int randomX = r.Next(0, game._x);
                    int randomY = r.Next(0, game._y);
                    game.MakeAMove(randomX, randomY);
                    BattleShipConsoleUi.DrawBombDrop();
                    BattleShipConsoleUi.DrawLatestMoveResult(game);
                    menu.MenuItems["d"].Label = $"Player {(game.NextMoveByPlayer2 ? game.Player1Name : game.Player2Name)} drop a bomb on ENEMY board!";
                    return "";
                }
                return "";
            }
            else
            {
                BattleShipConsoleUi.DrawGameOver(game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
                Console.WriteLine("Final board results:");
                Thread.Sleep(1500);
                BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
                game.ChangeNextMoveByPlayer2();
                Thread.Sleep(600);
                BattleShipConsoleUi.DrawBoard(game.GetBoards(), game.NextMoveByPlayer2, game.Player1Name, game.Player2Name);
                Thread.Sleep(600);
                return "m";
            }
        }

        private static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet");
            return "";
        }


        private static string ColorOptionsAction()
        {
            Console.WriteLine($"Console color is now {Console.ForegroundColor}!!!");
            return "";
        }


        // VALIDATORS
        private static string ValidateName(string prompt, int num)
        {
            Console.Write(prompt);
            var playerName = Console.ReadLine();
            int nameLength = 0;

            if (playerName != null)
            {
                nameLength = playerName.Length;
            }

            if (nameLength == 0 || playerName == null)
            {
                playerName = $"Player {num}";
            }

            return playerName;
        }

        static (int x, int y) ValidateBoardDimensions(string prompt)
        {
            Console.Write(prompt);
            var userValue = Console.ReadLine();
            var x = 10;
            var y = 10;
            if (userValue != null && userValue.ToLower().Contains(','))
            {
                var values = userValue.Split(',');
                var isFirstNumeric = int.TryParse(values[0].Trim(), out int n);
                var isSecondNumeric = int.TryParse(values[1].Trim(), out int m);
                if (isFirstNumeric && isSecondNumeric)
                {
                    x = n;
                    y = m;
                    if (x > 20 || y > 20)
                    {
                        Console.Write("sorry, dimensions too big, Try our smaller table: the Exquisite 20x20");
                        x = 20;
                        y = 20;
                    }
                }
            }
            else
            {
                Console.Write("Entry requirements ignored, punishment: Boring 10x10 board");
            }

            return (x, y);
        }

        private static int ValidateTouchingOption(string prompt)
        {
            Console.Write(prompt);
            const int defaultOption = 0;
            var touchingOption = Console.ReadLine();
            if (touchingOption == "1" || touchingOption == "2" || touchingOption == "3")
            {
                return int.Parse(touchingOption) - 1;
            }

            Console.Write("I see you enjoy tricking the prompt. OK, play with boring rules, Touching Allowed!");
            return defaultOption;
        }


        static (int y, int x) GetSingleCoordinate(BattleShip game, string input)
        {
            if (input.Length == 2 || input.Length == 3)
            {
                string row = input.Substring(0, 1);
                string col = input.Substring(1);
                var isNotNumeric = !int.TryParse(row, out _);
                var isNumeric = int.TryParse(col, out _);
                if (isNotNumeric && isNumeric && Letters.Contains(Char.ToUpper(char.Parse(row))) &&
                    int.Parse(col) < 22 && int.Parse(col) > 0)
                {
                    var y = Array.IndexOf(Letters, Char.ToUpper(char.Parse(row)));
                    var x = int.Parse(col.Trim()) - 1;
                    if (y + 1 <= game._y && x + 1 <= game._x)
                    {
                        return (y, x);
                    }
                    else
                    {
                        return (-1, -1);
                    }
                }
                else
                {
                    return (-1, -1);
                }
            }
            else
            {
                return (-1, -1);
            }
        }


        static (int y_start, int x_start, int y_end, int x_end) GetPlacementCoordinates(BattleShip game, string optionalString="")
        {
            var coords = optionalString;
            //string coords = Console.ReadLine() ?? "";
            if (optionalString.Length > 1)
            {
                coords = optionalString;
            }
            if (coords.Length == 5 && coords.Contains("-") || coords.Length == 6 && coords.Contains("-") || coords.Length == 7 && coords.Contains("-") )
            {
                var coordinateValues = coords.Split('-');
                var (startRow, startCol) = GetSingleCoordinate(game, coordinateValues![0]);
                var (endRow, endCol) = GetSingleCoordinate(game, coordinateValues![1]);
                if (startRow == endRow || startCol == endCol)
                {
                    return (startRow, startCol, endRow, endCol);
                }
                else
                {
                    return (-1, -1, -1, -1);
                }
            }
            else
            {
                return (-1, -1, -1, -1);
            }
        }
    }
}