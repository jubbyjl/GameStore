using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Models;

public class StoreUserGamePurchase
{
    public string? UserId { get; set; }
    public int GameId { get; set; }
    public StoreUser? User { get; set; }
    public Game? Game { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PaymentAmount { get; set; }
    public DateTime TimeMade { get; set; }
}
