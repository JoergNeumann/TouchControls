using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Demo.Views
{
    public sealed partial class RadialPickerPage : Page
    {
        public RadialPickerPage()
        {
            this.InitializeComponent();
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Visible", false);
        }

        private void radialPicker_Closed(object sender, System.EventArgs e)
        {
            VisualStateManager.GoToState(this, "Collapsed", false);
        }
    }
}
