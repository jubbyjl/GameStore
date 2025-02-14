using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Models;

public class StoreUser: IdentityUser
{
    public List<Game> Games { get; } = new List<Game>();
    public List<Review> Reviews { get; } = new List<Review>();

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    public List<StoreUserGamePurchase> Purchases { get; } = [];
    public List<Game> Library { get; } = [];
}
