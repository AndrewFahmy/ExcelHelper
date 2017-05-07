using System.Threading;
using ExcelHelper.Common.Enums;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Common.Interfaces
{
    public interface IChildService
    {
        (MethodResultEnum result, string message) Execute(ExecutionModel model, OptionsModel options, CancellationToken cancelToken);
    }
}