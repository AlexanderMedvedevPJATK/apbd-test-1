using System.ComponentModel.DataAnnotations;

namespace Test1.DTOs;

public class AddBookWithPublisherDto
{
    [MaxLength(100)]
    public string BookTitle { get; set; } = string.Empty;
    [MaxLength(100)]
    public string EditionTitle { get; set; } = string.Empty;
    public int PublishingHouseId { get; set; }
    public DateTime ReleaseDate { get; set; }
}