using Microsoft.AspNetCore.Mvc;
using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;

namespace GameStore.Controllers;

[Route("games/{gameid}")]
[Authorize]
public class PurchaseController : Controller
{
    private readonly GameStoreContext context;
    private readonly UserManager<StoreUser> userManager;

    public PurchaseController(GameStoreContext context, UserManager<StoreUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("addfree"), ActionName("AddFree")]
    public IActionResult AddFreeReturn(int gameid)
    {
        // intended login return url for AddFree POST action
        return RedirectToActionPermanent("Details", "Games", new { id = gameid });
    }

    // POST: games/3/addfree
    [HttpPost("addfree")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFree(int gameid)
    {
        var userId = userManager.GetUserId(User);
        var game = await context.Games
            .Include(x => x.Purchases.Where(p => p.UserId == userId))
            .SingleOrDefaultAsync(x => x.Id == gameid);

        if (game == null) return NotFound();
        if (game.Price != 0) return BadRequest();
        // if user already owns
        if (game.Purchases.Count != 0) return RedirectToAction("Details", "Games", new { id = gameid });

        var purchase = new StoreUserGamePurchase
        {
            UserId = userId,
            GameId = gameid,
            PaymentAmount = game.Price,
            TimeMade = DateTime.UtcNow,
        };
        context.Add(purchase);
        await context.SaveChangesAsync();
        TempData["Success"] = "Successfully added to library.";

        return RedirectToAction("Details", "Games", new { id = gameid });
    }

    // GET: games/3/checkout
    [HttpGet("checkout")]
    public async Task<IActionResult> Checkout(int gameid)
    {
        var userId = userManager.GetUserId(User);
        var res = await context.Games
            .Where(x => x.Id == gameid)
            .Select(x => new {
                CheckoutVM = new PurchaseCheckoutVM {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                },
                Existing = x.Purchases.Where(x => x.UserId == userId)
            })
            .SingleOrDefaultAsync();

        if (res == null) return NotFound();
        if (res.CheckoutVM.Price <= 0) return BadRequest();
        if (res.Existing.Any()) return RedirectToAction("Details", "Games", new { id = gameid });
        
        return View(res.CheckoutVM);
    }

    // POST: games/3/checkout
    [HttpPost("checkout"), ActionName("Checkout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckoutConfirmed(int gameid)
    {
        var userId = userManager.GetUserId(User);
        var game = await context.Games
            .Include(x => x.Purchases.Where(p => p.UserId == userId))
            .SingleOrDefaultAsync(x => x.Id == gameid);

        if (game == null) return NotFound();
        if (game.Price <= 0) return BadRequest();
        if (game.Purchases.Count != 0) return RedirectToAction("Details", "Games", new { id = gameid });
        
        var user = await userManager.GetUserAsync(User);
        if (user!.Balance < game.Price) // user cant afford
        {
            return RedirectToAction("Balance", "Account");
        }

        var purchase = new StoreUserGamePurchase
        {
            UserId = userId,
            GameId = gameid,
            PaymentAmount = game.Price,
            TimeMade = DateTime.UtcNow,
        };
        context.Add(purchase);

        user.Balance -= game.Price;
        context.Update(user);

        await context.SaveChangesAsync();
        TempData["Success"] = "Successfully purchased.";

        return RedirectToAction("Details", "Games", new { id = gameid });
    }
}