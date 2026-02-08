using SocialNetwork.Write.API.Models.Bases;

namespace SocialNetwork.Write.API.Models;

public class PostCategoryModel: BaseModel
{
    public required string PostId { get; set; }
    public required string CategoryId { get; set; }
    
    public uint Order { get; set; }
    
    public CategoryModel? Category { get; set; }
    public PostModel? Post { get; set; }
}