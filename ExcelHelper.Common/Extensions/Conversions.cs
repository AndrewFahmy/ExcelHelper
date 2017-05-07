using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExcelHelper.Common.Helpers;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Common.Extensions
{
    public static class Conversions
    {
        public static string ConvertToString(this IFileConvertable model)
        {
            var data = new StringBuilder();

            var modelType = model.GetType();

            var modelProperties = modelType.GetProperties();

            foreach (var modelProperty in modelProperties)
            {
                if (modelProperty.Name == "OldConnectionName") continue;

                data.Append(modelProperty.Name);
                data.Append("/*valueSep*/");
                data.Append(modelProperty.GetValue(model, null));
                data.Append("/*propSep*/");
            }

            return data.ToString();
        }


        public static void ConvertFromString(this IFileConvertable model, string data)
        {
            var modelType = model.GetType();

            var splittedData = data.Split(new[] {"/*propSep*/"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var propertyData in splittedData)
            {
                var keyValuePair = propertyData.Split(new[] {"/*valueSep*/"}, StringSplitOptions.RemoveEmptyEntries);

                if (keyValuePair.Length < 2) continue;

                var property = modelType.GetProperty(keyValuePair[0]);

                if (keyValuePair[0] == "ConnectionName")
                {
                    var oldConnectionProperty = modelType.GetProperty("OldConnectionName");
                    oldConnectionProperty?.SetMethod.Invoke(model, new object[] {keyValuePair[1]});
                }

                property?.SetMethod.Invoke(model, new[] {Convert.ChangeType(keyValuePair[1], property.PropertyType)});
            }
        }

        public static string PrepareQuery(this ExecutionModel model, ExcelHandler handler)
        {
            var usedRows = handler.UsedRows();
            var query = new StringBuilder();

            foreach (var row in usedRows)
            {
                var columns = Regex.Matches(model.Query, Constants.QueryColumnParameter, RegexOptions.IgnoreCase)
                    .Cast<Match>().Select(m => m.Value);

                var queryTemp = columns.Aggregate(model.Query, (current, column) =>
                {
                    var cellValue = handler.GetCellValue(row, int.Parse(column.Replace("$", string.Empty).Trim()));

                    if (DateTime.TryParse(cellValue, out DateTime result))
                        return current.Replace(column, result.ToString("yyyy-MM-dd HH:m:s tt"));

                    return  current.Replace(column, cellValue);
                });

                query.Append(queryTemp);
            }

            return query.ToString();
        }
    }
}