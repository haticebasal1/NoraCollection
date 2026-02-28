using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Configurations.Auth;
using NoraCollection.Shared.Dtos.AuthDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class AuthManager : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtConfig _jwtConfig;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<RefreshToken> _refreshToken;
    private readonly IEmailService _emailManager;
    private readonly IEmailTemplateService _emailTemplateServices;

    public AuthManager(UserManager<User> userManager, IOptions<JwtConfig> options, IUnitOfWork unitOfWork, IGenericRepository<RefreshToken> refreshToken, IEmailService emailManager, IEmailTemplateService emailTemplateServices)
    {
        _userManager = userManager;
        _jwtConfig = options.Value;
        _unitOfWork = unitOfWork;
        _refreshToken = _unitOfWork.GetRepository<RefreshToken>();
        _emailManager = emailManager;
        _emailTemplateServices = emailTemplateServices;
    }

    public async Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Kullanıcı adı VEYA e-posta ile ara (ikisiyle de giriş yapılabilir)
            var user = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail!)
                        ?? await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail!);
            if (user is null)
            {
                return ResponseDto<TokenDto>.Fail("Kullanıcı adı veya e-posta hatalı", StatusCodes.Status404NotFound);
            }
            //Şifre doğrula
            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!isValidPassword)
            {
                return ResponseDto<TokenDto>.Fail("Kullanıcı şifre hatalı!", StatusCodes.Status400BadRequest);
            }
            //JWT Access Token + Refresh Token üret ve dön (RefreshToken DB'ye kaydedilmiyor)
            var tokenResponse = await GenerateTokenAsync(user);
            return tokenResponse;
        }
        catch (Exception ex)
        {
            return ResponseDto<TokenDto>.Fail($"Giriş yapılırken hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Kullanıcı adı daha önce alınmış mı?
            var existingByName = await _userManager.FindByNameAsync(registerDto.UserName!);
            if (existingByName is not null)
            {
                return ResponseDto<UserDto>.Fail("Bu kullanıcı adı zaten kullanılıyor!", StatusCodes.Status400BadRequest);
            }
            // E-Posta daha önce alınmış mı?
            var existingByEmail = await _userManager.FindByEmailAsync(registerDto.Email!);
            if (existingByEmail is not null)
            {
                return ResponseDto<UserDto>.Fail("Bu e-posta zaten kullanılıyor!", StatusCodes.Status400BadRequest);
            }
            //User entity constructor: (firstName, lastName, birthDate, address, city, gender, registerionDate)
            var user = new User(
                registerDto.FirstName!,
                registerDto.LastName!,
                registerDto.BirthDate,
                registerDto.Address,
                registerDto.City,
                registerDto.Gender,
                DateTimeOffset.UtcNow
            )
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                EmailConfirmed = true
            };
            // Kullanıcı oluştur (Identity parola hash'ler)
            var result = await _userManager.CreateAsync(user, registerDto.Password!);
            if (!result.Succeeded)
            {
                var errors = string.Join(";", result.Errors.Select(x => x.Description));
                return ResponseDto<UserDto>.Fail(errors, StatusCodes.Status400BadRequest);
            }
            //Varsayılan "User" rolü ver
            await _userManager.AddToRoleAsync(user, "User");

            var subject = "Hoş Geldiniz - NoraCollection";
            var body = _emailTemplateServices.GetTemplate("welcome", new Dictionary<string, string>
            {
                ["FirstName"] = user.FirstName ?? ""
            });
            _ = await _emailManager.SendEmailAsync(user.Email!, subject, body);
            //Manuel UserDto mapping (RegisterionDate -> RegistrationDate, EmailConfirmed bool->string)
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                City = user.City,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                RegistrationDate = user.RegisterionDate,
                EmailConfirmed = user.EmailConfirmed.ToString()
            };
            return ResponseDto<UserDto>.Success(userDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<UserDto>.Fail($"Kayıt sırasında hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
    public async Task<ResponseDto<TokenDto>> RefreshTokenAsync(string? refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ResponseDto<TokenDto>.Fail("Refresh token gerekli!", StatusCodes.Status400BadRequest);
            }
            // Token'ı DB'de ara: Token eşleşmeli, süresi dolmamış, iptal edilmemiş olmalı
            var repo = _refreshToken;
            var tokenEntity = await repo.GetAsync(
                t => t.Token == refreshToken && t.ExpiresAt > DateTime.UtcNow && !t.IsRevoked
            );
            if (tokenEntity is null)
            {
                return ResponseDto<TokenDto>.Fail("Geçersiz veya süresi dolmuş refresh token! ", StatusCodes.Status401Unauthorized);
            }
            var user = await _userManager.FindByIdAsync(tokenEntity.UserId!);
            if (user is null)
            {
                return ResponseDto<TokenDto>.Fail("Kullanıcı bulunamadı!", StatusCodes.Status404NotFound);
            }
            var tokenResponse = await GenerateTokenAsync(user);
            if (!tokenResponse.IsSuccessful || tokenResponse.Data is null)
            {
                return tokenResponse;
            }
            // Eski token'ı iptal et (IsRevoked = true)
            tokenEntity.IsRevoked = true;
            repo.Update(tokenEntity);

            await _unitOfWork.SaveAsync();
            return tokenResponse;
        }
        catch (Exception ex)
        {
            return ResponseDto<TokenDto>.Fail($"Token yenilenirken hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> LogoutAsync(string? refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ResponseDto<NoContentDto>.Success(new NoContentDto(), StatusCodes.Status200OK);
            }
            var tokenEntity = await _refreshToken.GetAsync(
                t => t.Token == refreshToken
            );
            if (tokenEntity is not null)
            {
                tokenEntity.IsRevoked = true;
                _refreshToken.Update(tokenEntity);
                await _unitOfWork.SaveAsync();
            }
            return ResponseDto<NoContentDto>.Success(new NoContentDto(), StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Çıkış yapılırken hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
    public async Task<ResponseDto<NoContentDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
            {
                return ResponseDto<NoContentDto>.Fail("E-posta zorunludur!", StatusCodes.Status400BadRequest);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email!);
            if (user is null)
            {
                // Güvenlik: "Kayıtlı değil" dememek için yine Success dön
                return ResponseDto<NoContentDto>.Success(new NoContentDto(),StatusCodes.Status200OK);
            }
            //Identity ile şifre sıfırlama token'ı üret (1 saat geçerli)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // URL'de kullanmak için özel karakterleri encode et
            var encodedToken = Uri.EscapeDataString(token);
            //Frontend'deki reset-password sayfasının linki (email + token query'de gidecek)
            var resetLink = $"{_jwtConfig.FrontendBaseUrl.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";
            //reset-password.html template'inden içerik al, placeholders'ı doldur
            var body = _emailTemplateServices.GetTemplate("reset-password", new Dictionary<string, string>
            {
                ["FirstName"] = user.FirstName ?? "",
                ["ResetLink"] = resetLink
            });
            // E-postayı gönder (başarısız olsa bile kullanıcıya hata göstermiyoruz)
            _ = await _emailManager.SendEmailAsync(user.Email!, "Şifre Sıfırlama - NoraCollection", body);
            return ResponseDto<NoContentDto>.Success(new NoContentDto(),StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Şifre sıfırlama e-postası gönderilemedi!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            
            if (string.IsNullOrWhiteSpace(resetPasswordDto.Email) || string.IsNullOrWhiteSpace(resetPasswordDto.Token) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
            {
                return ResponseDto<NoContentDto>.Fail("E-posta, token ve yeni şifre zorunludur!", StatusCodes.Status400BadRequest);
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);
            if (user is null)
            {
                return ResponseDto<NoContentDto>.Fail("Geçersiz veya süresi dolmuş bağlantı.", StatusCodes.Status400BadRequest);
            }
            //Token URL'den geldiği için tekrar decode et (ForgotPassword'da EscapeDataString yaptık)
            var token = Uri.UnescapeDataString(resetPasswordDto.Token!);

            //Identity ile şifreyi sıfırla (token geçerli mi, süresi dolmuş mu kontrol eder)
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword!);

            if (!result.Succeeded)
            {
                // Token geçersiz, süresi dolmuş veya şifre kurallarına uymuyor
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ResponseDto<NoContentDto>.Fail($"Şifre sıfırlanamadı: {errors}", StatusCodes.Status400BadRequest);
            }

            return ResponseDto<NoContentDto>.Success(new NoContentDto(), StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Şifre sıfırlama sırasında hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
    // JWT Access Token + Refresh Token üretir. JwtConfig'ten süre (dakika) okunur.
    private async Task<ResponseDto<TokenDto>> GenerateTokenAsync(User user)
    {
        try
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,user.Id),
                new(ClaimTypes.Name,user.UserName ?? ""),
                new(ClaimTypes.Email,user.Email ?? "")
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Access token: AccessTokenExpiration dakika (örn: 30)
            var expriy = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiration);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpriy = DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenExpiration);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: expriy,
                signingCredentials: creds
            );
            var tokenDto = new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiretionDate = expriy,
                RefreshToken = refreshToken,
                RefreshTokenExpiretion = refreshTokenExpriy
            };

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpriy,
                IsRevoked = false
            };
            var repo = _refreshToken;
            await repo.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveAsync();
            return ResponseDto<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<TokenDto>.Fail($"Token üretilirken hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
    // Rastgele Refresh Token üretir. (Şu an DB'ye kaydedilmiyor; ileride refresh endpoint eklenebilir)
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
