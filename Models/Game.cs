using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Models;

public class Game
{
    public int Id { get; set; }

    [MinLength(1)]
    [Required]
    public string? Title { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required]
    public string? Developer { get; set; }

    [Required]
    public string? Publisher { get; set; }

    [Required]
    public string? Description { get; set; }

    public List<Review> Reviews { get; } = new List<Review>();

    public string? UserId { get; set; }
}