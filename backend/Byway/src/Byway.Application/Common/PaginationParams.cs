namespace Byway.Application.Common;

/// <summary>
/// Shared pagination input for every list endpoint. Default PageSize (10)
/// is intentionally uniform across the API — individual screens can pass a
/// different value if their layout needs it (e.g. a 3-column course grid
/// might request 9 or 12), but nothing in the API assumes a different
/// default per-endpoint.
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }
}
