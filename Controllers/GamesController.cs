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
        public async Task<IActionResult> Index(string? search, string? sort, int page = 1, int pageSize = 10)
        {
            var games = from g in context.Games
                        select g;

            if (!string.IsNullOrEmpty(search))
            {
                games = games.Where(g => g.Title!.ToUpper().Contains(search.ToUpper()));
            }
            
            var sortReleaseDesc = new SelectListItem { Value = "", Text = "Release Date" };
            var sortNameAsc = new SelectListItem { Value = "nameAsc", Text = "Name" };
            var sortPriceAsc = new SelectListItem { Value = "priceAsc", Text = "Lowest Price" };
            var sortPriceDesc = new SelectListItem { Value = "priceDesc", Text = "Highest Price" };
            var sortReviewsDesc = new SelectListItem { Value = "reviewsDesc", Text = "User Reviews" };

            if (string.IsNullOrEmpty(sort))
            {
                games = games.OrderByDescending(g => g.ReleaseDate);
            }
            else if (sort == sortNameAsc.Value)
            {
                games = games.OrderBy(g => g.Title)
                             .ThenByDescending(g => g.ReleaseDate);
            }
            else if (sort == sortPriceAsc.Value)
            {
                games = games.OrderBy(g => (double)g.Price) // sqlite doesnt support decimal
                             .ThenByDescending(g => g.ReleaseDate);
            }
            else if (sort == sortPriceDesc.Value)
            {
                games = games.OrderByDescending(g => (double)g.Price)
                             .ThenByDescending(g => g.ReleaseDate);
            }
            else if (sort == sortReviewsDesc.Value)
            {
                games = games.OrderByDescending(g => g.Reviews.Any() ? (double)g.Reviews.Count(x => x.IsPositive) / g.Reviews.Count() : 0)
                             .ThenByDescending(g => g.ReleaseDate);
            }

            var gamesBrief = games.Select(g =>
                new GameBriefVM
                {
                    Id = g.Id,
                    Title = g.Title,
                    Price = g.Price,
                    ReleaseDate = g.ReleaseDate.Date,
                    TotalReviews = g.Reviews.Count(),
                    PositiveReviews = g.Reviews.Count(x => x.IsPositive),
                }
            );

            var gamesListVM = new GameListVM
            {
                Games = await ItemsPage<GameBriefVM>.NewAsync(gamesBrief, page, pageSize),
                Page = page,
                Sorts = new SelectList(
                    new[] { sortReleaseDesc, sortNameAsc, sortPriceAsc, sortPriceDesc, sortReviewsDesc },
                    "Value",
                    "Text"
                ),
            };

            return View(gamesListVM);
        }

        // GET: games/3
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var games = from g in context.Games
                        select g;

            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                games = games.Include(x => x.Purchases.Where(p => p.UserId == userId));
            }

            var game = await games.SingleOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();

            var gameDetailedVM = new GameDetailedVM
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate.Date,
                Developer = game.Developer,
                Publisher = game.Publisher,
                Description = game.Description,
                UserIsPublisher = game.UserId == userId,
                InUserLibrary = game.Purchases.Count != 0,
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
        public async Task<IActionResult> Create(GameSubmitVM gameSubmitVM)
        {
            var newGame = new Game
            {
                Title = gameSubmitVM.Title,
                Price = Math.Round(gameSubmitVM.Price, 2),
                Developer = gameSubmitVM.Developer,
                Publisher = gameSubmitVM.Publisher,
                Description = gameSubmitVM.Description,
                ReleaseDate = DateTime.UtcNow,
                UserId = userManager.GetUserId(User),
            };

            if (TryValidateModel(newGame, nameof(newGame)))
            {
                context.Add(newGame);
                await context.SaveChangesAsync();

                TempData["Success"] = "Successfully published.";
                return RedirectToAction(nameof(Index));
            }

            return View(gameSubmitVM);
        }

        // GET: games/3/edit
        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var game = await context.Games.FindAsync(id);
            if (game == null) return NotFound();

            var userId = userManager.GetUserId(User);
            if (game.UserId != userId)
            {
                return Forbid();
            }

            var gameSubmitVM = new GameSubmitVM
            {
                Title = game.Title,
                Price = game.Price,
                Developer = game.Developer,
                Publisher = game.Publisher,
                Description = game.Description,
            };

            return View(gameSubmitVM);
        }

        // POST: games/3/edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GameSubmitVM gameSubmitVM)
        {
            var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();
            
            var userId = userManager.GetUserId(User);
            if (game.UserId != userId)
            {
                return Forbid();
            }

            game.Title = gameSubmitVM.Title;
            game.Price = Math.Round(gameSubmitVM.Price, 2);
            game.Developer = gameSubmitVM.Developer;
            game.Publisher = gameSubmitVM.Publisher;
            game.Description = gameSubmitVM.Description;

            if (TryValidateModel(game, nameof(game)))
            {
                try
                {
                    context.Update(game);
                    await context.SaveChangesAsync();
                    TempData["Success"] = "Successfully edited.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await context.Games.AnyAsync(g => g.Id == game.Id))
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
            return View(gameSubmitVM);
        }

        // GET: games/3/delete
        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();

            var userId = userManager.GetUserId(User);
            if (game.UserId != userId)
            {
                return Forbid();
            }

            return View(game);
        }

        // POST: games/3/delete
        [HttpPost("{id}/delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();

            var userId = userManager.GetUserId(User);
            if (game.UserId != userId)
            {
                return Forbid();
            }

            context.Games.Remove(game);
            TempData["Success"] = "Successfully deleted.";

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
