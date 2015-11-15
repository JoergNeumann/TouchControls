using Windows.UI.Xaml.Controls;

namespace Demo.Views
{
    public sealed partial class ColumnViewPage : Page
    {
        public ColumnViewPage()
        {
            this.InitializeComponent();
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 0;
        }

        private void ListBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 1;
        }

        private void ListBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 2;
        }

        private void ListBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 3;
        }

        private void ListBox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 4;
        }

        private void ListBox6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ColumnView.SelectedIndex = 5;
        }
    }
}
