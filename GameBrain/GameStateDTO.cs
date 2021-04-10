using System.Collections.Generic;
using ConsoleApp;
using Domain;

namespace GameBrain
{
    public class GameStateDTO
    {
        public bool NextMoveByPlayer2 { get; set; }
        public ECellState[][] Board1 { get; set; } = null!;
        public ECellState[][] Board2 { get; set; } = null!;
        public string Player1Name { get; set; } = null!;
        public string Player2Name { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public ETouchingRule TouchingRule { get; set; }
        public ICollection<ECellState> ships1 { get; set; } = null!;
        public ICollection<ECellState> ships2 { get; set; } = null!;
        public ICollection<Ship> player1Ships { get; set; } = null!;
        public ICollection<Ship> player2Ships { get; set; } = null!;
    }
}