using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.Utils.Classes;

public class ResultToggle<T>
{
    public AddedORRemoved Action { get; set; }
    public T? Value { get; set; }
}
