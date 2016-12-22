using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class RadialItemsControl : ItemsControl
    {

        #region Private Fields

        private Canvas _surface;
        private CircleItemsPanel _panel;
        private Panel _rootPanel;
        private List<Transition> _transitions = new List<Transition>();
        private bool _isInitializing = true;

        #endregion

        #region Constructors

        public RadialItemsControl()
        {
            this.DefaultStyleKey = typeof(RadialItemsControl);
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properites

        #region Header

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RadialItemsControl), new PropertyMetadata(null));

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(RadialItemsControl), new PropertyMetadata(null));

        #endregion

        #region LineStyle

        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }

        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register("LineStyle", typeof(Style), typeof(RadialItemsControl), new PropertyMetadata(null));

        #endregion

        #region ShowLines

        public bool ShowLines
        {
            get { return (bool)GetValue(ShowLinesProperty); }
            set { SetValue(ShowLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowLinesProperty =
            DependencyProperty.Register("ShowLines", typeof(bool), typeof(RadialItemsControl), new PropertyMetadata(true, OnShowLinesPropertyChanged));

        private static void OnShowLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialItemsControl;
            element.DrawLines();
        }

        #endregion

        #region Radius

        public double Radius { get { return (double)GetValue(RadiusProperty); } set { SetValue(RadiusProperty, value); } }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(RadialItemsControl),
            new PropertyMetadata(200d, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialItemsControl;
            if (element._panel != null)
            {
                element._panel.Radius = (double)e.NewValue;
            }
            element.UpdateLayout();
            element.DrawLines();
        }

        #endregion

        #region Panel

        internal Panel Panel
        {
            get { return (Panel)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        internal static readonly DependencyProperty PanelProperty =
            DependencyProperty.Register("Panel", typeof(Panel), typeof(RadialItemsControl), new PropertyMetadata(null));

        #endregion

        #region Reset

        public bool Reset
        {
            get { return (bool)GetValue(ResetProperty); }
            set { SetValue(ResetProperty, value); }
        }

        public static readonly DependencyProperty ResetProperty =
            DependencyProperty.Register("Reset", typeof(bool), typeof(RadialItemsControl), new PropertyMetadata(false, OnResetPropertyChanged));

        private static void OnResetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialItemsControl;
            var value = (bool)e.NewValue;
            if (value)
            {
                element.ToggleItemTransitions(false);
                element.StartAnimation();
            }
            else
            {
                element.ToggleItemTransitions(true);
            }
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _surface = this.GetTemplateChild("PART_Surface") as Canvas;
            _rootPanel = this.GetTemplateChild("PART_RootPanel") as Panel;
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (_isInitializing)
            {
                this.ToggleItemTransitions(false);
                return;
            }

            if (_surface != null && !_isInitializing)
            {
                this.AnimateSurfaceOpacity(false, () => _surface.Children.Clear());
                var timer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 0, 500),
                };
                timer.Tick += (s, a) =>
                {
                    this.UpdateLayout();
                    this.DrawLines();
                    timer.Stop();
                    this.AnimateSurfaceOpacity(true);
                };
                timer.Start();
            }
        }

        #endregion

        #region Private Methods

        private void DrawLines()
        {
            if (_surface != null)
            {
                _surface.Children.Clear();
                if (this.ShowLines)
                {
                    foreach (var item in this.Items)
                    {
                        var element = this.ContainerFromItem(item) as UIElement;
                        var transform = element.TransformToVisual(this);
                        var position = transform.TransformPoint(new Point(0, 0));
                        var itemSize = new Size(element.RenderSize.Width / 2, element.RenderSize.Height / 2);
                        var itemCenter = new Point(position.X + itemSize.Width, position.Y + itemSize.Height);
                        var center = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
                        var line = this.CreateLine();
                        line.X1 = itemCenter.X;
                        line.Y1 = itemCenter.Y;
                        line.X2 = center.X;
                        line.Y2 = center.Y;
                        _surface.Children.Add(line);
                    }
                }
            }
        }

        private Line CreateLine()
        {
            var line = new Line
            {
                Style = this.LineStyle,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransform = new TranslateTransform()
            };
            return line;
        }

        private void StartAnimation()
        {
            var animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = this.Radius;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            animation.EnableDependentAnimation = true;

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTargetProperty(animation, "Radius");
            Storyboard.SetTarget(storyboard, _panel);
            storyboard.Begin();
            storyboard.Completed += (s, e) =>
            {
                this.Reset = false;
                this.ToggleItemTransitions(true);
                _isInitializing = false;
            };
        }

        private void AnimateSurfaceOpacity(bool visible, Action completedHandler = null)
        {
            if (_surface != null)
            {
                var animation = new DoubleAnimation();
                animation.From = visible ? 0 : 1;
                animation.To = visible ? 1 : 0;
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, visible ? 300 : 100));
                animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                animation.EnableDependentAnimation = true;

                var storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, "Opacity");
                Storyboard.SetTarget(storyboard, _surface);
                storyboard.Begin();
                if (completedHandler != null)
                {
                    storyboard.Completed += (s, a) => completedHandler();
                }
            }
        }

        private void ToggleItemTransitions(bool add)
        {
            if (add)
            {
                this.ItemContainerTransitions = new TransitionCollection()
                {
                    new AddDeleteThemeTransition()
                };
            }
            else
            {
                if (this.ItemContainerTransitions != null)
                {
                    this.ItemContainerTransitions = null;
                }
            }
        }

        #endregion

        #region Event Handling

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

            _panel = VisualTreeHelpers.GetVisualChild<CircleItemsPanel>(this);
            if (_panel != null)
            {
                this.Panel = _panel;
                _panel.Radius = this.Radius;
                this.UpdateLayout();
                if (!DesignMode.DesignModeEnabled)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => this.DrawLines()));
                    _panel.RegisterPropertyChangedCallback(CircleItemsPanel.RadiusProperty, (s, p) =>
                    {
                        this.DrawLines();
                    });
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        this.StartAnimation();
                    }));
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SizeChanged -= this.OnSizeChanged;
            this.DrawLines();
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

    }
}
