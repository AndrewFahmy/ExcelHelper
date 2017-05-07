using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExcelHelper.Common.Extensions
{
    public class StringValidations
    {
        public static string ValidateSheetName(string sheetName)
        {
            var cleanSheetName = Regex.Replace(sheetName, @"(\\|\/|\*|\[|\] |\:|\?)", string.Empty, RegexOptions.IgnoreCase);

            return cleanSheetName.Length > 31 ? cleanSheetName.Substring(0, 30) : cleanSheetName;
        }

        public static string ValidateFileName(string filePath)
        {
            var invalidWindowsChars = Path.GetInvalidFileNameChars();

            var fileName = Path.GetFileName(filePath);

            if (string.IsNullOrWhiteSpace(fileName)) return filePath;

            filePath = filePath.Replace(fileName, string.Empty);

            var validName = new string(fileName.Where(c => !invalidWindowsChars.Contains(c)).ToArray());

            return $"{filePath}{validName}";
        }
    }
}