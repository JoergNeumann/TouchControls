using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Neumann.TouchControls
{
    public class ToggleButtonList : ItemsControl
    {

        #region Constructors

        public ToggleButtonList()
        {
            this.DefaultStyleKey = typeof(ToggleButtonList);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region SelectedItem

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ToggleButtonList), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is ToggleButton)
            {
                ((ToggleButton)element).ContentTemplate = this.ItemTemplate;
                ((ToggleButton)element).Checked += this.OnButtonChecked;
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToggleButton()
            {
            };
        }

        #endregion

        #region Private Methods

        private void CheckItem(ToggleButton checkedItem)
        {
            foreach (var item in this.Items)
            {
                var element = this.ContainerFromItem(item) as ToggleButton;
                if (element != null && element != checkedItem)
                {
                    element.IsChecked = false;
                }
            }
            checkedItem.IsChecked = true;
            this.SelectedItem = this.ItemFromContainer(checkedItem);
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;
            var view = this.ItemsSource as ICollectionView;
            if (view != null)
            {
                view.CurrentChanged += this.OnCurrentChanged;
                if (view.CurrentItem != null)
                {
                    var selectedItem = this.ContainerFromItem(view.CurrentItem) as ToggleButton;
                    if (selectedItem != null)
                    {
                        this.CheckItem(selectedItem);
                    }
                }
            }
        }

        private void OnButtonChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Items)
            {
                var element = this.ContainerFromItem(item) as ToggleButton;
                if (element != null && element != sender)
                {
                    element.IsChecked = false;
                }
            }
            var button = sender as ToggleButton;
            if (button.IsChecked.HasValue && button.IsChecked.Value)
            {
                this.SelectedItem = this.ItemFromContainer((ToggleButton)sender);
            }
            else
            {
                this.SelectedItem = null;
            }
            var view = this.ItemsSource as ICollectionView;
            if (view != null)
            {
                view.MoveCurrentTo(this.SelectedItem);
            }
        }

        private void OnCurrentChanged(object sender, object e)
        {
            var view = this.ItemsSource as ICollectionView;
            this.SelectedItem = view.CurrentItem;
            var element = this.ContainerFromItem(this.SelectedItem) as ToggleButton;
            if (element != null)
            {
                this.CheckItem(element);
            }
        }

        #endregion

    }
}
