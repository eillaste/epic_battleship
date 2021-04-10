using System;
using System.Threading;
using ConsoleApp;
using GameBrain;

namespace GameConsoleUI
{
    public static class BattleShipConsoleUi
    {
        private static readonly string[] Letters =
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V"
        };

        public static void DrawBoard((ECellState[,] board1, ECellState[,] board2) boards, bool nextMoveByPlayer2, string p1name, string p2name)
        {
            //Console.WriteLine(nextMoveByX);
            var (board1, board2) = boards;

            var width = board1.GetUpperBound(0) + 1; // x
            var height = board1.GetUpperBound(1) + 1; // y

            var multiplier = width * 5 - 5;
            var multiplier2 = width * 5 - 4;
            string spaces = new string(' ', multiplier);
            string equalSigns = new string('=', multiplier2);

            WriteTitle(equalSigns);
            InsertWhiteSpace(2);
            WriteLetters(width);
            InsertWhiteSpace(3);
            WriteLetters(width);
            Console.WriteLine();
            InsertWhiteSpace(2);
            DrawDividerLine(width);
            InsertWhiteSpace(3);
            DrawDividerLine(width);
            Console.WriteLine();

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                DrawRowNumbers(rowIndex);
                DrawCellsInRow(width, board1, rowIndex, true);
                InsertWhiteSpace(3);
                DrawCellsInRow(width, board2, rowIndex, false);
                Console.WriteLine();
                InsertWhiteSpace(2);
                DrawDividerLine(width);
                InsertWhiteSpace(1);
                InsertWhiteSpace(2);
                DrawDividerLine(width);
                Console.WriteLine();
            }

            DrawLegend(spaces, nextMoveByPlayer2, p1name, p2name);
            Console.WriteLine();
        }

        public static void Greet()
        {
            Console.WriteLine();
            Console.WriteLine("============> WELCOME TO BATTLESHIP! <=============");
            Console.WriteLine("Press ENTER for default choice 'n' i.e. New Game ");
            Console.WriteLine("--------------------------------------------------");
        }

        private static void WriteLetters(int width)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"  {Letters[colIndex]}  ");
            }

            Console.ResetColor();
        }

        private static void InsertWhiteSpace(int numberOfSpaces)
        {
            Console.Write(new string(' ', numberOfSpaces));
        }

        private static void DrawRowNumbers(int rowIndex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(rowIndex >= 9 ? $"{rowIndex + 1}" : $" {rowIndex + 1}");
            Console.ResetColor();
        }


        private static void WriteTitle(string equalSigns)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($" {equalSigns}EPIC BATTLESHIP{equalSigns}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DrawDividerSymbols(string symbols)
        {
            Console.Write(symbols);
        }

        private static void DrawDividerLine(int width)
        {
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                DrawDividerSymbols("+---+");
            }
        }

        private static void DrawCellsInRow(int width, ECellState[,] board, int rowIndex,
            bool enemyBoard)
        {
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"| {CellString(board[colIndex, rowIndex], enemyBoard)} |");
                Console.ResetColor();
            }
        }

        private static void DrawLegend(string spaces, bool nextMoveByPlayer2, string p1name, string p2name)
        {
            Console.Write("  ");
            Console.WriteLine($"{spaces}ENEMY   {spaces} {(nextMoveByPlayer2 ? p1name : p2name)}");

            Console.Write("Legend:");
            Console.Write(" ");
            Console.WriteLine();

            DrawLegendCell(ConsoleColor.DarkGray, "| - |", "Miss");
            DrawLegendCell(ConsoleColor.DarkRed, "| X |", "Hit");
            DrawLegendCell(ConsoleColor.White, "| S |", "Sunk");
            DrawLegendCell(ConsoleColor.Blue, "| 5 |", "Carrier");
            DrawLegendCell(ConsoleColor.Red, "| 4 |", "Battleship");
            DrawLegendCell(ConsoleColor.Yellow, "| 3 |", "Submarine");
            DrawLegendCell(ConsoleColor.Magenta, "| 2 |", "Cruiser");
            DrawLegendCell(ConsoleColor.Green, "| 1 |", "Patrol");
            Console.WriteLine();
        }

        private static void DrawLegendCell(ConsoleColor legendCellColor, string symbol, string meaning)
        {
            Console.BackgroundColor = legendCellColor;
            Console.Write(symbol);
            Console.ResetColor();
            Console.Write(" ");
            Console.Write(meaning);
            Console.Write(" ");
        }

        public static string CellString(ECellState cellState, bool enemyBoard)
        {
            switch (cellState)
            {
                case ECellState.Empty:
                    return "?";
                case ECellState.Carrier:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.Black : ConsoleColor.Blue;
                    return enemyBoard ? "?" : "5";
                case ECellState.Battleship:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.Black : ConsoleColor.Red;
                    return enemyBoard ? "?" : "4";
                case ECellState.Submarine:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.Black : ConsoleColor.Yellow;
                    return enemyBoard ? "?" : "3";
                case ECellState.Cruiser:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.Black : ConsoleColor.Magenta;
                    return enemyBoard ? "?" : "2";
                case ECellState.Patrol:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.Black : ConsoleColor.Green;
                    return enemyBoard ? "?" : "1";
                case ECellState.Hit:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    return "X";
                case ECellState.Miss:
                    Console.BackgroundColor = enemyBoard ? ConsoleColor.DarkGray : ConsoleColor.Black;
                    return "-";
                case ECellState.Sunk:
                    Console.BackgroundColor = ConsoleColor.White;
                    return "S";
                default:
                    throw new ArgumentOutOfRangeException(nameof(cellState), cellState, null);
            }
        }

        
        public static void DrawGameOver(bool nextMoveByPlayer2, string p1name, string p2name)
        {
            Console.WriteLine(@"
                                     |__
                                     |\/
                                     ---
                                     / | [
                              !      | |||
                            _/|     _/|-++'
                        +  +--|    |--|--|_ |-
                     { /|__|  |/\__|  |--- |||__/
                    +---------------___[}-_===_.'____                 /\
                ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _
 __..._____--==/___]_|__|_____________________________[___\==--____,------' .7
|               GAME OVER! Player " + (nextMoveByPlayer2 ? p1name : p2name) + @" WINS!                       BB-61/
 \_________________________________________________________________________|
  original ASCII art by Matthew Bace
");

            Thread.Sleep(3500);
        }

        public static void DrawLatestMoveResult(BattleShip game)
        {
            switch (game.LatestMoveResult)
            {
                case ECellState.Hit:
                    Console.WriteLine("IT'S A HIT!");
                    Console.WriteLine(@"
     _.-^^---....,,--
 _--                  --_
<                        >)
|                         |
 \._                   _./
    ```--. . , ; .--'''
          | |   |
       .-=||  | |=-.
       `-=#$%&%$#=-'
          | ;  :|
 _____.,-#%&$@%#&#~,._____ 

");
                    break;
                case ECellState.Miss:
                    Console.WriteLine("IT'S A MISS!");
                    Console.WriteLine(@"
           _.====.._
         ,:._       ~-_
             `\        ~-_
               | _  _  |  `.
             ,/ /_)/ | |    ~-_
    -..__..-''  \_ \_\ `_      ~~--..__...----... MISSED ...
");
                    break;
                case ECellState.Sunk:
                    Console.WriteLine(@"
                               ________________
                          ____/ (  (    )   )  \___
                         /( (  (  )   _    ))  )   )\
                       ((     (   )(    )  )   (   )  )
                     ((/  ( _(   )   (   _) ) (  () )  )
                    ( (  ( (_)   ((    (   )  .((_ ) .  )_
                   ( (  )    (      (  )    )   ) . ) (   )
                  (  (   (  (   ) (  _  ( _) ).  ) . ) ) ( )
                  ( (  (   ) (  )   (  ))     ) _)(   )  )  )
                 ( (  ( \ ) (    (_  ( ) ( )  )   ) )  )) ( )
                  (  (   (  (   (_ ( ) ( _    )  ) (  )  )   )
                 ( (  ( (  (  )     (_  )  ) )  _)   ) _( ( )
                  ((  (   )(      ~Nice shot!~  _) _(_ (  (_ )
                   (_((__(_(__(( ( ( |  ) ) ) )_))__))_)___)
                   ((__)        \\||lll|l||///          \_))
                            (   /(/ (  )  ) )\   )
                          (    ( ( ( | | ) ) )\   )
                           (   /(| / ( )) ) ) )) )
                         (     ( ((((_(|)_)))))     )
                          (      ||\(|(|)|/||     )
                        (        |(||(||)||||        )
                          (     //|/l|||)|\\ \     )
                        (/ / //  /|//||||\\  \ \  \ _)
");
                    Thread.Sleep(2000);
                    Console.Clear();
                    Console.WriteLine(@"
                   '''YOU SUNK AN ENEMY SHIP!'''

                 Enemy: Help! ..glub ..glubalghlghlgh... 
                        /                 
          ~^~ ~^ ~^~ ~^~ ~^ ~       
           ~^~^ ~^~ ~ \o/ ^~^~ ^~     
          ~^~ ^~^ ~^~ ^~^ ^~ 
");
                    Thread.Sleep(3000);
                    break;
            }
        }

        public static void DrawBombDrop()
        {
            Console.WriteLine("STARTING TO DROP BOMB");
            Console.WriteLine(@"

     ||
     ||   
   |\**/|      
   \ == /
    |  |
    |  |
    \  /
     \/
");
            Thread.Sleep(1000);
            Console.Clear();
        } 
    }
}