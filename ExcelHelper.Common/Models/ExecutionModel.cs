using System.IO;

namespace ExcelHelper.Common.Models
{
    public class ExecutionModel
    {
        public string Query { get; set; }

        public bool AddQueryToFile { get; set; }

        public bool HasHeader { get; set; }

        public string FileName { get; set; }

        public bool FileExists => File.Exists(FileName);
    }
}