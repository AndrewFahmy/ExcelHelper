using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Common.Helpers
{
    internal class ExcelWriter
    {
        private readonly ExecutionModel _model;
        private readonly Regex _invalidXmlCharacters;
        private XLWorkbook _excelFile;
        private readonly OptionsModel _options;
        private int _rowCount;

        public ExcelWriter(ExecutionModel model, OptionsModel options)
        {
            _model = model;
            _options = options;
            _invalidXmlCharacters = new Regex(@"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]", RegexOptions.Compiled);
            _excelFile = CreateOrOpenFile();
        }

        public void CommitChanges()
        {
            if (_model.FileExists)
                _excelFile.Save();
            else
                _excelFile.SaveAs(_model.FileName);

            _excelFile.Dispose();
            _excelFile = null;
        }

        public IXLWorksheet GetOrAddWorksheet(string sheetName)
        {
            if (_excelFile.Worksheets.Any(sheet => sheet.Name.ToLowerInvariant() == sheetName.ToLowerInvariant()))
                return _excelFile.Worksheet(sheetName);

            var newSheet = _excelFile.AddWorksheet(sheetName);

            return newSheet;
        }

        private void FormatRow(IXLRow row, bool isHeader = false)
        {
            row.Style.Fill.SetBackgroundColor(isHeader ? XLColor.FromArgb(_options.HeaderBgRed, _options.HeaderBgGreen, _options.HeaderBgBlue) :
                                                         XLColor.FromArgb(_options.RowBgRed, _options.RowBgGreen, _options.RowBgBlue));

            row.Style.Font.SetFontColor(isHeader ? XLColor.FromArgb(_options.HeaderFgRed, _options.HeaderFgGreen, _options.HeaderFgBlue) :
                                                   XLColor.FromArgb(_options.RowFgRed, _options.RowFgGreen, _options.RowFgBlue));

            row.Style.Font.Bold = isHeader ? _options.HeaderIsBold : _options.RowIsBold;

            row.Style.Font.FontSize = isHeader ? _options.HeaderFontSize : _options.RowFontSize;

            row.Style.Font.Italic = isHeader ? _options.HeaderIsItalic : _options.RowIsItalic;

            row.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            row.Style.Font.FontName = isHeader ? _options.HeaderFontName : _options.RowFontName;

            if (isHeader)
            {
                row.Height = 22;
                row.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }

            row.Style.Border.TopBorder =
                row.Style.Border.RightBorder =
                    row.Style.Border.BottomBorder =
                        row.Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            row.Style.Border.TopBorderColor =
                row.Style.Border.RightBorderColor =
                    row.Style.Border.BottomBorderColor =
                        row.Style.Border.LeftBorderColor = XLColor.LightGray;

        }

        public void AddTableHeader(IXLWorksheet currentSheet, IEnumerable<object> columns)
        {
            if (currentSheet == null) return;

            var lastUsedRow = currentSheet.LastRowUsed();

            var availableRowIndex = lastUsedRow?.RowNumber() ?? 1;

            //if this is not the first header in the sheet add a separator row
            if (availableRowIndex != 1) availableRowIndex++;

            var headerRow = currentSheet.Row(availableRowIndex);

            FormatRow(headerRow, true);

            foreach (var column in columns)
                AppendCell(headerRow, column.ToString(), true);

            currentSheet.Columns().AdjustToContents();
        }

        public void AppendCellsToHeader(IXLWorksheet currentSheet, IEnumerable<object> columns)
        {
            var headerRow = currentSheet?.RowsUsed(row => row.Style.Font.FontName == _options.HeaderFontName).LastOrDefault();

            if (headerRow == null) return;

            FormatRow(headerRow, true);

            foreach (var column in columns)
                AppendCell(headerRow, column.ToString(), true);
        }

        public void AppendRow(IXLWorksheet currentSheet, IEnumerable<object> values)
        {
            if (currentSheet == null) return;

            var availableRowIndex = (currentSheet.LastRowUsed()?.RowNumber() ?? 0) + 1;
            var selectedRow = currentSheet.Row(availableRowIndex);

            FormatRow(selectedRow);

            foreach (var value in values)
                AppendCell(selectedRow, value.ToString());

            _rowCount++;

            if (_rowCount >= Constants.MaxRowsPerSave)
                SaveAndReopen(currentSheet);
        }

        public void AppendCell(IXLRow row, string value, bool isHeaderCell = false)
        {
            if (row == null) return;

            var availableCellIndex = (row.LastCellUsed()?.Address?.ColumnNumber ?? 0) + 1;
            var selectedCell = row.Cell(availableCellIndex);
            AddSheetCell(selectedCell, value, isHeaderCell);
        }

        public void AddSheetCell(IXLCell cell, string value, bool isHeaderCell = false)
        {
            if (cell == null) return;

            var htmlMatch = Regex.Match(value, "(>).*?(<)", RegexOptions.CultureInvariant);

            if (htmlMatch.Success)
                value = Regex.Replace(htmlMatch.Value, ">|<", string.Empty);

            value = _invalidXmlCharacters.Replace(value, string.Empty);

            if (string.IsNullOrWhiteSpace(value))
            {
                cell.SetValue("  ");
                cell.SetDataType(XLCellValues.Text);
            }
            else if (isHeaderCell)
            {
                cell.SetValue(value);
                cell.SetDataType(XLCellValues.Text);
            }
            else if (DateTime.TryParse(value, out DateTime result))
            {
                cell.Value = result.ToString("yyyy-MM-dd HH:mm:ss");
                cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
            }
            else
                cell.Value = value;

            if (!isHeaderCell)
                cell.Style.Alignment.SetWrapText(true);
        }

        public IXLRows GetUsedRows(IXLWorksheet currentSheet)
        {
            if (_model.HasHeader)
                return currentSheet.RowsUsed(row => row.RowNumber() != 1);

            return currentSheet.RowsUsed();
        }

        public string GetCellValue(IXLRow currentRow, int cellIndex)
        {
            return currentRow.Cell(cellIndex).GetString();
        }



        private XLWorkbook CreateOrOpenFile()
        {
            return _model.FileExists ? new XLWorkbook(_model.FileName, XLEventTracking.Disabled) : new XLWorkbook(XLEventTracking.Disabled);
        }

        private void SaveAndReopen(IXLWorksheet currentSheet)
        {
            currentSheet.Columns().AdjustToContents();
            currentSheet.Dispose();
            CommitChanges();
            _excelFile = CreateOrOpenFile();
            _rowCount = 0;
        }
    }
}