using System;
using System.Collections.Generic;

namespace RecruitmentSystem.API.Common;

// Generic paged result used by many endpoints
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

// Lightweight API result wrapper
public class ApiResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public static ApiResult<T> Ok(T data, string? message = null) => new() { Success = true, Data = data, Message = message };
    public static ApiResult<T> Fail(string message) => new() { Success = false, Message = message };
}

// Common constants
public static class AppConstants
{
    public const string IsoDateTimeFormat = "o"; // ISO 8601
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;
}

// Base entity interfaces and classes to standardize new models
public interface IHasPublicId
{
    Guid PublicId { get; set; }
}

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

public abstract class EntityBase : IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public abstract class EntityWithPublicId : EntityBase, IHasPublicId
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
}

// Simple file metadata used across modules (onboarding, bgv, offers)
public class FileMetadataDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty; // encrypted when persisted
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? Comments { get; set; }
    public int? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; }
}

// Query parameters helper
public class PagingQuery
{
    public int PageNumber { get; set; } = AppConstants.DefaultPageNumber;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;
}
