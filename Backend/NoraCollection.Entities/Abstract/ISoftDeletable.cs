using System;

namespace NoraCollection.Entities.Abstract;

public interface ISoftDeletable
{
  bool IsDeleted { get; set;}

}
