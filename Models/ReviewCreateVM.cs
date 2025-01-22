namespace GameStore.Models;

public class ReviewCreateVM
{
    public string? GameTitle { get; set; }
    public bool IsPositive { get; set; }
    public string? Description { get; set; }
}