using System;
using NoraCollection.Shared.Dtos.AuthDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IAuthService
{
    Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto);
    Task<ResponseDto<NoContentDto>> LogoutAsync();
    Task<ResponseDto<TokenDto>> RefreshTokenAsync(string refreshToken);
    Task<ResponseDto<UserDto>> GetCurrentUserAsync();
    Task<ResponseDto<NoContentDto>> UpdateProfileAsync(UserUpdateDto userUpdateDto);
    Task<ResponseDto<NoContentDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<ResponseDto<NoContentDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<ResponseDto<NoContentDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
