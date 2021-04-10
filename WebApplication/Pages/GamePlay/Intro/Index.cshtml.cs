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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication.Pages.GamePlay
{
    public class Index1 : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.ApplicationDbContext _context;
        
        public Index1(DAL.ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /*public async Task OnGetAsync()
        {

        }*/


        public BattleShip BattleShip { get; set; } = null!;

        public void InsertBoat(ECellState shipType, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                BattleShip.Ships1.Add(shipType);
                BattleShip.Ships2.Add(shipType);
            }
        }
        
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string Player1Name, string Player2Name, int x, int y, int TouchingRule)
        {

            BattleShip = new BattleShip(x, y,Player1Name, Player2Name);
                InsertBoat(ECellState.Carrier, 1);
                InsertBoat(ECellState.Battleship, 1);
                InsertBoat(ECellState.Submarine, 1);
                InsertBoat(ECellState.Cruiser, 1);
                InsertBoat(ECellState.Patrol, 1);
                BattleShip.TouchingRule = ETouchingRule.CanTouch;

                Domain.GameState gameState = new Domain.GameState()
                    {
                        DT = DateTime.Now,
                        Json = BattleShip.GetSerializedGameState(),
                        Game = new Game()
                        {
                            CreationDT = DateTime.Now,
                            PlayerA = BattleShip.Player1Name,
                            PlayerB = BattleShip.Player2Name
                        }
                    }
;
                // createInitialGamestate
                _context.GameStates.Add(gameState);
                await _context.SaveChangesAsync();

                /*_context.Games.Add(Game);
                await _context.SaveChangesAsync();*/
                return RedirectToPage("../Setup/Index");
                
            //return Page();
        }
    }
}