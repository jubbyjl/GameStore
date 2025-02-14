using Microsoft.AspNetCore.Mvc;
using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Controllers;

[Route("account")]
[Authorize]
public class AccountController: Controller
{
    private readonly UserManager<StoreUser> userManager;

    public AccountController(UserManager<StoreUser> userManager)
    {
        this.userManager = userManager;
    }

    // GET: account/balance
    [HttpGet("balance")]
    public async Task<IActionResult> Balance()
    {
        var user = await userManager.GetUserAsync(User);
        return View(user!.Balance);
    }

    // POST: account/balance/addfunds
    [HttpPost("balance/addfunds")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFunds()
    {
        var user = await userManager.GetUserAsync(User);
        if (user!.Balance < 100)
        {
            user.Balance = 100;
            await userManager.UpdateAsync(user);
            TempData["Success"] = "Successfully added funds.";
        }
        return RedirectToAction(nameof(Balance));
    }
}