using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Controllers
{
    [Route("games")]
    public class GamesController : Controller
    {
        private readonly GameStoreContext context;
        private readonly UserManager<StoreUser> userManager;

        public GamesController(GameStoreContext context, UserManager<StoreUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: games
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            var games = from g in context.Games
                        select g;

            if (!string.IsNullOrEmpty(search))
            {
                games = games.Where(g => g.Title!.ToUpper().Contains(search.ToUpper()));
            }
            
            games = games.OrderByDescending(g => g.ReleaseDate);

            var gamesBrief = games.Select(g =>
                new GameBriefVM
                {
                    Id = g.Id,
                    Title = g.Title,
                    Price = g.Price,
                    ReleaseDate = g.ReleaseDate
                }
            );

            var gamesListVM = new GameListVM
            {
                Games = await ItemsPage<GameBriefVM>.NewAsync(gamesBrief, page, pageSize),
                Page = page
            };

            return View(gamesListVM);
        }

        // GET: games/3
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();
            
            var gameDetailedVM = new GameDetailedVM
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                Developer = game.Developer,
                Publisher = game.Publisher,
                Description= game.Description
            };
            return View(gameDetailedVM);
        }

        // GET: games/create
        [HttpGet("create")]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: games/create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Price,ReleaseDate,Developer,Publisher,Description")] Game game)
        {
            game.UserId = userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                context.Add(game);
                await context.SaveChangesAsync();

                TempData["Success"] = "Successfully published.";
                return RedirectToAction(nameof(Index));
            }

            return View(game);
        }

        // GET: games/3/edit
        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await context.Games.FindAsync(id);
            if (game == null) return NotFound();

            return View(game);
        }

        // POST: games/3/edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,ReleaseDate,Developer,Publisher,Description")] Game game)
        {
            if (id != game.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(game);
                    await context.SaveChangesAsync();
                    TempData["Success"] = "Successfully edited.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(game);
        }

        // GET: games/3/delete
        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var game = await context.Games.FirstOrDefaultAsync(m => m.Id == id);
            if (game == null) return NotFound();

            return View(game);
        }

        // POST: games/3/delete
        [HttpPost("{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await context.Games
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game != null)
            {
                context.Games.Remove(game);
                TempData["Success"] = "Successfully deleted.";
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return context.Games.Any(e => e.Id == id);
        }
    }
}
