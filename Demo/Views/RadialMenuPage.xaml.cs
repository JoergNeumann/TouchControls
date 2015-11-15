using Neumann.TouchControls;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Demo.Views
{
    public sealed partial class RadialMenuPage : Page
    {
        public RadialMenuPage()
        {
            this.InitializeComponent();
        }

        private async void RadialMenuItem_Click(object sender, System.EventArgs e)
        {
            await new MessageDialog(((RadialMenuItem)sender).Header).ShowAsync();
        }
    }
}
