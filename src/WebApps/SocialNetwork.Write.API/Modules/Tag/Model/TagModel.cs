using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Modules.PostTag.Model;

namespace SocialNetwork.Write.API.Modules.Tag.Model;

public class TagModel: BaseModel
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public bool IsSystem { get; set; }
    
    public ICollection<PostTagModel> PostTags { get; set; } = new List<PostTagModel>();
}