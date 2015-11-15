using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class Spinner : Control
    {

        #region Private Fields

        private bool _tapped;
        private CompositeTransform _transform = new CompositeTransform();
        private PieSlice _slice;
        private Ellipse _invisibleEllipse;
        private Canvas _circle;
        private Rectangle _slider;

        #endregion

        #region Constructors

        public Spinner()
        {
            this.DefaultStyleKey = typeof(Spinner);
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        #endregion

        #region Properties

        #region Label

        public string Label { get { return (string)GetValue(LabelProperty); } set { SetValue(LabelProperty, value); } }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(Spinner),
            new PropertyMetadata(" "));

        #endregion

        #region LabelTemplate

        public DataTemplate LabelTemplate { get { return (DataTemplate)GetValue(LabelTemplateProperty); } set { SetValue(LabelTemplateProperty, value); } }
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(Spinner),
            new PropertyMetadata(null));

        #endregion

        #region Minimum

        public int Minimum { get { return (int)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(Spinner),
            new PropertyMetadata(0, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Spinner;
            element.CheckRange();
        }

        #endregion

        #region Maximum

        public int Maximum { get { return (int)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(Spinner),
            new PropertyMetadata(100, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Spinner;
            element.CheckRange();
        }

        #endregion

        #region Value

        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(Spinner),
            new PropertyMetadata(50, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Spinner;
            element.CheckRange();
        }

        #endregion

        #region SelectionColor

        public Brush SelectionColor { get { return (Brush)GetValue(SelectionColorProperty); } set { SetValue(SelectionColorProperty, value); } }
        public static readonly DependencyProperty SelectionColorProperty =
            DependencyProperty.Register("SelectionColor", typeof(Brush), typeof(Spinner),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _slice = GetTemplateChild("Slice") as PieSlice;
            _circle = GetTemplateChild("Circle") as Canvas;
            _slider = GetTemplateChild("Slider") as Rectangle;
            _invisibleEllipse = GetTemplateChild("InvisibleEllipse") as Ellipse;

            if (_circle != null)
            {
                _transform.CenterX = _circle.Width / 2.0;
                _transform.CenterY = _circle.Height / 2.0;
                _circle.RenderTransform = _transform;
            }

            if (this.Maximum == 0)
            {
                this.Maximum = 1;
            }
            double angle = (this.Value * (double)360) / Maximum;
            this.UpdateLayout(angle, false);
        }

        #endregion

        #region Private Methods

        private void RegisterEvents()
        {
            Unloaded -= OnUnloaded;
            Unloaded += OnUnloaded;

            if (_invisibleEllipse == null)
                return;

            _invisibleEllipse.PointerPressed -= OnPointerPressed;
            _invisibleEllipse.PointerMoved -= OnPointerMoved;
            _invisibleEllipse.PointerReleased -= OnPointerReleased;
            _invisibleEllipse.PointerExited -= OnPointerReleased;
            _invisibleEllipse.PointerPressed += OnPointerPressed;
            _invisibleEllipse.PointerMoved += OnPointerMoved;
            _invisibleEllipse.PointerReleased += OnPointerReleased;
            _invisibleEllipse.PointerExited += OnPointerReleased;
        }

        private void UnRegisterEvents()
        {
            Unloaded -= OnUnloaded;

            if (_invisibleEllipse == null)
                return;

            _invisibleEllipse.PointerPressed -= OnPointerPressed;
            _invisibleEllipse.PointerMoved -= OnPointerMoved;
            _invisibleEllipse.PointerReleased -= OnPointerReleased;
            _invisibleEllipse.PointerExited -= OnPointerReleased;
        }

        private void UpdateLayout(double angle, bool touchInput)
        {
            if (!(this.Maximum >= 0 || this.Maximum == 0 && angle < 0))
            {
                return;
            }

            if (_transform.Rotation >= 0 && _transform.Rotation <= 360 && Maximum != 0)
            {
                _transform = _circle.RenderTransform as CompositeTransform;
                if (_transform != null)
                {
                    if (touchInput)
                    {
                        _transform.Rotation += angle;
                    }
                    else
                    {
                        _transform.Rotation = angle;
                    }
                    _slice.EndAngle = _transform.Rotation - 2;

                    if (_slice.EndAngle > 359)
                    {
                        _slice.EndAngle = 359;
                    }

                    if (_transform.Rotation > 360)
                    {
                        _transform.Rotation = 360;
                    }

                    if (_transform.Rotation < 0)
                    {
                        _transform.Rotation = 0;
                        _slice.EndAngle = 0;
                    }

                    this.Value = Convert.ToInt32(_transform.Rotation / (360 / (double)Maximum));
                    if (this.Value > Maximum)
                    {
                        this.Value = Maximum;
                    }
                }
            }

            if (_transform != null)
            {
                if ((_transform.Rotation < 0 && angle > 0) || (_transform.Rotation.Equals(360) && angle < 0))
                {
                    _transform = _circle.RenderTransform as CompositeTransform;
                    if (_transform != null)
                    {
                        if (touchInput)
                        {
                            _transform.Rotation += angle;
                        }
                        else
                        {
                            _transform.Rotation = angle;
                        }
                        _slice.EndAngle = _transform.Rotation - 2;
                    }
                }

                if (this.Maximum < 0)
                {
                    this.Value += this.Maximum;
                    _slice.EndAngle = this.Value * (360 / Convert.ToDouble(Maximum));
                    if (_transform != null) _transform.Rotation = this.Value * (360 / Convert.ToDouble(Maximum));
                    this.Maximum = 0;
                }
            }
        }

        private void CheckRange()
        {
            if (this.Value < this.Minimum)
            {
                this.Value = this.Minimum;
            }
            if (this.Value > this.Maximum)
            {
                this.Value = this.Maximum;
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnRegisterEvents();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_tapped)
            {
                return;
            }
            _tapped = false;
            e.Handled = true;
            _slider.Opacity += 0.3;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(_circle);
            if ((pt.Position.X >= Canvas.GetLeft(_slider) - 20 && pt.Position.X <= Canvas.GetLeft(_slider) + 34) &&
                (pt.Position.Y >= 0 && pt.Position.Y <= Canvas.GetTop(_slider) + 42))
            {
                _tapped = true;
                _slider.Opacity -= 0.3;
            }

            e.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_tapped)
            {
                var pt = e.GetCurrentPoint(_slider);
                this.UpdateLayout(pt.Position.X, true);
            }
        }

        #endregion

    }
}
