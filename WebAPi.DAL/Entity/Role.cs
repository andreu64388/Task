using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPi.DAL.Enum;

namespace WebAPi.DAL.Entity;

public class Role
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public List<UserRole> UserRoles { get; set; }
}