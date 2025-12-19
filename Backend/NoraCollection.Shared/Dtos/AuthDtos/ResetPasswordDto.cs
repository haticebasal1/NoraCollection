using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class ResetPasswordDto
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }  // Email'den gelen reset token

    [Required(ErrorMessage = "Yeni şifre zorunludur!")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Şifre tekrarı zorunludur!")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor!")]
    public string? ConfirmNewPassword { get; set; }
}
