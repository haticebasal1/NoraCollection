using System;
using NoraCollection.Shared.Dtos.AuthDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IAuthService
{
  Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto);
  Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto);
  Task<ResponseDto<TokenDto>> RefreshTokenAsync(string? refreshToken);
  Task<ResponseDto<NoContentDto>> LogoutAsync(string? refreshToken);
  Task<ResponseDto<NoContentDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
  Task<ResponseDto<NoContentDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
