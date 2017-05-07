using System.Collections.Generic;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Common.Interfaces
{
    public interface IOptionsService
    {
        OptionsModel GetCurrentOrDefaultOptions();

        void WriteOptionsToFile(OptionsModel model);

        IEnumerable<string> GetInstalledFonts();
    }
}