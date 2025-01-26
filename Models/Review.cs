using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Models;

public class Review
{
    public int Id { get; set; }
    public bool IsPositive { get; set; }
    public string? Description { get; set; }
    public DateTime TimeCreated { get; set; }

    public int GameId { get; set; }
    public Game? Game { get; set; }
}