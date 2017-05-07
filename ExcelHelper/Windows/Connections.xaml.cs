using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;
using ExcelHelper.Extensions;

namespace ExcelHelper.Windows
{
    public partial class Connections
    {
        private readonly ILocalService _service;
        private List<ConnectionModel> _connectionList;

        public Connections(ILocalService service, Window owner)
        {
            _service = service;
            InitializeComponent();

            Owner = owner;

            _connectionList = _service.ConnectionsService.GetAllConnections();
            LvConnections.ItemsSource = _connectionList;
        }

        private bool ValidateControls()
        {
            var result = TbConnectionName.IsValid();

            result = TbConnectionString.IsValid() && result;

            return result;
        }

        private void ClearPage()
        {
            DataContext = new ConnectionModel();
            _connectionList = _service.ConnectionsService.GetAllConnections();
            LvConnections.ItemsSource = _connectionList;

            BtnSave.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnClose.Visibility = Visibility.Visible;
            BtnDelete.Visibility = Visibility.Collapsed;

            TbConnectionString.IsEnabled =
                TbConnectionName.IsEnabled = false;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            ClearPage();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateControls()) return;

            var model = (ConnectionModel) DataContext;


            if (model == null) return;


            if (_connectionList.Any(c => string.Equals(c.ConnectionName, model.OldConnectionName,
                StringComparison.InvariantCultureIgnoreCase)))
            {
                var oldConnection = _connectionList.First(c => string.Equals(c.ConnectionName, model.OldConnectionName,
                    StringComparison.InvariantCultureIgnoreCase));

                oldConnection.ConnectionName = model.ConnectionName;
                oldConnection.ConnectionString = model.ConnectionString;
            }
            else
            {
                _connectionList.Add(model);
            }

            _service.ConnectionsService.SaveConnections(_connectionList);

            ClearPage();
        }

        private void LvConnections_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvConnections.SelectedIndex <= -1) return;


            var model = (ConnectionModel) e.AddedItems[0];

            DataContext = model;

            BtnSave.IsEnabled = true;
            BtnCancel.Visibility = Visibility.Visible;
            BtnClose.Visibility = Visibility.Collapsed;
            BtnDelete.Visibility = Visibility.Visible;

            TbConnectionString.IsEnabled =
                TbConnectionName.IsEnabled = true;
        }

        private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var model = (ConnectionModel) DataContext;

            if (model == null) return;

            if (_connectionList.Any(c => c.ConnectionName == model.OldConnectionName))
                _connectionList.RemoveAll(c => c.ConnectionName == c.OldConnectionName);

            _service.ConnectionsService.SaveConnections(_connectionList);

            ClearPage();
        }

        private void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            DataContext = new ConnectionModel();
            BtnSave.IsEnabled = true;
            BtnCancel.Visibility = Visibility.Visible;
            BtnClose.Visibility = Visibility.Collapsed;
            BtnDelete.Visibility = Visibility.Collapsed;
            LvConnections.SelectedIndex = -1;

            TbConnectionString.IsEnabled =
                TbConnectionName.IsEnabled = true;
        }

        private void TextBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox) sender).ResetValidation();
        }
    }
}