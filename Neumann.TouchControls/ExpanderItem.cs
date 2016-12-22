using System;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Neumann.TouchControls
{
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Header", Type = typeof(LayoutTransformer))]
    [TemplatePart(Name = "PART_HeaderRow", Type = typeof(RowDefinition))]
    [TemplatePart(Name = "PART_ContentRow", Type = typeof(RowDefinition))]
    [TemplatePart(Name = "PART_HeaderCol", Type = typeof(ColumnDefinition))]
    [TemplatePart(Name = "PART_ContentCol", Type = typeof(ColumnDefinition))]
    [TemplatePart(Name = "PART_HorizontalCollapseAnimation", Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = "PART_HorizontalExpandAnimation", Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = "PART_VerticalCollapseAnimation", Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = "PART_VerticalExpandAnimation", Type = typeof(DoubleAnimation))]
    [TemplateVisualState(GroupName = "Common", Name = "HorizontalCollapsed")]
    [TemplateVisualState(GroupName = "Common", Name = "HorizontalExpanded")]
    [TemplateVisualState(GroupName = "Common", Name = "VerticalCollapsed")]
    [TemplateVisualState(GroupName = "Common", Name = "VerticalExpanded")]
    public class ExpanderItem : ContentControl
    {

        #region Private Fields

        private Expander _expander;
        private ContentPresenter _content;
        private LayoutTransformer _header;
        private RowDefinition _headerRow;
        private RowDefinition _contentRow;
        private ColumnDefinition _headerColumn;
        private ColumnDefinition _contentColumn;
        private DoubleAnimation _horizontalCollapseAnimation;
        private DoubleAnimation _horizontalExpandAnimation;
        private DoubleAnimation _verticalCollapseAnimation;
        private DoubleAnimation _verticalExpandAnimation;
        internal double _expandedLength;
        private bool _isInitializing = true;

        #endregion

        #region Constructors

        public ExpanderItem()
        {
            this.DefaultStyleKey = typeof(ExpanderItem);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Header

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #region HeaderTemplateExpanded

        public DataTemplate HeaderTemplateExpanded
        {
            get { return (DataTemplate)GetValue(HeaderTemplateExpandedProperty); }
            set { SetValue(HeaderTemplateExpandedProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateExpandedProperty =
            DependencyProperty.Register("HeaderTemplateExpanded", typeof(DataTemplate), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #region HeaderTemplateCollapsed

        public DataTemplate HeaderTemplateCollapsed
        {
            get { return (DataTemplate)GetValue(HeaderTemplateCollapsedProperty); }
            set { SetValue(HeaderTemplateCollapsedProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateCollapsedProperty =
            DependencyProperty.Register("HeaderTemplateCollapsed", typeof(DataTemplate), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #region IsExpanded

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExpanderItem), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ExpanderItem;
            if (!element._isInitializing)
            {
                element.SetExpasionState();
            }
        }

        #endregion

        #region Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #region CommandParameter

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #region Expander

        public Expander Expander
        {
            get
            {
                if (_expander == null)
                {
                    _expander = this.GetParent<Expander>();
                }
                return _expander;
            }
            internal set { _expander = value; }
        }

        #endregion

        #region Orientation

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ExpanderItem), new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ExpanderItem;
            element.SetOrientation((Orientation)Enum.Parse(typeof(Orientation), e.NewValue.ToString()));
        }

        #endregion

        #region HeaderTemplate

        internal DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        internal static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ExpanderItem), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        public event EventHandler IsExpandedChanged;

        protected virtual void OnIsExpandedChanged()
        {
            if (IsExpandedChanged != null)
            {
                IsExpandedChanged(this, EventArgs.Empty);
            }
            if (this.Command != null)
            {
                this.Command.Execute(this.CommandParameter);
            }
        }

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _content = this.GetTemplateChild("PART_Content") as ContentPresenter;
            _header = this.GetTemplateChild("PART_Header") as LayoutTransformer;

            _headerColumn = this.GetTemplateChild("PART_HeaderCol") as ColumnDefinition;
            _contentColumn = this.GetTemplateChild("PART_ContentCol") as ColumnDefinition;
            _headerRow = this.GetTemplateChild("PART_HeaderRow") as RowDefinition;
            _contentRow = this.GetTemplateChild("PART_ContentRow") as RowDefinition;

            _horizontalCollapseAnimation = this.GetTemplateChild("PART_HorizontalCollapseAnimation") as DoubleAnimation;
            _horizontalExpandAnimation = this.GetTemplateChild("PART_HorizontalExpandAnimation") as DoubleAnimation;
            _verticalCollapseAnimation = this.GetTemplateChild("PART_VerticalCollapseAnimation") as DoubleAnimation;
            _verticalExpandAnimation = this.GetTemplateChild("PART_VerticalExpandAnimation") as DoubleAnimation;

            // register handler for header click
            if (_header != null)
            {
                _header.PointerPressed += this.OnHeaderPointerPressed;
            }
        }

        #endregion

        #region Private Methods

        internal void Initialize(double expandedLength)
        {
            // set expansion length
            _expandedLength = expandedLength;
            this.SetExpandedLength();

            var orientation = this.Orientation;
            var isExpanded = this.IsExpanded;

            if (_content != null)
            {
                // set content length
                if (orientation == Orientation.Horizontal)
                {
                    _content.Width = isExpanded ? _expandedLength : 0;
                }
                else
                {
                    _content.Height = isExpanded ? _expandedLength : 0;
                }

                // make content visible
                _content.Visibility = Visibility.Visible;
            }

            // refresh layout acording to expansion state
            this.SetExpasionState();
            _isInitializing = false;
        }

        internal void SetExpandedLength(double length)
        {
            _expandedLength = length;
            this.SetExpandedLength();

            if (this.IsExpanded && _content != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    _content.Width = length;
                }
                else
                {
                    _content.Height = length;
                }
            }
        }

        private void SetOrientation(Orientation orientation)
        {
            if (_header != null && _content != null)
            {
                if (orientation == Orientation.Horizontal)
                {
                    _header.SetValue(Grid.RowSpanProperty, 2);
                    _content.SetValue(Grid.RowSpanProperty, 2);
                    _header.SetValue(Grid.ColumnSpanProperty, 1);
                    _content.SetValue(Grid.ColumnSpanProperty, 1);
                    _header.SetValue(Grid.ColumnProperty, 0);
                    _content.SetValue(Grid.ColumnProperty, 1);
                    _header.LayoutTransform = new RotateTransform { Angle = -90 };
                }
                else if (orientation == Orientation.Vertical)
                {
                    _header.SetValue(Grid.RowSpanProperty, 1);
                    _content.SetValue(Grid.RowSpanProperty, 1);
                    _header.SetValue(Grid.ColumnSpanProperty, 2);
                    _content.SetValue(Grid.ColumnSpanProperty, 2);
                    _header.SetValue(Grid.RowProperty, 0);
                    _content.SetValue(Grid.RowProperty, 1);
                    this._header.LayoutTransform = null;
                }
            }
        }

        private void SetExpandedLength()
        {
            // setup animations with actual width/height
            if (this.Orientation == Orientation.Horizontal &&
                _horizontalCollapseAnimation != null &&
                _horizontalExpandAnimation != null &&
                _headerColumn != null &&
                _contentColumn != null)
            {
                _horizontalCollapseAnimation.From = _expandedLength;
                _horizontalExpandAnimation.To = _expandedLength;
            }
            else if (this.Orientation == Orientation.Vertical &&
                _verticalCollapseAnimation != null &&
                _verticalExpandAnimation != null &&
                _headerRow != null &&
                _contentRow != null)
            {
                _verticalCollapseAnimation.From = _expandedLength;
                _verticalExpandAnimation.To = _expandedLength;
            }
        }

        private void SetHeaderTemplate(bool isExpanded)
        {
            if (isExpanded && this.HeaderTemplateExpanded != null)
            {
                this.HeaderTemplate = this.HeaderTemplateExpanded;
            }
            else if (this.HeaderTemplateCollapsed != null)
            {
                this.HeaderTemplate = this.HeaderTemplateCollapsed;
            }
        }

        private void SetExpasionState()
        {
            var isExpanded = this.IsExpanded;

            // apply header template
            this.SetHeaderTemplate(isExpanded);

            // change layout
            if (isExpanded)
            {
                this.Expander.ExpandItem(this, isExpanded);
                if (this.Orientation == Orientation.Horizontal)
                {
                    _header.SetValue(Grid.RowProperty, 0);
                    _header.SetValue(Grid.RowSpanProperty, 1);
                    _header.SetValue(Grid.ColumnProperty, 0);
                    _header.SetValue(Grid.ColumnSpanProperty, 2);
                    _content.SetValue(Grid.RowProperty, 1);
                    _content.SetValue(Grid.RowSpanProperty, 1);
                    _content.SetValue(Grid.ColumnProperty, 0);
                    _content.SetValue(Grid.ColumnSpanProperty, 2);
                    _header.LayoutTransform = null;
                }
            }
            else
            {
                this.SetOrientation(this.Orientation);
            }

            // raise ExpandedChanged event
            this.OnIsExpandedChanged();

            // apply visual state
            if (!DesignMode.DesignModeEnabled && !_isInitializing)
            {
                var state = isExpanded ? this.Orientation == Orientation.Horizontal ? "HorizontalExpanded" : "VerticalExpanded"
                    : this.Orientation == Orientation.Horizontal ? "HorizontalCollapsed" : "VerticalCollapsed";
                VisualStateManager.GoToState(this, state, false);
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

            // set the orientation
            this.SetOrientation(this.Orientation);

            // set template
            this.SetHeaderTemplate(this.IsExpanded);
        }

        private void OnHeaderPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // toggle expansion
            this.IsExpanded = !this.IsExpanded;
        }

        #endregion

    }
}
