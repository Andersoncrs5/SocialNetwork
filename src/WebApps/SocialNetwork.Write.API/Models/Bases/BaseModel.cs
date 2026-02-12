using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Models.Bases;

public class BaseModel
{
    [MaxLength(450)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [ConcurrencyCheck] public long Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}