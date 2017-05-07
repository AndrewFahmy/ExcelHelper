using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ExcelHelper.Common.Interfaces;
using ExcelHelper.Common.Models;
using ExcelHelper.Extensions;

namespace ExcelHelper.Windows
{
    public partial class Options 
    {
        private readonly ILocalService _service;

        public Options(ILocalService service, Window owner)
        {
            _service = service;
            InitializeComponent();

            Owner = owner;

            CbHeaderFonts.ItemsSource =
                CbRowFonts.ItemsSource = _service.OptionsService.GetInstalledFonts();

            DataContext = _service.OptionsService.GetCurrentOrDefaultOptions();
        }

        private bool ValidateControls()
        {
            var result = TbHeaderBgRed.IsValid();

            result = TbHeaderBgGreen.IsValid() && result;

            result = TbHeaderBgBlue.IsValid() && result;

            result = TbHeaderFgRed.IsValid() && result;

            result = TbHeaderFgGreen.IsValid() && result;

            result = TbHeaderFgBlue.IsValid() && result;

            result = CbHeaderFonts.IsValid() && result;

            result = TbHeaderFontSize.IsValid() && result;

            result = TbRowBgRed.IsValid() && result;

            result = TbRowBgGreen.IsValid() && result;

            result = TbRowBgBlue.IsValid() && result;

            result = TbRowFgRed.IsValid() && result;

            result = TbRowFgGreen.IsValid() && result;

            result = TbRowFgBlue.IsValid() && result;

            result = CbRowFonts.IsValid() && result;

            result = TbRowFontSize.IsValid() && result;

            return result;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateControls()) return;

            var model = (OptionsModel)DataContext;

            _service.OptionsService.WriteOptionsToFile(model);

            Close();
        }

        private void BgColorTextBoxes_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = (TextBox)sender;

            var model = (OptionsModel)DataContext;

            if (model == null) return;

            if (currentTextBox.Tag != null)
                TbHeaderBackground.Background = new SolidColorBrush(new Color { R = model.HeaderBgRed, G = model.HeaderBgGreen, B = model.HeaderBgBlue, A = 255 });
            else
                TbRowBackground.Background = new SolidColorBrush(new Color { R = model.RowBgRed, G = model.RowBgGreen, B = model.RowBgBlue, A = 255 });
        }

        private void FgColorTextBoxes_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = (TextBox)sender;

            var model = (OptionsModel)DataContext;

            if (model == null) return;

            if (currentTextBox.Tag != null)
                TbHeaderForeground.Background = new SolidColorBrush(new Color { R = model.HeaderFgRed, G = model.HeaderFgGreen, B = model.HeaderFgBlue, A = 255 });
            else
                TbRowForeground.Background = new SolidColorBrush(new Color { R = model.RowFgRed, G = model.RowFgGreen, B = model.RowFgBlue, A = 255 });
        }

        private void TextBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).ResetValidation();
        }

        private void ComboBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ComboBox)sender).ResetValidation();
        }
    }
}