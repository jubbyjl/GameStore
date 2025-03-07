using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class GameDetailedVM: GameBriefVM
{
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public string? Description { get; set; }
    public bool UserIsPublisher { get; set; }
    public bool InUserLibrary { get; set; }
}