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
    public class UpdateExcelService : IChildService
    {
        private readonly DbContext _context;

        public UpdateExcelService(DbContext context)
        {
            _context = context;
        }

        public (MethodResultEnum result, string message) Execute(ExecutionModel model, OptionsModel options, CancellationToken cancelToken)
        {
            var sheetNames = Regex.Match(model.Query, Constants.SheetNamesPattern, RegexOptions.IgnoreCase);

            if (!sheetNames.Success) return (MethodResultEnum.Fail, "Please Specify Sheet Name");

            using (var handler = new ExcelHandler(model, sheetNames.Value.Split(','), options))
            {
                try
                {
                    if (handler.UsedRowsCount() <= 0)
                        return (MethodResultEnum.Fail, "There are now rows in this sheet");


                    var query = model.PrepareQuery(handler);

                    var rowIndex = model.HasHeader ? 2 : 1;
                    var isFirstRow = true;
                    foreach (var row in _context.GetData(query, CommandType.Text))
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            handler.SaveFile();
                            return (MethodResultEnum.Cancelled, "Operation was cancelled");
                        }

                        handler.UpdateExcelData(row, rowIndex, isFirstRow);
                        isFirstRow = false;
                        rowIndex++;
                    }

                    if (model.AddQueryToFile) handler.AddQueryToExcel(query);

                    handler.SaveFile();

                    return (MethodResultEnum.Success,
                        $"File was updated succesfully.{Environment.NewLine}{Environment.NewLine}Location: {model.FileName}"
                        );
                }
                catch (Exception ex)
                {
                    var erroMessage = ex.ConcatenateExceptionMessages();

                    return (MethodResultEnum.Fail, erroMessage);
                }
            }
        }
    }
}