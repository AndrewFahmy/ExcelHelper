using System.Data.SqlClient;
using SqlMapper;

namespace ExcelHelper.Services.Context
{
    public class DbContext : ContextBase<SqlConnection>
    {
        public DbContext(string connectionString) : base(connectionString)
        {
        }
    }
}