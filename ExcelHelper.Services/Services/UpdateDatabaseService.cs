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
    public class UpdateDatabaseService : IChildService
    {
        private readonly DbContext _context;

        public UpdateDatabaseService(DbContext context)
        {
            _context = context;
        }

        public (MethodResultEnum result, string message) Execute(ExecutionModel model, OptionsModel options, CancellationToken cancelToken)
        {
            try
            {
                var sheetNames = Regex.Match(model.Query, Constants.SheetNamesPattern, RegexOptions.IgnoreCase);

                if (!sheetNames.Success) return (MethodResultEnum.Fail, "Please Specify Sheet Name");

                using (var handler = new ExcelHandler(model, sheetNames.Value.Split(','), options))
                {
                    if (handler.UsedRowsCount() <= 0)
                        return (MethodResultEnum.Fail, "There are now rows in this sheet");

                    var query = model.PrepareQuery(handler);

                    _context.Execute(query, CommandType.Text);
                }

                return (MethodResultEnum.Success, "Database was updated successfuly");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.ConcatenateExceptionMessages();

                return (MethodResultEnum.Fail, errorMessage);
            }
        }
    }
}