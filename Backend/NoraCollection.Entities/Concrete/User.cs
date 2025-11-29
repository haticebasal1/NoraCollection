using System;
using Microsoft.AspNetCore.Identity;
using NoraCollection.Entities.Abstract;
using NoraCollection.Shared;
namespace NoraCollection.Entities.Concrete;

public class User : IdentityUser, IEntity
{
    private User()
    {

    }
    public User(string firstName, string lastName, DateTime? birthDate, string? address, string? city, Gender? gender, DateTimeOffset registerionDate)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
        City = city;
        Gender = gender;
        RegisterionDate = registerionDate;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public Gender? Gender { get; set; }
    public DateTimeOffset RegisterionDate { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Favorite> Favorites { get; set; } = [];
    public ICollection<Cart> Carts { get; set; } = [];
}
