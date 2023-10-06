using System.ComponentModel.DataAnnotations;

namespace WebAPi.DAL.DTO;

public class UserDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Range(1, 150)]
    public int Age { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
}