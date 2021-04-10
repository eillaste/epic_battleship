using System;
using System.Collections.Generic;
using ConsoleApp;

namespace Domain
{
    public class Ship
    {
        public ECellState ShipType { get; set; }
        public int ShipSize { get; set; }
        public Boolean IsSunk { get; set; }
        public List<int[]> HealthyCoords { get; set; }
        //public int[,] HitCoords { get; set; }
        //public List<int[]> HealthyCoords2 { get; set; }
        public List<int[]> HitCoords { get; set; }

        public Ship(ECellState shipType, int shipSize, List<int[]> healthyCoords)
        {
            ShipType = shipType;
            ShipSize = shipSize;
            IsSunk = false;
            HealthyCoords = healthyCoords;
            HitCoords = new List<int[]>();
        }

        
    }
}