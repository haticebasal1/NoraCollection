using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mevcut şifre zorunludur!")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifre zorunludur!")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Şifre tekrarı zorunludur!")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor!")]
    public string? ConfirmNewPassword { get; set; }
}
