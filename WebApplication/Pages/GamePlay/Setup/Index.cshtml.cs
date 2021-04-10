using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp;
using GameConsoleUI;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication.Pages.GamePlay
{
    public class Index2 : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.ApplicationDbContext _context;
        private static readonly char[] Letters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V'
        };
        
        public Index2(DAL.ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IList<Game>? Games { get;set; }
        public IList<Domain.GameState>? GameStates { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public ECellState[,] b1 { get; set; } = null!;
        public ECellState[,] b2 { get; set; } = null!;
        public IList<ECellState> ships1 { get; set; } = null!;
        public IList<ECellState> ships2 { get; set; } = null!;
        public ICollection<ECellState> ships11 { get; set; } = null!;
        public ICollection<ECellState> ships22 { get; set; } = null!;
        public BattleShip? TheGame { get; set; }
        public Game? gamee { get; set; }

        public async Task OnGetAsync()
        {
            GameStates = await _context.GameStates.Include(g=> g.Game).ToListAsync();
            var json = GameStates.Last().Json;
            var g = GameStates.Last().Game;
            gamee = g;
            BattleShip game = new BattleShip(5, 5, "unknown1", "unknown2");
            game.SetGameStateFromJsonString(json);
            ships1 = game.Ships1.ToList();
            ships2 = game.Ships2.ToList();
            x = game._x;
            y = game._y;
            b1 = game._board1;
            b2 = game._board2;
            TheGame = game;
        }
        
        
        public async Task<IActionResult> OnPostAsync(string Carrier_A, string Battleship_A, string Submarine_A, string Cruiser_A, string Patrol_A,
            string Carrier_B, string Battleship_B, string Submarine_B, string Cruiser_B, string Patrol_B)
        {
            GameStates = await _context.GameStates.Include(g=> g.Game).ToListAsync();
            var j = GameStates.Last().Json;
            var g = GameStates.Last().Game;
            gamee = g;
            BattleShip game = new BattleShip(5, 5, "unknown1", "unknown2");
            game.SetGameStateFromJsonString(j);
            ships11 = game.Ships1;
            ships22 = game.Ships2;
            x = game._x;
            y = game._y;
            b1 = game._board1;
            b2 = game._board2;
            TheGame = game;
            TheGame.ShipCountPerPlayer = 5;
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships1, b2, Carrier_A, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships1, b2, Battleship_A, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships1, b2, Submarine_A, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships1, b2, Cruiser_A, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships1, b2, Patrol_A, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships2, b1, Carrier_B, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships2, b1, Battleship_B, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships2, b1, Submarine_B, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships2, b1, Cruiser_B, false);
            ConsoleApp.Program.PlaceShips(TheGame, TheGame.Ships2, b1, Patrol_B, false);
            string json = TheGame.GetSerializedGameState();
            Domain.GameState gs = new Domain.GameState()
            {
                DT = DateTime.Now,
                Game = gamee,
                Json = json
            };
            _context.GameStates.Add(gs);
            await _context.SaveChangesAsync();
            int gsid = gs.Id;
            int gid = gs.GameId;
            return RedirectToPage("../Fight/Index", new {gameid = gid,  gamestateid = gsid});
        }
    }
}
