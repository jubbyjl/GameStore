using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Models;

public class GameSubmitVM
{
    [MinLength(1)]
    [Required]
    public string? Title { get; set; }
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    [Required]
    public string? Developer { get; set; }
    [Required]
    public string? Publisher { get; set; }
    [Required]
    public string? Description { get; set; }
}