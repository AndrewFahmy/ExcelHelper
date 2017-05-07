using System;
using System.Text.RegularExpressions;
using System.Windows;
using ExcelHelper.Common;
using ExcelHelper.Common.Interfaces;
using mshtml;

namespace ExcelHelper.Windows
{
    public partial class Snippets
    {
        private readonly ILocalService _localService;
        private readonly Window _parentWindow;

        public Snippets(ILocalService localService, Window parentWindow)
        {
            _localService = localService;
            _parentWindow = parentWindow;
            InitializeComponent();

            WbQuery.Navigate(new Uri(
                $"file:///{AppDomain.CurrentDomain.BaseDirectory}Resources/SnippetPage.html"));
            WbQuery.Focus();

            parentWindow.LocationChanged += MainWindow_LocationChanged;
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            Left = _parentWindow.Left;
            Top = _parentWindow.Top + 54;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            var snippetsData = Regex.Match(((IHTMLDocument2)WbQuery.Document).body.innerHTML,
                Constants.QueryValuePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            _localService.SnippetsService.UpdateSnippetsFile(snippetsData);

            Close();
        }
    }
}