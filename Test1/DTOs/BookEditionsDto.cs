using System.ComponentModel.DataAnnotations;

namespace Test1.DTOs;

public class BookEditionsDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string EditionTitle { get; set; } = string.Empty;
    public string PublishingHouseName { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
}