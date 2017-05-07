using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using ExcelHelper.Common;
using ExcelHelper.Common.Extensions;
using ExcelHelper.Common.Helpers;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Services.Services
{
    public class OptionsService : IOptionsService
    {
        public OptionsModel GetCurrentOrDefaultOptions()
        {
            var model = new OptionsModel();
            var filePath = Constants.OptionsFile.Replace(@".\", AppDomain.CurrentDomain.BaseDirectory);

            if (!File.Exists(filePath)) return model;

            var originalText = DataFileHelper.GetFileData(filePath);

            model.ConvertFromString(originalText);

            return model;
        }

        public void WriteOptionsToFile(OptionsModel model)
        {
            var filePath = Constants.OptionsFile.Replace(@".\", AppDomain.CurrentDomain.BaseDirectory);

            var data = model.ConvertToString();

            if (string.IsNullOrWhiteSpace(data)) return;

            DataFileHelper.WriteDataFile(filePath, data);
        }

        public IEnumerable<string> GetInstalledFonts()
        {
            return new InstalledFontCollection().Families.Select(font => font.Name);
        }
    }
}