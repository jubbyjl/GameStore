using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.Models;

public class ReviewListVM
{
    public string? GameTitle { get; set; }
    public int TotalReviews { get; set; }
    public decimal PercentPositive { get; set; }
    public ItemsPage<Review>? Reviews { get; set; }
    public int Page { get; set; }
    public SelectList? Filters { get; set; }
    public string? Filter { get; set; }
    public Review? CurrentUserReview { get; set; }
}