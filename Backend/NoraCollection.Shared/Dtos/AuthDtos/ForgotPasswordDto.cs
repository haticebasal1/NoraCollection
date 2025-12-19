using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email zorunludur!")]
    [EmailAddress(ErrorMessage = "Geçerli bir email giriniz!")]
    public string? Email { get; set; }
}
