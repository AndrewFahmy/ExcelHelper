using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExcelHelper.Common;
using ExcelHelper.Common.Extensions;
using ExcelHelper.Common.Helpers;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;

namespace ExcelHelper.Services.Services
{
    public class ConnectionsService : IConnectionsService
    {
        public List<ConnectionModel> GetAllConnections()
        {
            var list = new List<ConnectionModel>();
            var filePath = Constants.ConnectionsFile.Replace(@".\", AppDomain.CurrentDomain.BaseDirectory);


            if (!File.Exists(filePath)) return list;

            var originalText = DataFileHelper.GetFileData(filePath);

            if (string.IsNullOrWhiteSpace(originalText)) return list;

            var connections = originalText.Split(new[] { "/*modelSep*/" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var connection in connections)
            {
                var model = new ConnectionModel();

                model.ConvertFromString(connection);

                list.Add(model);
            }

            return list;
        }

        public void SaveConnections(List<ConnectionModel> connections)
        {
            var filePath = Constants.ConnectionsFile.Replace(@".\", AppDomain.CurrentDomain.BaseDirectory);

            var data = new StringBuilder();

            foreach (var model in connections)
            {
                data.Append(model.ConvertToString());
                data.Append("/*modelSep*/");
            }

            DataFileHelper.WriteDataFile(filePath, data.ToString());
        }

        public IList<ListItemModel> GetConnections()
        {
            var returnList = new List<ListItemModel> { new ListItemModel("Choose Connection", null) };

            var connections = GetAllConnections();

            returnList.AddRange(connections.Select(connection => new ListItemModel($"{connection.ConnectionName} Connection",
                                connection.ConnectionString.Replace(Environment.NewLine, string.Empty))));

            return returnList;
        }
    }
}