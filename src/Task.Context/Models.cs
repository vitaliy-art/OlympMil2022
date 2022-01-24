using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Task.Context.Models;

public enum JuniorRanks
{
    Private,
    Sergeant
}

public enum SeniorRanks
{
    Captain,
    Major
}

[Owned]
public class Person
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
}

public class Division
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Cadet
{
    public int Id { get; set; }

    [Required]
    public Person? Person { get; set; }
    public JuniorRanks Rank { get; set; }
}

public class Officer
{
    public int Id { get; set; }
    public Person? Person { get; set; }
    public SeniorRanks Rank { get; set ;}
}
