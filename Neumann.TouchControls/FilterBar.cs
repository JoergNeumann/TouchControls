using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Neumann.TouchControls
{
    public class FilterBar : ItemsControl
    {

        #region Constructors

        public FilterBar()
        {
            this.DefaultStyleKey = typeof(FilterBar);
        }

        #endregion

        #region Properties

        #region HeaderTemplate

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(FilterBar), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Overrides

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is FilterBarItem)
            {
                ((FilterBarItem)element).FilterBar = this;
                ((FilterBarItem)element).HeaderTemplate = this.HeaderTemplate;
                ((FilterBarItem)element).ContentTemplate = this.ItemTemplate;
            }
            base.PrepareContainerForItemOverride(element, item); base.PrepareContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FilterBarItem()
            {
            };
        }

        #endregion

        #region Internal Methods

        internal void CollapseAll()
        {
            foreach (var item in this.Items)
            {
                var barItem = this.ContainerFromItem(item) as FilterBarItem;
                if (barItem != null)
                {
                    barItem.IsExpanded = false;
                }
            }
        }

        #endregion

    }
}
