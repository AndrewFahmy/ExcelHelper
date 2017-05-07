using ExcelHelper.Common.Interfaces;

namespace ExcelHelper.Common.Models
{
    public class ConnectionModel : IFileConvertable
    {
        public string ConnectionName { get; set; }

        public string OldConnectionName { get; set; }

        public string ConnectionString { get; set; }


        public override string ToString()
        {
            return ConnectionName;
        }
    }
}