using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Neumann.TouchControls
{
    public class FanSelector : ContentControl
    {

        #region Private Fields

        private Grid _grid;
        private PieSlice _button;
        private bool _isUpdating;

        #endregion

        #region Constructors

        public FanSelector()
        {
            this.DefaultStyleKey = typeof(FanSelector);
            this.Fans = new FanCollection();
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Fans

        public FanCollection Fans { get { return (FanCollection)GetValue(FansProperty); } set { SetValue(FansProperty, value); } }
        public static readonly DependencyProperty FansProperty =
            DependencyProperty.Register("Fans", typeof(FanCollection), typeof(FanSelector),
            new PropertyMetadata(null, OnFansPropertyChanged));

        private static void OnFansPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FanSelector;
            var oldList = e.NewValue as FanCollection;
            if (oldList != null)
            {
                oldList.CollectionChanged -= element.OnCollectionChanged;
            }
            var newList = e.NewValue as FanCollection;
            if (newList != null)
            {
                newList.CollectionChanged += element.OnCollectionChanged;
                element.CreateFans();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CreateFans();
        }

        #endregion

        #region ButtonStyle

        public Style ButtonStyle { get { return (Style)GetValue(ButtonStyleProperty); } set { SetValue(ButtonStyleProperty, value); } }
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(FanSelector), new PropertyMetadata(null));

        #endregion

        #region IsOpen

        public bool IsOpen { get { return (bool)GetValue(IsOpenProperty); } set { SetValue(IsOpenProperty, value); } }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FanSelector),
            new PropertyMetadata(false, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FanSelector;
            var value = (bool)e.NewValue;
            element.StartStoryboards(value);
        }

        #endregion

        #region DisplayMode

        public DisplayMode DisplayMode { get { return (DisplayMode)GetValue(DisplayModeProperty); } set { SetValue(DisplayModeProperty, value); } }
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode ", typeof(DisplayMode), typeof(FanSelector), new PropertyMetadata(DisplayMode.Quarter));

        #endregion

        #region IsRotatingContent

        public bool IsRotatingContent { get { return (bool)GetValue(IsRotatingContentProperty); } set { SetValue(IsRotatingContentProperty, value); } }
        public static readonly DependencyProperty IsRotatingContentProperty =
            DependencyProperty.Register("IsRotatingContent", typeof(bool), typeof(FanSelector), new PropertyMetadata(true));

        #endregion

        #endregion

        #region Events

        #region SelectionChanged

        public event EventHandler SelectionChanged;
        internal virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Click

        public event EventHandler Click;

        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = this.GetTemplateChild("PART_RootGrid") as Grid;
            _button = this.GetTemplateChild("PART_Button") as PieSlice;
            if (_button != null)
            {
                _button.PointerEntered += this.OnButtonPointerEntered;
                _button.PointerExited += this.OnButtonPointerExited;
                _button.PointerPressed += this.OnButtonPointerPressed;
                _button.PointerReleased += this.OnButtonPointerReleased;
            }
            //if (this.IsOpen)
            //{
            //    this.StartStoryboards(this.IsOpen);
            //}
        }

        #endregion

        #region Private Functions

        private void CreateFans()
        {
            var fans = this.Fans;
            var grid = this._grid;
            if (grid != null && fans != null)
            {
                var count = fans.Count;
                int start = 270;
                int distance = 0;
                if (this.DisplayMode == DisplayMode.Quarter)
                {
                    distance = (360 - start) / count;
                }
                else if (this.DisplayMode == DisplayMode.Half)
                {
                    distance = (start - 180 + 90) / count;
                }
                else if (this.DisplayMode == DisplayMode.Horseshoe)
                {
                    start = 225;
                    distance = 270 / count;
                }
                var end = start + distance;
                double radius = 0;
                if (this.DisplayMode == DisplayMode.Quarter)
                {
                    radius = this.RenderSize.Width;
                }
                else if (this.DisplayMode == DisplayMode.Half)
                {
                    radius = this.RenderSize.Width / 2;
                }
                else if (this.DisplayMode == DisplayMode.Horseshoe)
                {
                    radius = this.RenderSize.Width / 2;
                }

                grid.Children.Clear();
                foreach (var fanControl in fans)
                {
                    fanControl.StartAngle = start;
                    fanControl.EndAngle = end;
                    fanControl.Radius = radius;
                    fanControl.Width = this.RenderSize.Width;
                    fanControl.Height = this.RenderSize.Height;
                    fanControl.IsRotatingContent = this.IsRotatingContent;
                    fanControl.Distance = distance;
                    fanControl.DisplayMode = this.DisplayMode;
                    double angle = 145;
                    if (this.DisplayMode == DisplayMode.Quarter)
                    {
                        fanControl.RenderTransformOrigin = new Point(1, 1);
                    }
                    else if (this.DisplayMode == DisplayMode.Half)
                    {
                        fanControl.RenderTransformOrigin = new Point(0.5, 0.5);
                        angle = 180;
                    }
                    else if (this.DisplayMode == DisplayMode.Horseshoe)
                    {
                        fanControl.RenderTransformOrigin = new Point(0.5, 0.5);
                        fanControl.Opacity = 0;
                        angle = 180;
                    }
                    fanControl.RenderTransform = new TransformGroup() { Children = new TransformCollection() { new RotateTransform() { Angle = angle } } };
                    grid.Children.Add(fanControl);

                    start += distance;
                    if (start >= 360)
                        start = start - 360;
                    end += distance;
                    if (end >= 360)
                        end = end - 360;
                }
            }
        }

        private void StartStoryboards(bool isOpen)
        {
            var grid = this._grid;
            if (grid != null && !_isUpdating)
            {
                _isUpdating = true;
                var storyboard = new Storyboard();
                foreach (var item in grid.Children)
                {
                    var fan = item as FanControl;
                    var animation = new DoubleAnimation();
                    animation.To = (isOpen ? 0 : this.DisplayMode == DisplayMode.Quarter ? 145 : this.DisplayMode == DisplayMode.Half ? 180 : 180);
                    animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
                    animation.EnableDependentAnimation = true;
                    animation.EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut };
                    Storyboard.SetTarget(animation, fan);
                    Storyboard.SetTargetProperty(animation, "(RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)");
                    storyboard.Children.Add(animation);
                    if (this.DisplayMode == DisplayMode.Horseshoe)
                    {
                        var opani = new DoubleAnimation
                        {
                            To = (isOpen ? 1 : 0),
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                            EnableDependentAnimation = true,
                            EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(opani, fan);
                        Storyboard.SetTargetProperty(opani, "Opacity");
                        storyboard.Children.Add(opani);
                    }
                }
                storyboard.Begin();
                this.IsOpen = isOpen;
                _isUpdating = false;
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_grid != null)
            {
                Rect rect = new Rect(-2, -2, this.RenderSize.Width + 2, this.RenderSize.Height + 2);
                if (this.DisplayMode == DisplayMode.Quarter)
                {
                    rect = new Rect(-2, -2, this.RenderSize.Width + 2, this.RenderSize.Height + 2);
                }
                else if (this.DisplayMode == DisplayMode.Half)
                {
                    rect = new Rect(-2, -2, this.RenderSize.Width + 2, (this.RenderSize.Height + 0)/2);
                }
                else if (this.DisplayMode == DisplayMode.Horseshoe)
                {
                    rect = new Rect(-2, -2, this.RenderSize.Width + 2, this.RenderSize.Height + 2);
                }
                _grid.Clip = new RectangleGeometry() { Rect = rect };
            }
            this.CreateFans();

            var button = this.GetTemplateChild("PART_Button") as PieSlice;
            var back = this.GetTemplateChild("PART_Background") as PieSlice;
            var content = this.GetTemplateChild("PART_Content") as ContentControl;
            if (this.DisplayMode == DisplayMode.Quarter)
            {
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.VerticalAlignment = VerticalAlignment.Bottom;
                button.Margin = new Thickness(0, 0, -50, -50);
            }
            else if (this.DisplayMode == DisplayMode.Half)
            {
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.StartAngle = 0;
                button.EndAngle = 359.9;
                button.Margin = new Thickness(0);
            }
            else if (this.DisplayMode == DisplayMode.Horseshoe)
            {
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.StartAngle = 0;
                button.EndAngle = 359.9;
                button.Margin = new Thickness(0);
            }
            back.HorizontalAlignment = button.HorizontalAlignment;
            back.VerticalAlignment = button.VerticalAlignment;
            back.Margin = button.Margin;
            back.StartAngle = button.StartAngle;
            back.EndAngle = button.EndAngle;
            content.HorizontalAlignment = button.HorizontalAlignment;
            content.VerticalAlignment = button.VerticalAlignment;
            if (this.IsOpen)
            {
                this.StartStoryboards(this.IsOpen);
            }
        }

        private void OnButtonPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerEnter", false);
        }

        private void OnButtonPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void OnButtonPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerPress", false);
        }

        private void OnButtonPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerEnter", false);
            this.OnClick();
            this.StartStoryboards(!this.IsOpen);
        }

        #endregion

    }

    #region FanCollection

    public class FanCollection : ObservableCollection<FanControl> { }

    #endregion

    #region DisplayMode

    public enum DisplayMode
    {
        Quarter,
        Half,
        Horseshoe
    }

    #endregion

}
