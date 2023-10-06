using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPi.DAL.Entity;

public class User
{
    [Key]
    public int Id { get; set; }


    public string Name { get; set; }

    public int Age { get; set; }


    public string Email { get; set; }

    [JsonIgnore]
    public List<UserRole> UserRoles { get; set; }
}