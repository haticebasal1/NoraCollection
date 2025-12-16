using System;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class UserDto
{
 public string? Id { get; set; }
 public string? FirstName { get; set; }
 public string? LastName { get; set; }
 public DateTime? BirthDate { get; set; }
 public string? Address { get; set; }
 public string? City { get; set; }
 public Gender? Gender { get; set; }
 public DateTimeOffset RegistrationDate { get; set; }
 public string? UserName { get; set; }
 public string? Email { get; set; }
 public string? EmailConfirmed { get; set; }
}
