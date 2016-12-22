using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Neumann.TouchControls
{
    public class Expander : ItemsControl
    {

        #region Private Fields

        private StackPanel _panel;
        private bool _isUpdating;
        private bool _isInitializing = true;
        private double _collapsedItemsLength;

        #endregion

        #region Constructors

        public Expander()
        {
            this.DefaultStyleKey = typeof(Expander);
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties

        #region SelectedIndex

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Expander), new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Expander;
            if (!element._isUpdating)
            {
                element._isUpdating = true;
                var index = (int)e.NewValue;
                if (index > -1 && index < element.Items.Count)
                {
                    var expanderItem = element.ContainerFromIndex(index) as ExpanderItem;
                    if (expanderItem != null)
                    {
                        expanderItem.IsExpanded = true;
                    }
                    element.SelectedItem = element.Items[index];
                    element.ExecuteCommand();
                }
                element._isUpdating = false;
            }
            element.OnSelectedIndexChanged();
        }

        #endregion

        #region SelectedItem

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Expander), new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Expander;
            if (!element._isUpdating)
            {
                element._isUpdating = true;
                var item = e.NewValue;
                if (item != null)
                {
                    var expanderItem = element.ContainerFromItem(item) as ExpanderItem;
                    if (expanderItem != null)
                    {
                        expanderItem.IsExpanded = true;
                    }
                    element.SelectedIndex = element.Items.IndexOf(item);
                    element.ExecuteCommand();
                }
                else
                {
                    element.SelectedIndex = -1;
                }
                element._isUpdating = false;
            }
            element.OnSelectedItemChanged();
        }

        #endregion

        #region HeaderTemplateExpanded

        public DataTemplate HeaderTemplateExpanded
        {
            get { return (DataTemplate)GetValue(HeaderTemplateExpandedProperty); }
            set { SetValue(HeaderTemplateExpandedProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateExpandedProperty =
            DependencyProperty.Register("HeaderTemplateExpanded", typeof(DataTemplate), typeof(Expander), new PropertyMetadata(null));

        #endregion

        #region HeaderTemplateCollapsed

        public DataTemplate HeaderTemplateCollapsed
        {
            get { return (DataTemplate)GetValue(HeaderTemplateCollapsedProperty); }
            set { SetValue(HeaderTemplateCollapsedProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateCollapsedProperty =
            DependencyProperty.Register("HeaderTemplateCollapsed", typeof(DataTemplate), typeof(Expander), new PropertyMetadata(null));

        #endregion

        #region Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Expander), new PropertyMetadata(null));

        #endregion

        #region CommandParameter

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(Expander), new PropertyMetadata(null));

        #endregion

        #region Orientation

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Expander), new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Expander;
            if (e.NewValue != null)
            {
                element.SetOrientation((Orientation)Enum.Parse(typeof(Orientation), e.NewValue.ToString()));
            }
        }

        #endregion

        #endregion

        #region Events

        #region SelectedIndexChanged

        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region SelectedItemChanged

        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #endregion

        #region Overrides

        protected async override void OnItemsChanged(object e)
        {
            if (!_isInitializing)
            {
                this.SetOrientation(this.Orientation);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    this.MeasureCollapsedItemsLength();
                });
            }
            base.OnItemsChanged(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExpanderItem() { Expander = this };
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var expanderItem = element as ExpanderItem;
            if (expanderItem != null)
            {
                expanderItem.Orientation = this.Orientation;
                if (this.HeaderTemplateExpanded != null)
                {
                    expanderItem.HeaderTemplateExpanded = this.HeaderTemplateExpanded;
                }
                if (this.HeaderTemplateCollapsed != null)
                {
                    expanderItem.HeaderTemplateCollapsed = this.HeaderTemplateCollapsed;
                }
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion

        #region Private Methods

        private double GetElementLength(FrameworkElement element)
        {
            return this.Orientation == Orientation.Vertical ? element.RenderSize.Height : element.RenderSize.Width;
        }

        private double MeasureCollapsedItemsLength()
        {
            // measure length of collapsed items
            _collapsedItemsLength = 0d;
            var count = this.Orientation == Orientation.Vertical ? this.Items.Count : this.Items.Count - 1;
            for (int i = 0; i < count; i++)
            {
                var element = this.ContainerFromIndex(i) as ExpanderItem;
                if (element != null)
                {
                    _collapsedItemsLength += this.GetElementLength(element);
                }
            }

            // measure available length
            var availableLength = this.GetElementLength(this);

            // calculate expanded length and apply it to items
            var expandedLength = Math.Max(0, availableLength - _collapsedItemsLength);
            foreach (var item in this.Items)
            {
                var element = this.ContainerFromItem(item) as ExpanderItem;
                if (element != null)
                {
                    // initialize item with expanded length
                    element.Initialize(expandedLength);
                }
            }
            return _collapsedItemsLength;
        }

        private StackPanel GetPanel()
        {
            if (_panel == null)
            {
                _panel = this.ItemsPanelRoot as StackPanel;
            }
            return _panel;
        }

        internal void ExpandItem(ExpanderItem item, bool expand)
        {
            if (expand)
            {
                if (this.SelectedIndex != -1)
                {
                    var selectedElement = this.ContainerFromIndex(this.SelectedIndex) as ExpanderItem;
                    if (selectedElement != null && selectedElement != item)
                    {
                        selectedElement.IsExpanded = false;
                    }
                }
                var element = this.ItemFromContainer(item);
                if (element != null)
                {
                    this.SelectedIndex = this.Items.IndexOf(element);
                    this.SelectedItem = element;
                }
            }
            else
            {
                this.SelectedIndex = -1;
                this.SelectedItem = null;
            }

        }

        private void SetOrientation(Orientation orientation)
        {
            if (this.Items.Count > 0)
            {
                // set orientation on items
                for (int i = 0; i < this.Items.Count; i++)
                {
                    var element = this.ContainerFromIndex(i) as ExpanderItem;
                    if (element != null)
                    {
                        element.Orientation = Orientation;
                    }
                }

                // refersh layout
                this.UpdateLayout();
            }
        }

        protected virtual void ExecuteCommand()
        {
            if (!_isInitializing && this.Command != null)
            {
                this.Command.Execute(this.CommandParameter == null ? this.SelectedItem : this.CommandParameter);
            }
        }

        #endregion

        #region Event Handling

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

            // set orientation on panel
            var panel = this.GetPanel();
            if (panel != null)
            {
                panel.Orientation = this.Orientation;
            }

            // set orientation on items
            this.SetOrientation(this.Orientation);

            // measure items
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                this.MeasureCollapsedItemsLength();
                _isInitializing = false;
            });
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                var availableSize = this.GetElementLength(this);
                var expandedLength = Math.Max(0, availableSize - _collapsedItemsLength);
                for (int i = 0; i < this.Items.Count; i++)
                {
                    var element = this.ContainerFromIndex(i) as ExpanderItem;
                    if (element != null)
                    {
                        element.SetExpandedLength(expandedLength);
                    }
                }
            }
        }

        #endregion

    }
}
