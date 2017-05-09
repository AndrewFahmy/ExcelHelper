using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExcelHelper.LocalCommon;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Services;
using ExcelHelper.UserControls;
using ExcelHelper.Windows;

namespace ExcelHelper
{
    public partial class MainWindow
    {
        private readonly ILocalService _service;

        public MainWindow()
        {
            InitializeComponent();
            _service = new LocalService();
        }

        private void BtnShutdown_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExcelRender_OnClick(object sender, RoutedEventArgs e)
        {
            var newTab = new CustomTabItem();
            newTab.SetHeader("Render New Excel");
            newTab.Content = new RenderExcel(_service, this, newTab);
            AddNewTab(newTab);
        }

        private void ExcelUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            var newTab = new CustomTabItem();
            newTab.SetHeader("Update Excel");
            newTab.Content = new UpdateExcel(_service, this, newTab);
            AddNewTab(newTab);
        }

        private void DatabaseChange_OnClick(object sender, RoutedEventArgs e)
        {
            var newTab = new CustomTabItem();
            newTab.SetHeader("Change Database From Excel");
            newTab.Content = new UpdateDatabase(_service, this, newTab);
            AddNewTab(newTab);
        }

        private void LblTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Options_OnClick(object sender, RoutedEventArgs e)
        {
            new Options(_service, this).ShowDialog();
        }

        private void Connections_OnClick(object sender, RoutedEventArgs e)
        {
            new Connections(_service, this).ShowDialog();
        }

        private void AddNewTab(CustomTabItem newTab)
        {
            TabsContainer.Items.Add(newTab);

            if (TabsContainer.Items.Count > 1) return;

            TabsContainer.Visibility = Visibility.Visible;
            TabsContainer.SelectedIndex = 0;
            TabsContainer_OnSelectionChanged(null, null);
        }

        private void SnippetManager_OnClick(object sender, RoutedEventArgs e)
        {
            var snippetWindow = new Snippets(_service, this);
            snippetWindow.Owner = this;

            snippetWindow.Left = Left;
            snippetWindow.Top = Top + 54;

            snippetWindow.Show();
        }

        private void TabsContainer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for(var i = 0; i < TabsContainer.Items.Count; i++)
            {
                var currentTabItem = (CustomTabItem)TabsContainer.Items[i];

                if (i != TabsContainer.SelectedIndex)
                {
                    currentTabItem.IsActive = false;
                    currentTabItem.OnLoseFocus();
                }
                else
                    currentTabItem.IsActive = true;
            }
        }
    }
}