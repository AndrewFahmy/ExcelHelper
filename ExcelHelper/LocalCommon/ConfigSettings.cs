using System;
using System.Configuration;
using ExcelHelper.Common.Models;

namespace ExcelHelper.LocalCommon
{
    internal class ConfigSettings
    {
        internal static string GetAppSettingKey(string key)
        {
            ConfigurationManager.RefreshSection("AppSettings");

            return ConfigurationManager.AppSettings[key];
        }

        internal static void AddNewConnection(ConnectionModel model)
        {
            var config =
                ConfigurationManager.OpenExeConfiguration($"{AppDomain.CurrentDomain.BaseDirectory}Excel Helper.exe");

            var connectionStringsSection = (ConnectionStringsSection) config.GetSection("connectionStrings");

            var collection = connectionStringsSection.ConnectionStrings;

            collection.Add(new ConnectionStringSettings
            {
                Name = model.ConnectionName,
                ConnectionString = model.ConnectionString
            });

            collection.Remove("LocalSqlServer");

            config.Save(ConfigurationSaveMode.Full);

            ConfigurationManager.RefreshSection("connectionStrings");
        }

        internal static void UpdateConnection(ConnectionModel model)
        {
            var config =
                ConfigurationManager.OpenExeConfiguration($"{AppDomain.CurrentDomain.BaseDirectory}Excel Helper.exe");

            var connectionStringsSection = (ConnectionStringsSection) config.GetSection("connectionStrings");

            var collection = connectionStringsSection.ConnectionStrings;

            var connection = collection[model.OldConnectionName];

            connection.ConnectionString = model.ConnectionString;
            connection.Name = model.ConnectionName;

            collection.Remove("LocalSqlServer");

            config.Save(ConfigurationSaveMode.Full);

            ConfigurationManager.RefreshSection("connectionStrings");
        }

        internal static void DeleteConnection(ConnectionModel model)
        {
            var config =
                ConfigurationManager.OpenExeConfiguration($"{AppDomain.CurrentDomain.BaseDirectory}Excel Helper.exe");

            var connectionStringsSection = (ConnectionStringsSection) config.GetSection("connectionStrings");

            var collection = connectionStringsSection.ConnectionStrings;

            collection.Remove(model.ConnectionName);

            collection.Remove("LocalSqlServer");

            config.Save(ConfigurationSaveMode.Full);

            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}