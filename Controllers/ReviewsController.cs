using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Controllers
{
    [Route("games/{gameid}/reviews")]
    public class ReviewsController : Controller
    {
        private readonly GameStoreContext context;
        private readonly UserManager<StoreUser> userManager;

        public ReviewsController(GameStoreContext context, UserManager<StoreUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: games/3/reviews
        [HttpGet]
        public async Task<IActionResult> Index(int gameId, string? filter, int page = 1, int pageSize = 10)
        {
            var game = await context.Games.FindAsync(gameId);
            if (game == null) return NotFound();

            var reviews = from r in context.Reviews
                          where r.GameId == gameId
                          select r;
            
            var counts = await (
                from r in reviews
                group r by 1 into g
                select new {
                    Total = g.Count(),
                    Positive = g.Count(x => x.IsPositive),
                }
            ).FirstOrDefaultAsync();

            int total = counts != null ? counts.Total : 0;
            decimal percentPositive = 0;
            if (total > 0)
            {
                percentPositive = (decimal)counts!.Positive / counts.Total * 100;
            }

            var selectAll = new SelectListItem { Value = "all", Text = "All" };
            var selectPositive = new SelectListItem { Value = "positive", Text = "Positive only" };
            var selectNegative = new SelectListItem { Value = "negative", Text = "Negative only" };

            if (!filter.IsNullOrEmpty())
            {
                if (filter!.Equals(selectPositive.Value))
                {
                    reviews = reviews.Where(r => r.IsPositive);
                } 
                else if (filter.Equals(selectNegative.Value))
                {
                    reviews = reviews.Where(r => !r.IsPositive);
                }
            }

            reviews = reviews.OrderByDescending(r => r.TimeCreated)
                             .Include(r => r.User);

            Review? currentUserReview = null;
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                currentUserReview = await context.Reviews
                    .FirstOrDefaultAsync(r => r.GameId == game.Id && r.UserId == userId);
            }

            var reviewListVM = new ReviewListVM
            {
                GameTitle = game.Title,
                TotalReviews = total,
                PercentPositive = percentPositive,
                Reviews = await ItemsPage<Review>.NewAsync(reviews, page, pageSize),
                Page = page,
                Filters = new SelectList
                (
                    new[] { selectAll, selectPositive, selectNegative },
                    "Value",
                    "Text"
                ),
                CurrentUserReview = currentUserReview,
            };
            return View(reviewListVM);
        }

        // GET: games/3/reviews/create
        [HttpGet("create")]
        [Authorize]
        public async Task<IActionResult> Create(int gameId)
        {
            var game = await context.Games.FindAsync(gameId);
            if (game == null) return NotFound();

            var userId = userManager.GetUserId(User);
            var reviewExists = await context.Reviews
                .AnyAsync(r => r.UserId == userId && r.GameId == gameId);

            if (reviewExists)
            {
                return RedirectToAction(nameof(Index), new { gameid = gameId });
            }

            var reviewCreateVM = new ReviewCreateVM
            {
                GameTitle = game.Title,
            };

            return View(reviewCreateVM);
        }

        // POST: games/3/reviews/create
        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int gameId, ReviewCreateVM reviewCreateVM)
        {
            var game = await context.Games.FindAsync(gameId);
            if (game == null) return NotFound();

            var userId = userManager.GetUserId(User);
            var reviewExists = await context.Reviews
                .AnyAsync(r => r.UserId == userId && r.GameId == gameId);

            if (reviewExists)
            {
                return RedirectToAction(nameof(Index), new { gameid = gameId });
            }

            var review = new Review
            {
                IsPositive = reviewCreateVM.IsPositive,
                Description = reviewCreateVM.Description,
                GameId = gameId,
                TimeCreated = DateTime.UtcNow,
                UserId = userId
            };

            if (ModelState.IsValid)
            {
                context.Add(review);
                await context.SaveChangesAsync();

                TempData["Success"] = "Successfully created.";
                return RedirectToAction(nameof(Index), new { gameid = gameId });
            }
            
            reviewCreateVM.GameTitle = game.Title;
            return View(reviewCreateVM);
        }
    }
}