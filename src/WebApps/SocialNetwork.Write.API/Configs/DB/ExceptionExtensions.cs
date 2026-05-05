namespace SocialNetwork.Write.API.Configs.DB;

public static partial class ExceptionExtensions
{
    public static bool IsDuplicateEntry(this System.Exception ex) => DbErrorHelper.IsUniqueConstraintViolation(ex);
    
    public static bool IsForeignKeyViolation(this System.Exception ex) => DbErrorHelper.IsForeignKeyViolation(ex);
    
    public static bool IsDataTooLong(this System.Exception ex) => DbErrorHelper.IsDataTooLong(ex);
    
}