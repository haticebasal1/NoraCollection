using System;

namespace NoraCollection.Entities.Abstract;

public abstract class BaseEntity : ISoftDeletable, IEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdateAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}
