using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.Models;

public class GameListVM
{
    public ItemsPage<GameBriefVM>? Games { get; set; }
    public int Page { get; set; }
    public string? Search { get; set; }
}