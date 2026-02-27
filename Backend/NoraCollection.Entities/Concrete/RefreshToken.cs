using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class RefreshToken:IEntity
{
    //tokenın benzersiz kimlik(guid)
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public string? Token { get; set; }
  public string? UserId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  //ne zaman geçersiz olacak?
  public DateTime ExpiresAt { get; set; }
  //token kullanıldı mı
  public bool IsRevoked { get; set; }
  // Eğer bu token bir önceki token'ı yenilediyse, onun Id'si burada tutulur.
  public string? ReplacedByTokenId { get; set; }
  public User? User { get; set; }
}
