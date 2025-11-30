using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class CustomDesign : BaseEntity, IEntity
{
    public string? UserId { get; set; }
    public User? User { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsCompleted { get; set; } = false;
}
