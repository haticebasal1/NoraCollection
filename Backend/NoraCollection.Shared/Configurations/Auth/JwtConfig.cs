namespace NoraCollection.Shared.Configurations.Auth;

public class JwtConfig
{
    // Token'ı üreten servis. Genelde API adın (örn: "NoraCollection.API").
    public string Issuer { get; set; } = string.Empty;
    // Token'ın hedefi; kim kullanacak (örn: "NoraCollection.Frontend").
    public string Audience { get; set; } = string.Empty;
    // Token imzalamak için gizli anahtar. En az 32 karakter olmalı.
    public string Secret { get; set; } = string.Empty;
    // Access token geçerlilik süresi (dakika). Örn: 30 = yarım saat.
    public double AccessTokenExpiration { get; set; }
    // Refresh token geçerlilik süresi (dakika). Örn: 43200 = 30 gün.
    public double RefreshTokenExpiration { get; set; }
}
