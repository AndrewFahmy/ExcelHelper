using System;

namespace ExcelHelper.Common
{
    public static class Constants
    {
        public const string ExcelFileFilter = "Excel WorkBook (*.xlsx)|*.xlsx";

        public const string QueryValuePattern = "(?<=<textarea id=\"tb_queryText\" .*?>)(.*)(?=</textarea>)";

        public const string SheetNamesPattern = @"(?<=--\[\[sheets:)(.*)(?=]])";

        public const string QueryColumnParameter = @"\$\d*";

        public const string OptionsFile = @".\Options.dat";

        public const string ConnectionsFile = @".\Connections.dat";

        public const string RenderMessage = "--Press F4 to enter sheet names in CSV format";

        public const string UpdateExcelAndDatabaseMessage = "--Please press F4 to enter the sheet name";

        public const int MaxRowsPerSave = 10000;

        public const string SnippetsFile = @"define(""ace/snippets/sqlserver"",[""require"",""exports"",""module""], function(require, exports, module) {
""use strict"";

exports.snippetText = ""<-snippets->"";
exports.scope = ""sqlserver"";
});";

    }

    [Flags]
    public enum DialogButtons
    {
        Ok = 1,
        Yes = 2,
        No = 4,
        Cancel = 8
    }
}