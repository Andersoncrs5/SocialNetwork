using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Write.API.Configs.DB;

public static class DbErrorHelper
{
    private const int DuplicateEntryCode = 1062;
    private const int ForeignKeyNoParentCode = 1452; 
    private const int ForeignKeyHasChildrenCode = 1451;
    private const int DataTooLongCode = 1406;

    public static bool IsUniqueConstraintViolation(System.Exception ex) => GetErrorCode(ex) == DuplicateEntryCode;
    
    public static bool IsForeignKeyViolation(System.Exception ex) 
    {
        var code = GetErrorCode(ex);
        return code == ForeignKeyNoParentCode || code == ForeignKeyHasChildrenCode;
    }

    public static bool IsDataTooLong(System.Exception ex) => GetErrorCode(ex) == DataTooLongCode;

    private static int GetErrorCode(System.Exception ex)
    {
        if (ex is DbUpdateException dbEx && dbEx.InnerException != null)
        {
            var inner = dbEx.InnerException;
            var numberProperty = inner.GetType().GetProperty("Number");
            if (numberProperty != null)
            {
                return (int)numberProperty.GetValue(inner)!;
            }
        }
        return -1;
    }
}