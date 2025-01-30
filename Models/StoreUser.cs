using Microsoft.AspNetCore.Identity;

namespace GameStore.Models;

public class StoreUser: IdentityUser
{
    public List<Game> Games { get; } = new List<Game>();
    public List<Review> Reviews { get; } = new List<Review>();
}
