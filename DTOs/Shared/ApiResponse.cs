namespace Nafes.API.DTOs.Shared;

/// <summary>
/// Standardized API response wrapper for consistent responses across all endpoints
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>Indicates if the request was successful</summary>
    public bool Success { get; set; }
    
    /// <summary>Human-readable message describing the result</summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>The response data payload</summary>
    public T? Data { get; set; }
    
    /// <summary>List of error messages if any</summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>Timestamp of the response</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>Request trace ID for debugging</summary>
    public string? TraceId { get; set; }

    /// <summary>Create a successful response</summary>
    public static ApiResponse<T> Ok(T data, string message = "تمت العملية بنجاح")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>Create an error response</summary>
    public static ApiResponse<T> Error(string message, params string[] errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors.ToList()
        };
    }

    /// <summary>Create an error response with exception details</summary>
    public static ApiResponse<T> Error(string message, Exception ex)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { ex.Message }
        };
    }
}

/// <summary>
/// Paginated response wrapper extending ApiResponse
/// </summary>
public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>Current page number (1-indexed)</summary>
    public int Page { get; set; }
    
    /// <summary>Number of items per page</summary>
    public int PageSize { get; set; }
    
    /// <summary>Total number of items across all pages</summary>
    public int TotalCount { get; set; }
    
    /// <summary>Total number of pages</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>Create a successful paginated response</summary>
    public static PaginatedResponse<T> Ok(
        IEnumerable<T> data, 
        int page, 
        int pageSize, 
        int totalCount, 
        string message = "تمت العملية بنجاح")
    {
        return new PaginatedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

/// <summary>
/// Legacy PagedResult for backward compatibility
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
