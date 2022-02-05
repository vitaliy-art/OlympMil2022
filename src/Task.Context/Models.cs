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
    Captain = 10,
    Major
}

[Owned]
public class Person
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
}

public class Division
{
    public int Id { get; set; }
    public int DisplayId { get; set; }
    public string Name { get; set; }

    public override string ToString() =>
        $"{{Division: {{Id: {DisplayId}, Name: {Name}}}";
}

public class Cadet
{
    public int Id { get; set; }
    public int DisplayId { get; set; }

    [Required]
    public Person Person { get; set; }
    public JuniorRanks Rank { get; set; }

    public int? DivisionId { get; set; }
    public Division Division { get; set; }

    public override string ToString() => ToString("irfmlb");

    public string ToString(string args)
    {
        var result = "{Cadet: {";
        var lastIndex = args.Length - 1;

        foreach (var arg in args)
        {
            result += arg switch
            {
                'i' => $"Id: {DisplayId}",
                'r' => $"Rank: {Rank}",
                'f' => $"FirstName: {Person?.FirstName}",
                'm' => $"MiddleName: {Person?.MiddleName}",
                'l' => $"LastName: {Person?.LastName}",
                'b' => $"BirthDate: {Person?.BirthDate.ToString("yyyy-MM-dd")}",
                _ => ""
            };
            result += args[lastIndex] == arg ? "}" : ", ";
        }

        result += "}";
        return result;
    }
}

public class Officer
{
    public int Id { get; set; }
    public int DisplayId { get; set; }

    [Required]
    public Person Person { get; set; }
    public SeniorRanks Rank { get; set ;}

    public int? DivisionId { get; set; }
    public Division Division { get; set; }

    public override string ToString() => ToString("irfmlb");

    public string ToString(string args)
    {
        var result = "{Officer: {";
        var lastIndex = args.Length - 1;

        foreach (var arg in args)
        {
            result += arg switch
            {
                'i' => $"Id: {DisplayId}",
                'r' => $"Rank: {Rank}",
                'f' => $"FirstName: {Person?.FirstName}",
                'm' => $"MiddleName: {Person?.MiddleName}",
                'l' => $"LastName: {Person?.LastName}",
                'b' => $"BirthDate: {Person?.BirthDate.ToString("yyyy-MM-dd")}",
                _ => ""
            };
            result += args[lastIndex] == arg ? "}" : ", ";
        }

        result += "}";
        return result;
    }
}
