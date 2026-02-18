using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.PostCategory.Model;

public class PostCategoryModel: BaseModel
{
    public required string PostId { get; set; }
    public required string CategoryId { get; set; }
    
    public uint Order { get; set; }
    
    public CategoryModel? Category { get; set; }
    public PostModel? Post { get; set; }
}