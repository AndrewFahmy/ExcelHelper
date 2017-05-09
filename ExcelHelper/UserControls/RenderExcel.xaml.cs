using System;
using System.ComponentModel;
using System.Net;
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
    public partial class RenderExcel
    {
        private readonly ILocalService _localService;
        private readonly Window _owner;
        private readonly CustomTabItem _parentTab;
        private readonly SaveFileDialog _saveFileDialog;
        private Dialog _currentDialog;
        private CancellationTokenSource _tokenSource;

        public RenderExcel(ILocalService localService, Window owner, CustomTabItem parentTab)
        {
            _localService = localService;
            _owner = owner;
            _parentTab = parentTab;

            InitializeComponent();

            _saveFileDialog = new SaveFileDialog { Filter = Constants.ExcelFileFilter };
            _saveFileDialog.FileOk += SaveFileDialog_FileOk;

            CbConnections.ItemsSource = _localService.ConnectionsService.GetConnections();

            WbQuery.Navigate(new Uri(
                $"file:///{AppDomain.CurrentDomain.BaseDirectory}Resources/QueryPage.html?m={Constants.RenderMessage}"));
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
            _parentTab.CloseButton.IsEnabled = false;

            if (_parentTab.IsActive)
                dialog.Show();
        }

        private void ResetWindow()
        {
            LoadingImage.Visibility = Visibility.Hidden;
            BtnRender.IsEnabled = BtnParse.IsEnabled = BtnCancel.IsEnabled = true;
            _tokenSource = null;
            _currentDialog?.Close();
            _currentDialog = null;
            _parentTab.CloseButton.IsEnabled = true;
            LblTimer.Content = "00:00:00";
            LblTimer.Visibility = Visibility.Hidden;
        }

        private void ShowLoadingIconAndTimerLabel()
        {
            LoadingImage.Visibility = Visibility.Visible;
            LblTimer.Visibility = Visibility.Visible;
            BtnRender.IsEnabled = BtnParse.IsEnabled = false;
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

        private async void Render_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateControls()) return;

            _tokenSource = new CancellationTokenSource();

            var query = Regex.Match(((IHTMLDocument2)WbQuery.Document).body.innerHTML,
                    Constants.QueryValuePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            if (!string.IsNullOrWhiteSpace(query))
            {
                ShowLoadingIconAndTimerLabel();

                var connectionString = CbConnections.SelectedValue.ToString();

                var model = new ExecutionModel
                {
                    AddQueryToFile = ChkAddQuery.IsChecked.GetValueOrDefault(),
                    FileName = TbLocation.Text,
                    Query = query
                };

                var timer = StartTimer();

                var rederResult = await Task.Factory.StartNew(() =>
                {
                    IDbService dbService = new DbService(connectionString);

                    var result = dbService.RenderExcelService.Execute(model,
                        _localService.OptionsService.GetCurrentOrDefaultOptions(), _tokenSource.Token);

                    return result;
                });


                timer.Stop();

                OpenPopup(new Dialog(rederResult.result.ToString(), rederResult.message, DialogButtons.Ok));
            }
            else
            {
                OpenPopup(new Dialog("Query Missing", "Please write a query to execute", DialogButtons.Ok));
            }
        }

        private async void Parse_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CbConnections.IsValid()) return;

            var query = Regex.Match(((IHTMLDocument2)WbQuery.Document).body.innerHTML,
                    Constants.QueryValuePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            if (string.IsNullOrWhiteSpace(query))
            {
                OpenPopup(new Dialog("Query Missing", "Please write a query to parse", DialogButtons.Ok));
                return;
            }

            ShowLoadingIconAndTimerLabel();

            WbQuery.Focus();

            var connectionString = CbConnections.SelectedValue.ToString();

            var timer = StartTimer();


            var resultParse = await Task.Factory.StartNew(() =>
            {
                IDbService dbService = new DbService(connectionString);

                var result = dbService.ParseQuery(WebUtility.HtmlDecode(query));

                return result;
            });


            timer.Stop();

            OpenPopup(new Dialog(resultParse.result.ToString(), resultParse.message, DialogButtons.Ok));
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            WbQuery.Focus();
            _tokenSource?.Cancel();
        }

        private void FileBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            _saveFileDialog.ShowDialog();
        }

        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            TbLocation.Text = _saveFileDialog.FileName;
        }

        private void CbConnections_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CbConnections.ResetValidation();
        }

        private void TbLocation_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TbLocation.ResetValidation();
        }

        private void WbQuery_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    Render_OnClick(null, null);
                    break;

                case Key.F6:
                    Parse_OnClick(null, null);
                    break;

                case Key.F7:
                    Stop_OnClick(null, null);
                    break;
            }
        }

        private void RenderExcel_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_currentDialog != null)
                OpenPopup(_currentDialog);
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