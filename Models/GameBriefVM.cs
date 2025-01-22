using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class GameBriefVM
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public decimal Price { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
}