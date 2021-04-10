using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ConsoleApp;
using Domain;


namespace GameBrain
{
    public class BattleShip
    {
        public bool GameOver = false;
        public ECellState LatestMoveResult;
        public int _x;
        public int _y;
        public string Player1Name;
        public string Player2Name;

        public bool _nextMoveByPlayer2 { get; set; }
        public ECellState[,] _board1;
        public ECellState[,] _board2;
        public int ShipCountPerPlayer;
        public ETouchingRule TouchingRule = ETouchingRule.CornersCanTouch;

        public ICollection<Ship> Player1Ships { get; set; }
        public ICollection<Ship> Player2Ships { get; set; }

        public ICollection<ECellState> Ships1;
        public ICollection<ECellState> Ships2;

        public bool NextMoveByPlayer2 => _nextMoveByPlayer2;

        public BattleShip(int x, int y, string p1name, string p2name)
        {
            _x = x;
            _y = y;
            _board1 = new ECellState[x, y];
            _board2 = new ECellState[x, y];
            Player1Ships = new List<Ship>();
            Player2Ships = new List<Ship>();
            Ships1 = new List<ECellState>();
            Ships2 = new List<ECellState>();
            Player1Name = p1name;
            Player2Name = p2name;
        }

        public ECellState GetShip()
        {
            return ECellState.Battleship;
        }


        //PLACING SHIPS
        public bool PlaceShip(int startRow, int endRow, int startCol, int endCol, ICollection<ECellState> playerShips,
            ECellState[,] board)
        {
            
            var lastRowIndex = _y - 1;
            var lastColIndex = _x - 1;
            var shipLength = (int) playerShips.ToArray()[0];
            ECellState shipType = playerShips.ToArray()[0];
            var desiredPlacementSize = 0;
            for (int i = startRow; i < endRow + 1; i++)
            {
                for (int j = startCol; j < endCol + 1; j++)
                {
                    if (board[i, j] == ECellState.Empty)
                    {
                        desiredPlacementSize += 1;
                    }
                }
            }

            List<int[]> shipCoords2 = new List<int[]>();

            if (desiredPlacementSize.Equals(shipLength))
            {
                
                var thCellInShip = 0;

                for (int i = startRow; i < endRow + 1; i++)
                {
                    for (int j = startCol; j < endCol + 1; j++)
                    {
                        if (board[i, j] == ECellState.Empty)
                        {
                            var cellPositionOnBoard = CheckEdgeTouching(i, j, lastRowIndex, lastColIndex);

                            var cellsTouched = 0;
                            switch (thCellInShip)
                            {
                                case 0:
                                {
                                    if (TouchingRule == ETouchingRule.CanNotTouch)
                                    {
                                        cellsTouched = SwitchTouchFunctionNoTouchAllowed(cellPositionOnBoard, board, i, j);
                                    }
                                    else if (TouchingRule == ETouchingRule.CornersCanTouch)
                                    {
                                        cellsTouched =
                                            SwitchTouchFunctionCornersTouchAllowedFirstCell(cellPositionOnBoard, board, i,
                                                j);
                                    }
                                    else if (TouchingRule == ETouchingRule.CanTouch)
                                    {
                                        cellsTouched = 0;
                                    }
                                    if (cellsTouched == 0)
                                    {
                                        board[i, j] = shipType;
                                        shipCoords2.Add(new[] {i, j});
                                        thCellInShip++;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                                case > 0:
                                {
                                    if (TouchingRule == ETouchingRule.CanTouch)
                                    {
                                        cellsTouched = 0;
                                    }
                                    else if (TouchingRule == ETouchingRule.CanNotTouch)
                                    {
                                        cellsTouched = SwitchTouchFunctionNoTouchAllowed(cellPositionOnBoard, board, i, j);
                                    }
                                    else if (TouchingRule == ETouchingRule.CornersCanTouch && thCellInShip == shipLength)
                                    {
                                        cellsTouched =
                                            SwitchTouchFunctionCornersTouchAllowedFirstCell(cellPositionOnBoard, board, i,
                                                j) + 1;
                                    }
                                    // this 2 is a bit fishy
                                    if (cellsTouched <= 1 || cellsTouched == 2)
                                    {
                                        //PLACE
                                        board[i, j] = shipType;
                                        shipCoords2.Add(new[] {i, j});
                                        thCellInShip++;
                                    }
                                    else
                                    {
                                        foreach (var coord in shipCoords2)
                                        {
                                            board[coord[0], coord[1]] =
                                                ECellState.Empty;
                                        }

                                        shipCoords2.Clear();
                                        return false;
                                    }

                                    break;
                                }
                                default:
                                    return false;
                            }
                        }

                        thCellInShip++;
                    }
                }

                if (Player1Ships.Count < ShipCountPerPlayer)
                {
                    Ships1 = Ships1.Skip(1).ToArray();
                    Player1Ships.Add(new Ship(shipType, shipLength, shipCoords2));
                    return true;
                }
                else
                {
                    _nextMoveByPlayer2 = false;
                    Ships2 = Ships2.Skip(1).ToArray();
                    Player2Ships.Add(new Ship(shipType, shipLength, shipCoords2));
                    return true;
                }
            }
            return false;
        }


        private static int CountTouches(ECellState[] surroundingCells)
        {
            int numberOfTouches = 0;
            foreach (var cell in surroundingCells)
            {
                if (cell == ECellState.Empty)
                {
                    numberOfTouches += 0;
                }
                else
                {
                    numberOfTouches += 1;
                }
            }

            return numberOfTouches;
        }

        private static int SwitchTouchFunctionNoTouchAllowed(ETouchingEdge cellPosition, ECellState[,] board, int i, int j)
        {
            switch (cellPosition)
            {
                case ETouchingEdge.UpperLeft:
                    return CountTouches(new[] {board[i, j + 1], board[i + 1, j], board[i + 1, j + 1]});
                case ETouchingEdge.Upper:
                    return CountTouches(new[]
                        {board[i, j - 1], board[i, j + 1], board[i + 1, j - 1], board[i, j + 1], board[i + 1, j + 1]});
                case ETouchingEdge.UpperRight:
                    return CountTouches(new[] {board[i, j - 1], board[i + 1, j - 1], board[i + 1, j]});
                case ETouchingEdge.Left:
                    return CountTouches(new[]
                        {board[i - 1, j], board[i - 1, j + 1], board[i, j + 1], board[i + 1, j], board[i + 1, j + 1]});
                case ETouchingEdge.Right:
                    return CountTouches(new[]
                        {board[i - 1, j], board[i - 1, j - 1], board[i, j - 1], board[i + 1, j - 1], board[i + 1, j]});
                case ETouchingEdge.LowerLeft:
                    return CountTouches(new[] {board[i - 1, j], board[i - 1, j + 1], board[i, j + 1]});
                case ETouchingEdge.LowerRight:
                    return CountTouches(new[] {board[i - 1, j], board[i - 1, j - 1], board[i, j - 1]});
                case ETouchingEdge.Lower:
                    return CountTouches(new[]
                        {board[i - 1, j - 1], board[i - 1, j], board[i - 1, j + 1], board[i, j - 1], board[i, j + 1]});
                case ETouchingEdge.Middle:
                    return CountTouches(new[]
                    {
                        board[i - 1, j - 1], board[i - 1, j], board[i - 1, j + 1], board[i, j - 1], board[i, j + 1],
                        board[i + 1, j - 1], board[i + 1, j], board[i + 1, j + 1]
                    });
            }

            return 5;
        }

        private static int SwitchTouchFunctionCornersTouchAllowedFirstCell(ETouchingEdge cellPosition,
            ECellState[,] board, int i, int j)
        {
            switch (cellPosition)
            {
                // Check all touching sides besides corners> these all have to return zero for first cell! corners may touch
                case ETouchingEdge.UpperLeft:
                    return CountTouches(new[] {board[i, j + 1], board[i + 1, j]});
                case ETouchingEdge.Upper:
                    return CountTouches(new[] {board[i, j - 1], board[i, j + 1], board[i + 1, j]});
                case ETouchingEdge.UpperRight:
                    return CountTouches(new[] {board[i, j - 1], board[i + 1, j]});
                case ETouchingEdge.Left:
                    return CountTouches(new[] {board[i - 1, j], board[i, j + 1], board[i + 1, j]});
                case ETouchingEdge.Right:
                    return CountTouches(new[] {board[i - 1, j], board[i, j - 1], board[i + 1, j]});
                case ETouchingEdge.LowerLeft:
                    return CountTouches(new[] {board[i - 1, j], board[i, j + 1]});
                case ETouchingEdge.LowerRight:
                    return CountTouches(new[] {board[i - 1, j], board[i, j - 1]});
                case ETouchingEdge.Lower:
                    return CountTouches(new[] {board[i - 1, j], board[i, j - 1], board[i, j + 1]});
                case ETouchingEdge.Middle:
                    return CountTouches(new[]
                    {
                        board[i - 1, j], board[i, j - 1], board[i, j + 1],
                        board[i + 1, j]
                    });
            }

            return 5;
        }

        private static ETouchingEdge CheckEdgeTouching(int i, int j, int lastRowIndex, int lastColIndex)
        {
            if (i == 0 && j == 0)
            {
                return ETouchingEdge.UpperLeft;
            }

            if (i == 0 && j < lastColIndex)
            {
                return ETouchingEdge.Upper;
            }

            if (i == 0 && j == lastColIndex)
            {
                return ETouchingEdge.UpperRight;
            }

            if (i < lastRowIndex && j == 0)
            {
                return ETouchingEdge.Left;
            }

            if (i == lastRowIndex && j == 0)
            {
                return ETouchingEdge.LowerLeft;
            }

            if (i == lastRowIndex && j < lastColIndex)
            {
                return ETouchingEdge.Lower;
            }

            if (i == lastRowIndex && j == lastColIndex)
            {
                return ETouchingEdge.LowerRight;
            }

            if (i < lastRowIndex && j == lastColIndex)
            {
                return ETouchingEdge.Right;
            }
            else
            {
                return ETouchingEdge.Middle;
            }
        }
        
        // FOR UI DRAWING
        public (ECellState[,] board1, ECellState[,] board2) GetBoards()
        {
            if (_nextMoveByPlayer2)
            {
                return (_board1, _board2);
            }
            else
            {
                return (_board2, _board1);
            }
        }

        public bool MakeAMove(int x, int y)
        {
            bool isMoveValid = ValidateMove(x, y);
            Console.Clear();
            if (isMoveValid)
            {
                var bombDropped = DropBomb(x, y);
                if (_nextMoveByPlayer2)
                {
                    CheckForGameOver(Player2Ships);
                    return bombDropped;
                }
                else
                {
                    CheckForGameOver(Player1Ships);
                    return bombDropped;
                }
            }

            return false;
        }
        
        private bool ValidateMove(int x, int y)
        {
            return x < _x && y < _y;
        }
        
        private void CheckForGameOver(ICollection<Ship> playerShips)
        {
            var deadShips = playerShips.Count(ship => ship.HealthyCoords.Count == 0);

            if (deadShips == playerShips.Count)
            {
                GameOver = true;
            }
            else
            {
                _nextMoveByPlayer2 = !_nextMoveByPlayer2;
            }
        }
        
        private bool DropBomb(int x, int y)
        {
            if (!_nextMoveByPlayer2)
            {
                SwitchFunc(_board2, x, y, Player1Ships);
            }
            else
            {
                SwitchFunc(_board1, x, y, Player2Ships);
            }

            return true;
        }
        
        private void SwitchFunc(ECellState[,] board, int x, int y, ICollection<Ship> ships)
        {
            switch (board[x, y])
            {
                case ECellState.Carrier:
                    LatestMoveResult = ECellState.Hit;
                    DealWithHealthyHitCoordinates(board, x, y, ships);
                    break;
                case ECellState.Battleship:
                    LatestMoveResult = ECellState.Hit;
                    DealWithHealthyHitCoordinates(board, x, y, ships);
                    break;
                case ECellState.Submarine:
                    LatestMoveResult = ECellState.Hit;
                    DealWithHealthyHitCoordinates(board, x, y, ships);
                    break;
                case ECellState.Cruiser:
                    LatestMoveResult = ECellState.Hit;
                    DealWithHealthyHitCoordinates(board, x, y, ships);
                    break;
                case ECellState.Patrol:
                    LatestMoveResult = ECellState.Hit;
                    DealWithHealthyHitCoordinates(board, x, y, ships);
                    break;
                case ECellState.Empty:
                    board[x, y] = ECellState.Miss;
                    LatestMoveResult = ECellState.Miss;
                    break;
                case ECellState.Hit:
                    LatestMoveResult = ECellState.Miss;
                    break;
                case ECellState.Miss:
                    LatestMoveResult = ECellState.Miss;
                    break;
                case ECellState.Sunk:
                    LatestMoveResult = ECellState.Miss;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void DealWithHealthyHitCoordinates(ECellState[,] board, int x, int y, ICollection<Ship> ships)
        {
            foreach (var ship in ships)
            {
                if (ship.HealthyCoords.Any(p => p.SequenceEqual(new[] {x, y})))
                {
                    ship.HitCoords.Add(ship.HealthyCoords.Find(p => p.SequenceEqual(new[] {x, y})));
                    ship.HealthyCoords.Remove(ship.HealthyCoords.Find(p => p.SequenceEqual(new[] {x, y})));

                    if (ship.HealthyCoords.Count == 0)
                    {
                        foreach (var hitCoord in ship.HitCoords)
                        {
                            board[hitCoord[0], hitCoord[1]] = ECellState.Sunk;
                        }
                        LatestMoveResult = ECellState.Sunk;
                        ship.IsSunk = true;
                    }
                    else
                    {
                        board[x, y] = ECellState.Hit;
                    }

                    break;
                }
            }
        }

        private static ECellState[][] TwoDimensionalArrayToJaggedArray(ECellState[,] twoDimensionalArray)
        {
            ECellState[] obj1D = twoDimensionalArray.Cast<ECellState>().ToArray();

            var j = 0;
            ECellState[][] jagged = obj1D.GroupBy(_ => j++ / twoDimensionalArray.GetLength(1)).Select(y => y.ToArray())
                .ToArray();
            return jagged;
        }
        
        public string GetSerializedGameState()
        {
            ECellState[][] jaggedArrayFromBoard1 = TwoDimensionalArrayToJaggedArray(_board1);
            ECellState[][] jaggedArrayFromBoard2 = TwoDimensionalArrayToJaggedArray(_board2);

            var state = new GameStateDTO
            {
                NextMoveByPlayer2 = _nextMoveByPlayer2,
                Board1 = jaggedArrayFromBoard1,
                Board2 = jaggedArrayFromBoard2,
                Width = _x,
                Height = _y,
                ships1 = Ships1,
                ships2 = Ships2,
                player1Ships = Player1Ships,
                player2Ships = Player2Ships,
                TouchingRule = TouchingRule,
                Player1Name = Player1Name,
                Player2Name = Player2Name
            };
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
        }
        
        public void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameStateDTO>(jsonString);

            // restore actual state from deserialized state
            _nextMoveByPlayer2 = state!.NextMoveByPlayer2;
            _board1 = new ECellState[state.Width, state.Height];
            _board2 = new ECellState[state.Width, state.Height];

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    _board1[x, y] = state.Board1[x][y];
                }
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    _board2[x, y] = state.Board2[x][y];
                }
            }

            Ships1 = state.ships1;
            Ships2 = state.ships2;
            Player1Ships = state.player1Ships;
            Player2Ships = state.player2Ships;
            TouchingRule = state.TouchingRule;
            Player1Name = state.Player1Name;
            Player2Name = state.Player2Name;
            _x = state.Width;
            _y = state.Height;

        }

        public void ChangeNextMoveByPlayer2()
        {
            _nextMoveByPlayer2 = !_nextMoveByPlayer2;
        }
    }
}