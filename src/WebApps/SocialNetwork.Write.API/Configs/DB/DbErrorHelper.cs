using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Write.API.Configs.DB;

public static class DbErrorHelper
{
    private const int DuplicateEntryCode = 1062;
    private const int ForeignKeyNoParentCode = 1452; 
    private const int ForeignKeyHasChildrenCode = 1451;
    private const int DataTooLongCode = 1406;
    
    public static bool IsForeignKeyViolation(System.Exception ex)
    {
        /*
         * var code = GetErrorCode(ex);
         * return code == ForeignKeyNoParentCode || code == ForeignKeyHasChildrenCode;
         */
        
        var code = GetErrorCode(ex);

        if (code == ForeignKeyNoParentCode || code == ForeignKeyHasChildrenCode)
            return true;

        System.Exception? current = ex;
        while (current?.InnerException != null)
            current = current.InnerException;

        var message = current?.Message ?? ex.Message;

        return message.Contains("FOREIGN KEY constraint fails", StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool IsUniqueConstraintViolation(System.Exception ex) => GetErrorCode(ex) == DuplicateEntryCode;
    
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