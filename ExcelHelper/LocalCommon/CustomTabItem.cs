using System.Windows;
using System.Windows.Controls;
using ExcelHelper.UserControls;

namespace ExcelHelper.LocalCommon
{
    public class CustomTabItem : TabItem
    {
        public void SetHeader(string header)
        {
            var headerText = new TextBlock { Text = header };
            var dockpanel = new DockPanel { Style = (Style)Application.Current.Resources["TabPanelStyle"] };

            var closebutton = new TabCloseButton(() =>
            {
                var tabCtl = Parent as TabControl;
                tabCtl?.Items?.Remove(this);

                if (tabCtl != null && tabCtl.Items?.Count <= 0)
                    tabCtl.Visibility = Visibility.Collapsed;
            });

            DockPanel.SetDock(closebutton, Dock.Right);
            dockpanel.Children.Add(closebutton);
            headerText.Style = (Style)Application.Current.Resources["TabHeaderStyle"];
            dockpanel.Children.Add(headerText);

            Header = dockpanel;
        }
    }
}