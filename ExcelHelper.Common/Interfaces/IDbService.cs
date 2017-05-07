using ExcelHelper.Common.Enums;

namespace ExcelHelper.Common.Interfaces
{
    public interface IDbService
    {
        IChildService RenderExcelService { get; }
        IChildService UpdateExcelService { get; }
        IChildService UpdateDatabaseService { get; }



        (MethodResultEnum result, string message) ParseQuery(string query);
    }
}