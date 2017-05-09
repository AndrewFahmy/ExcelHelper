using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ExcelHelper.Common;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;
using ExcelHelper.Extensions;
using ExcelHelper.LocalCommon;
using ExcelHelper.Services;
using mshtml;
using Microsoft.Win32;
using ExcelHelper.Windows;

namespace ExcelHelper.UserControls
{
    public partial class UpdateDatabase
    {
        private readonly ILocalService _localService;
        private readonly Window _owner;
        private readonly CustomTabItem _parentTab;
        private readonly OpenFileDialog _openFileDialog;
        private Dialog _currentDialog;
        private CancellationTokenSource _tokenSource;

        public UpdateDatabase(ILocalService localService, Window owner, CustomTabItem parentTab)
        {
            _localService = localService;
            _owner = owner;
            _parentTab = parentTab;

            InitializeComponent();
            
            _openFileDialog = new OpenFileDialog {Filter = Constants.ExcelFileFilter};
            _openFileDialog.FileOk += OpenFileDialog_FileOk;

            CbConnections.ItemsSource = _localService.ConnectionsService.GetConnections();

            WbQuery.Navigate(new Uri(
                $"file:///{AppDomain.CurrentDomain.BaseDirectory}Resources/QueryPage.html?m={Constants.UpdateExcelAndDatabaseMessage}"));
            WbQuery.Focus();

            _owner.LocationChanged += MainWindow_LocationChanged;
            _parentTab.OnLoseFocus += ParentTab_LoseFocus;
        }

        private bool ValidateControls()
        {
            var result = CbConnections.IsValid();

            result = TbLocation.IsValid() && result;

            return result;
        }

        private void OpenPopup(Dialog dialog)
        {
            _currentDialog = dialog;
            dialog.Left = _owner.Left;
            dialog.Top = _owner.Top + 78;
            dialog.Owner = _owner;
            _currentDialog.OnActionClick = button => ResetWindow();

            if (_parentTab.IsActive)
                dialog.Show();
        }

        private void ResetWindow()
        {
            LoadingImage.Visibility = Visibility.Hidden;
            BtnUpdate.IsEnabled = BtnCancel.IsEnabled = true;
            _tokenSource = null;
            _currentDialog.Close();
            _currentDialog = null;
            LblTimer.Content = "00:00:00";
            LblTimer.Visibility = Visibility.Hidden;
        }

        private void ShowLoadingIconAndTimerLabel()
        {
            LoadingImage.Visibility = Visibility.Visible;
            LblTimer.Visibility = Visibility.Visible;
            BtnUpdate.IsEnabled = false;
        }

        private DispatcherTimer StartTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);

            var startTime = DateTime.Now;

            timer.Tick += (sndr, evt) =>
            {
                var timeDiff = DateTime.Now - startTime;

                LblTimer.Dispatcher.Invoke(() => LblTimer.Content =
                    $"{timeDiff.Hours:D2}:{timeDiff.Minutes:D2}:{timeDiff.Seconds:D2}");
            };

            timer.Start();

            return timer;
        }

        private void OpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TbLocation.Text = _openFileDialog.FileName;
        }

        private void UpdateDatabase_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_currentDialog != null)
                OpenPopup(_currentDialog);
        }

        private void CbConnections_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CbConnections.ResetValidation();
        }

        private void FileBrowse_OnClick(object sender, EventArgs e)
        {
            TbLocation.ResetValidation();
            _openFileDialog.ShowDialog();
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateControls()) return;

            _tokenSource = new CancellationTokenSource();

            var query = Regex.Match(((IHTMLDocument2) WbQuery.Document).body.innerHTML,
                Constants.QueryValuePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            if (!string.IsNullOrWhiteSpace(query))
            {
                ShowLoadingIconAndTimerLabel();

                WbQuery.Focus();

                var connectionString = CbConnections.SelectedValue.ToString();

                var model = new ExecutionModel
                {
                    HasHeader = ChkHasHeader.IsChecked.GetValueOrDefault(),
                    FileName = TbLocation.Text,
                    Query = query
                };

                var timer = StartTimer();

                var resultUpdate = await Task.Factory.StartNew(() =>
                {
                    IDbService dbService = new DbService(connectionString);

                    var result = dbService.UpdateDatabaseService.Execute(model,
                        _localService.OptionsService.GetCurrentOrDefaultOptions(), _tokenSource.Token);

                    return result;
                });

                timer.Stop();

                OpenPopup(new Dialog(resultUpdate.result.ToString(), resultUpdate.message, DialogButtons.Ok));
            }
            else
            {
                OpenPopup(new Dialog("Query Missing", "Please write a query to execute", DialogButtons.Ok));
            }
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            WbQuery.Focus();
            _tokenSource?.Cancel();
        }

        private void WbQuery_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    BtnUpdate_OnClick(null, null);
                    break;

                case Key.F7:
                    BtnCancel_OnClick(null, null);
                    break;
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (_currentDialog == null) return;

            _currentDialog.Left = _owner.Left;
            _currentDialog.Top = _owner.Top + 78;
        }

        private void ParentTab_LoseFocus()
        {
            _currentDialog?.Hide();
        }
    }
}