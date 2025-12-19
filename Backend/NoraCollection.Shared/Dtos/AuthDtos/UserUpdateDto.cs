using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class UserUpdateDto
{
    [Required(ErrorMessage = "Ad zorunludur!")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Soyad zorunludur!")]
    public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public Gender? Gender { get; set; }
    public string? PhoneNumber { get; set; }
}
