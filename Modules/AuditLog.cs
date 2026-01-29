using System;

namespace Nafes.API.Modules;

public class AuditLog : BaseModel
{
    public long? AdminId { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string EntityName { get; set; } = string.Empty; // Student, Question, etc.
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty; // JSON or text summary
    public string IpAddress { get; set; } = string.Empty;
}
