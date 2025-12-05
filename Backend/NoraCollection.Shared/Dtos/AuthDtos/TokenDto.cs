using System;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class TokenDto
{
    public string? AccessToken { get; set; }
    public DateTime AccessTokenExpiretionDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiretion { get; set; }
}
