using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using ExcelHelper.Common;
using ExcelHelper.Common.Enums;
using ExcelHelper.Common.Extensions;
using ExcelHelper.Common.Helpers;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;
using ExcelHelper.Services.Context;

namespace ExcelHelper.Services.Services
{
    public class RenderExcelService : IChildService
    {
        private readonly DbContext _context;

        public RenderExcelService(DbContext context)
        {
            _context = context;
        }

        public (MethodResultEnum result, string message) Execute(ExecutionModel model, OptionsModel options, CancellationToken cancelToken)
        {
            try
            {
                var sheetNames = Regex.Match(model.Query, Constants.SheetNamesPattern, RegexOptions.IgnoreCase);

                using (var handler = new ExcelHandler(model,
                    sheetNames.Success ? sheetNames.Value.Split(',') : new string[0], options))
                {
                    var isHeader = true;

                    foreach (var row in _context.GetData(model.Query, CommandType.Text))
                    {
                        if (cancelToken.IsCancellationRequested) return (MethodResultEnum.Cancelled, "Opertation was cancelled");
                        handler.WriteDataToExcel(row, isHeader);
                        isHeader = false;
                    }

                    if (model.AddQueryToFile) handler.AddQueryToExcel(model.Query);

                    handler.SaveFile();
                }

                return (MethodResultEnum.Success,
                    $"File was created successfully. {Environment.NewLine}{Environment.NewLine}Location: {model.FileName}");
            }
            catch (Exception ex)
            {
                return (MethodResultEnum.Fail, ex.ConcatenateExceptionMessages());
            }
        }
    }
}