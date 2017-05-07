using System;
using System.Windows;
using ExcelHelper.Common;

namespace ExcelHelper.Windows
{
    public partial class Dialog
    {
        public Action<DialogButtons> OnActionClick;

        public Dialog(string title, string message, DialogButtons buttons)
        {
            InitializeComponent();
            LblTitle.Content = title;
            TbMessage.Text = message;
            ShowDiaogButtons(buttons);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            OnActionClick?.Invoke(DialogButtons.Cancel);
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            OnActionClick?.Invoke(DialogButtons.Ok);
        }

        private void BtnNo_OnClick(object sender, RoutedEventArgs e)
        {
            OnActionClick?.Invoke(DialogButtons.No);
        }

        private void BtnYes_OnClick(object sender, RoutedEventArgs e)
        {
            OnActionClick?.Invoke(DialogButtons.Yes);
        }

        private void ShowDiaogButtons(DialogButtons buttons)
        {
            BtnCancel.Visibility = buttons.HasFlag(DialogButtons.Cancel) ? Visibility.Visible : Visibility.Collapsed;

            BtnOk.Visibility = buttons.HasFlag(DialogButtons.Ok) ? Visibility.Visible : Visibility.Collapsed;

            BtnNo.Visibility = buttons.HasFlag(DialogButtons.No) ? Visibility.Visible : Visibility.Collapsed;

            BtnYes.Visibility = buttons.HasFlag(DialogButtons.Yes) ? Visibility.Visible : Visibility.Collapsed;

            if (buttons.HasFlag(DialogButtons.Cancel))
                BtnCancel.IsCancel = true;
            else if (buttons.HasFlag(DialogButtons.No))
                BtnNo.IsCancel = true;

            if (buttons.HasFlag(DialogButtons.Ok))
                BtnCancel.IsDefault = true;
            else if (buttons.HasFlag(DialogButtons.Yes))
                BtnOk.IsDefault = true;
        }
    }
}