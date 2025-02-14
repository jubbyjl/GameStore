using Microsoft.AspNetCore.Mvc;
using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers;

[Route("library")]
[Authorize]
public class LibraryController: Controller
{
    private readonly UserManager<StoreUser> userManager;

    public LibraryController(UserManager<StoreUser> userManager)
    {
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);
        var library = await userManager.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Library)
            .SingleOrDefaultAsync();

        return View(library);
    }
}