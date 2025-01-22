using Microsoft.EntityFrameworkCore;

namespace GameStore;

public class ItemsPage<T> where T : class
{
    public List<T> Items { get; private set; }
    public int TotalItems { get; private set; }
    public int PageNum { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }

    public bool CanPrev => PageNum > 1;
    public bool CanNext => PageNum < TotalPages;

    public ItemsPage(List<T> items, int totalItems, int pageNum, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageNum = pageNum;
        PageSize = pageSize;
        TotalPages = (totalItems + pageSize - 1) / pageSize;
    }

    public static async Task<ItemsPage<T>> NewAsync(IQueryable<T> src, int pageNum, int pageSize)
    {
        var totalItems = await src.CountAsync();
        var items = await src.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
        return new ItemsPage<T>(items, totalItems, pageNum, pageSize);
    }
}
