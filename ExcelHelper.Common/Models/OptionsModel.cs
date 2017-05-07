using ExcelHelper.Common.Interfaces;

namespace ExcelHelper.Common.Models
{
    public class OptionsModel : IFileConvertable
    {
        //Header Properties
        public byte HeaderBgRed { get; set; } = 146;

        public byte HeaderBgGreen { get; set; } = 208;

        public byte HeaderBgBlue { get; set; } = 80;

        public byte HeaderFgRed { get; set; } = 255;

        public byte HeaderFgGreen { get; set; } = 255;

        public byte HeaderFgBlue { get; set; } = 255;

        public string HeaderFontName { get; set; } = "Times New Roman";

        public double HeaderFontSize { get; set; } = 12;

        public bool HeaderIsBold { get; set; }

        public bool HeaderIsItalic { get; set; } = true;


        //Row Properties

        public byte RowBgRed { get; set; } = 255;

        public byte RowBgGreen { get; set; } = 255;

        public byte RowBgBlue { get; set; } = 255;

        public byte RowFgRed { get; set; }

        public byte RowFgGreen { get; set; }

        public byte RowFgBlue { get; set; }

        public string RowFontName { get; set; } = "Calibri";

        public double RowFontSize { get; set; } = 12;

        public bool RowIsBold { get; set; }

        public bool RowIsItalic { get; set; }
    }
}