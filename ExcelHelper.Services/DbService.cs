using System;
using System.Data;
using ExcelHelper.Common.Enums;
using ExcelHelper.Common.Extensions;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Services.Context;
using ExcelHelper.Services.Services;

namespace ExcelHelper.Services
{
    public class DbService : IDbService
    {
        private readonly DbContext _context;
        
        public DbService(string connectionString)
        {
            _context = new DbContext(connectionString);
        }


        public IChildService RenderExcelService => new RenderExcelService(_context);
        public IChildService UpdateExcelService => new UpdateExcelService(_context);
        public IChildService UpdateDatabaseService => new UpdateDatabaseService(_context);


        public (MethodResultEnum result, string message) ParseQuery(string query)
        {
            try
            {
                _context.Execute($"Set FmtOnly On; {query} Set FmtOnly Off;", CommandType.Text);

                return (MethodResultEnum.Success, "Query was parsed successfuly");
            }
            catch (Exception ex)
            {
                return (MethodResultEnum.Fail, ex.ConcatenateExceptionMessages());
            }
        }
    }
}