using System;
using System.IO;
using System.Text.RegularExpressions;
using ExcelHelper.Common;
using ExcelHelper.Common.Interfaces;

namespace ExcelHelper.Services.Services
{
    public class SnippetsService : ISnippetService
    {
        public void UpdateSnippetsFile(string snippets)
        {
            var newLineReplaced = Regex.Replace(snippets, @"\n", $@"\n\{Environment.NewLine}");

            var finalText = Regex.Replace(newLineReplaced, @"\s{4}", "\t");

            var fileData = Regex.Replace(Constants.SnippetsFile, "<-snippets->", finalText);

            File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}Resources/snippets-sqlserver.js", fileData);
        }
    }
}