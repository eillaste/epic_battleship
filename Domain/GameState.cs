using System;

namespace Domain
{
    public class GameState
    {
        public int Id { get; set; }
        public DateTime DT { get; set; }
        //what about [MaxLength(?)], max size is 2 x 20x20 boards filled with ships = approx 27716 characters and 27kB, however no way to surpass that so constraint not necessary.
        public string Json { get; set; } = default!;
        
        public int GameId { get; set; }
        public Game? Game { get; set; }
        
        public override string ToString()
        {
            return "GameState id: " + Id + " " +  "GameId: " + GameId +  " " +  "Created: " + DT + " ";
        }
    }
    
}