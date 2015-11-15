using Neumann.TouchControls;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Demo.Views
{
    public sealed partial class FanSelectorPage : Page
    {
        public FanSelectorPage()
        {
            this.InitializeComponent();
        }

        private async void OnFanClick(object sender, EventArgs e)
        {
            var element = sender as FanControl;
            await new MessageDialog(element.Content.ToString() + " clicked!").ShowAsync();
        }
    }
}
