using System.Collections.Generic;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Common.Interfaces
{
    public interface IConnectionsService
    {
        List<ConnectionModel> GetAllConnections();

        void SaveConnections(List<ConnectionModel> connections);

        IList<ListItemModel> GetConnections();
    }
}