namespace SocialNetwork.Write.API.Configs.Exception.classes;

public class UnauthenticatedException : System.Exception
{
    public int HttpStatusCode { get; } = 401;
    public string ErrorCode { get; } = "USER_NOT_LOGGED_IN";

    public UnauthenticatedException(string message = "You need to be logged in to perform this action."): base(message) { }
}