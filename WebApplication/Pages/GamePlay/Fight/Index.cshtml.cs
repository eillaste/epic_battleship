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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace WebApplication.Pages.GamePlay
{
    public class Index3 : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.ApplicationDbContext _context;
        
        public Index3(DAL.ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IList<Game>? Games { get;set; }
        public IList<Domain.GameState>? GameStates { get; set; }
        public Domain.GameState? GameState { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public ECellState[,] b1 { get; set; } = null!;
        public ECellState[,] b2 { get; set; } = null!;
        public string player1name { get; set; } = null!;
        public string player2name { get; set; } = null!;
        public bool nextmovebyplayer2 { get; set; }
        public int gid { get; set; }
        public int gsid { get; set; }
        public bool GameOver { get; set; }

        public async Task OnGetAsync(int gameid, int gamestateid, int y=999, int x=999)
        {
            gid = gameid;
            gsid = gamestateid;
            //GameStates = await _context.GameStates.Include(g=> g.Game).ToListAsync();
            GameState =  _context.GameStates.Find(gsid);
            
            if (GameState == null)
            {
                GameState =  _context.GameStates.Find(gsid-1);
            }
            var j = GameState.Json;
            var g = _context.Games.Find(gameid);
            BattleShip game = new BattleShip(5, 5, "unknown1", "unknown2");
            game.SetGameStateFromJsonString(j);


            if (x != 999 && y != 999)
            {
                game.MakeAMove(x, y);
                game._nextMoveByPlayer2 = game._nextMoveByPlayer2;
                string json = game.GetSerializedGameState();
                Domain.GameState gs = new Domain.GameState()
                {
                    DT = DateTime.Now,
                    Game = g,
                    Json = json
                };
                // createInitialGamestate
                _context.GameStates.Add(gs);
                await _context.SaveChangesAsync();
                /*int gsid = gs.Id;
                int gid = gs.GameId;*/
            }
            
            width = game._x;
            height = game._y;
            player1name = game.Player1Name;
            player2name = game.Player2Name;
            nextmovebyplayer2 = game._nextMoveByPlayer2;
            if (game._nextMoveByPlayer2)
            {
                b1 = game._board1;
                b2 = game._board2;
            }
            else
            {
                b1 = game._board2;
                b2 = game._board1;
            }

            if (game.GameOver)
            {
                GameOver = true;
            }


            //return RedirectToPage("/Index");
        }
    }
}
