using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Game
    {
        public int Id { get; set; }

        public DateTime CreationDT { get; set; }

        [MaxLength(64)]
        public string PlayerA { get; set; }  = default!;

        [MaxLength(64)]
        public string PlayerB { get; set; } = default!;


        public ICollection<GameState> GameStates { get; set; } = default!;

        public override string ToString()
        {
            return PlayerA + " vs " + PlayerB + " " + GameStates.Count;
        }
    }
}