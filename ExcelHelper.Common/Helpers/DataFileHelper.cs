using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Management;

namespace ExcelHelper.Common.Helpers
{
    public class DataFileHelper
    {
        private static byte[] _entropy;

        private static byte[] GetEntropy()
        {
            try
            {
                if (_entropy != null) return _entropy;

                var searchObject = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                var list = searchObject.Get();

                var processorId = list.Cast<ManagementObject>().Select(item => item["ProcessorId"].ToString())
                    .FirstOrDefault();

                _entropy = Encoding.UTF7.GetBytes(processorId ?? "ExcelHelper_2017");

                return _entropy;
            }
            catch
            {
                return Encoding.UTF7.GetBytes("ExcelHelper_2017");
            }
        }


        public static string GetFileData(string filePath)
        {
            try
            {
                var encodedData = ProtectedData.Unprotect(
                    File.ReadAllBytes(filePath), GetEntropy(), DataProtectionScope.LocalMachine);

                return Encoding.UTF7.GetString(encodedData);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void WriteDataFile(string filePath, string data)
        {
            var encodedData = Encoding.UTF7.GetBytes(data);

            var encryptedData = ProtectedData.Protect(encodedData, GetEntropy(), DataProtectionScope.LocalMachine);

            File.WriteAllBytes(filePath, encryptedData);
        }
    }
}