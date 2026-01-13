using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Models.Bases;

public class BaseModel
{
    [MaxLength(450)] public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}