using ExcelHelper.Common.Interfaces;
using ExcelHelper.Services.Services;

namespace ExcelHelper.Services
{
    public class LocalService : ILocalService
    {
        public IConnectionsService ConnectionsService => new ConnectionsService();
        public IOptionsService OptionsService => new OptionsService();
        public ISnippetService SnippetsService => new SnippetsService();
    }
}