using System;
using System.Windows;

namespace ExcelHelper.UserControls
{
    public partial class TabCloseButton
    {
        private readonly Action _click;

        public TabCloseButton(Action clickAction)
        {
            InitializeComponent();
            _click = clickAction;
        }

        private void BtnTabClose_OnClick(object sender, RoutedEventArgs e)
        {
            _click?.Invoke();
        }
    }
}