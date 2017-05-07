using System;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using ExcelHelper.Common.Extensions;
using ExcelHelper.Common.Models;
using SqlMapper.Common.Models;

namespace ExcelHelper.Common.Helpers
{
    public class ExcelHandler : IDisposable
    {
        private IXLWorksheet _currentSheet;
        private int _lastResultIndex = -1;
        private string[] _sheets;
        private ExcelWriter _writer;

        public ExcelHandler(ExecutionModel model, string[] sheets, OptionsModel options)
        {
            model.FileName = StringValidations.ValidateFileName(model.FileName);
            _writer = new ExcelWriter(model, options);
            _sheets = sheets;
        }

        public void Dispose()
        {
            _sheets = null;
            _writer = null;
            _lastResultIndex = -1;
            _currentSheet = null;
        }

        public void WriteDataToExcel(CommandResult row, bool isHeader)
        {
            OpenSheet(row);

            if (isHeader)
                _writer.AddTableHeader(_currentSheet, row.Columns.Keys);

            _writer.AppendRow(_currentSheet, row.Columns.Values);
        }

        public void UpdateExcelData(CommandResult row, int rowIndex, bool isFirstRow)
        {
            OpenSheet(row);

            if (isFirstRow)
                _writer.AppendCellsToHeader(_currentSheet, row.Columns.Keys);

            var excelRow = _currentSheet?.Row(rowIndex);

            foreach (var value in row.Columns.Values)
                _writer.AppendCell(excelRow, value.ToString());
        }

        public void AddQueryToExcel(string query)
        {
            var querySheet = _writer.GetOrAddWorksheet("Query");

            var queryWithoutSheetNames = Regex.Replace(query, @"--\[\[sheets:.*\]\]", string.Empty,
                RegexOptions.IgnoreCase);

            _writer.AppendRow(querySheet, new[] { queryWithoutSheetNames });

            querySheet.Columns().AdjustToContents();
        }

        public void SaveFile()
        {
            _currentSheet?.Columns().AdjustToContents();
            _writer.CommitChanges();
        }

        public IXLRows UsedRows()
        {
            OpenSheet(_sheets[0]);

            return _writer.GetUsedRows(_currentSheet);
        }

        public int UsedRowsCount() => UsedRows().Count();

        public string GetCellValue(IXLRow row, int cellValue)
        {
            return _writer.GetCellValue(row, cellValue);
        }



        private void OpenSheet(CommandResult row)
        {
            if (_lastResultIndex != row.ResultSetIndex)
            {
                var sheetName = _sheets.Length > row.ResultSetIndex
                    ? _sheets[row.ResultSetIndex]
                    : $"Sheet{row.ResultSetIndex + 1}";

                _currentSheet = _writer.GetOrAddWorksheet(sheetName);
                _lastResultIndex = row.ResultSetIndex;
            }
        }

        private void OpenSheet(string sheetName)
        {
            _currentSheet = _writer.GetOrAddWorksheet(sheetName);
            _lastResultIndex = 0;
        }
    }
}