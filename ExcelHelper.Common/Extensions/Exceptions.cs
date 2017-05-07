using System;
using System.Text;

namespace ExcelHelper.Common.Extensions
{
    public static class Exceptions
    {
        public static string ConcatenateExceptionMessages(this Exception ex, StringBuilder builder = null, int level = 0)
        {
            builder = builder ?? new StringBuilder();

            builder.Append($"Level {level} - {ex.Message}");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);

            if (ex.InnerException != null)
                ConcatenateExceptionMessages(ex.InnerException, builder, level + 1);

            return builder.ToString();
        }
    }
}