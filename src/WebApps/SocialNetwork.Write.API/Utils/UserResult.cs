using System.Collections.Generic;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils;

public class UserResult
{
    public bool Succeeded { get; set; }
    public UserModel? User { get; set; } 
    public IEnumerable<string>? Errors { get; set; } = new List<string>();
}