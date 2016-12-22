using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Neumann.TouchControls
{
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    [TemplateVisualState(GroupName = "Common", Name = "Normal")]
    [TemplateVisualState(GroupName = "Common", Name = "Expanded")]
    public class FilterBarItem : HeaderedContentControl
    {

        #region Private Fields

        private Popup _popup;
        private ContentPresenter _presenter;
        private bool _isClosing;

        #endregion

        #region Constructors

        public FilterBarItem()
        {
            this.DefaultStyleKey = typeof(FilterBarItem);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region FilterBar

        public FilterBar FilterBar
        {
            get { return (FilterBar)GetValue(FilterBarProperty); }
            set { SetValue(FilterBarProperty, value); }
        }

        public static readonly DependencyProperty FilterBarProperty =
            DependencyProperty.Register("FilterBar", typeof(FilterBar), typeof(FilterBarItem), new PropertyMetadata(null));

        #endregion

        #region IsExpanded

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(FilterBarItem), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FilterBarItem;
            var value = (bool)e.NewValue;
            element._isClosing = true;
            if (value && element.FilterBar != null)
            {
                foreach (var item in element.FilterBar.Items)
                {
                    var barItem = element.FilterBar.ContainerFromItem(item) as FilterBarItem;
                    if (barItem != null && barItem != element)
                    {
                        barItem.IsExpanded = false;
                    }
                }
            }
            VisualStateManager.GoToState(element, value ? "Expanded" : "Normal", false);
            element._isClosing = false;
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = this.GetTemplateChild("PART_Popup") as Popup;
            _presenter = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_presenter != null && this.FilterBar != null)
            {
                var position = this.TransformToVisual(this.FilterBar).TransformPoint(new Point(0, 0));
            }
            this.IsExpanded = !this.IsExpanded;
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

            if (this.FilterBar == null)
            {
                this.FilterBar = VisualTreeHelpers.GetParent<FilterBar>(this);
            }

            if (this.Header == null || string.IsNullOrWhiteSpace(this.Header.ToString()))
            {
                this.Header = this.Content;
            }

            if (_popup != null && this.FilterBar != null)
            {
                _popup.VerticalOffset = this.FilterBar.RenderSize.Height + this.BorderThickness.Bottom;
                _popup.HorizontalOffset = -this.BorderThickness.Left;
                _popup.Closed += this.OnPopupClosed;
            }
        }

        private void OnPopupClosed(object sender, object e)
        {
            if (!_isClosing)
            {
                this.IsExpanded = false;
            }
        }

        #endregion

    }
}
