using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class RadialPicker : Control
    {

        #region Private Fields

        private const double OUTER_DISTANCE = -1d;
        private const double PART_LENGTH = 44d;
        private Panel _pointsPanel;
        private Panel _labelsPanel;
        private Arc _indicator;
        private Line _valueLine;
        private Line _indicatorLine;
        private Ellipse _innerCircle;
        private Ellipse _outerCircle;
        private ToolTip _toolTip;
        private RadialImageButton _centerButton;

        #endregion

        #region Constructors

        public RadialPicker()
        {
            this.DefaultStyleKey = typeof(RadialPicker);
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
            this.IsEnabledChanged += this.OnIsEnabledChanged;
        }

        #endregion

        #region Properties

        #region Minimum

        public int Minimum { get { return (int)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(0, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region Maximum

        public int Maximum { get { return (int)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(100, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region Value

        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(-1, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.PositionValue();
            element.OnValueChanged();
        }

        #endregion

        #region Distance

        public int Distance { get { return (int)GetValue(DistanceProperty); } set { SetValue(DistanceProperty, value); } }
        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(10, OnDistancePropertyChanged));

        private static void OnDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialPicker),
            new PropertyMetadata(-1d));

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(RadialPicker),
            new PropertyMetadata(-1d));

        #endregion

        #region AcceptOnlyStepValues

        public bool AcceptOnlyStepValues { get { return (bool)GetValue(AcceptOnlyStepValuesProperty); } set { SetValue(AcceptOnlyStepValuesProperty, value); } }
        public static readonly DependencyProperty AcceptOnlyStepValuesProperty =
            DependencyProperty.Register("AcceptOnlyStepValues", typeof(bool), typeof(RadialPicker),
            new PropertyMetadata(false));

        #endregion

        #region OpenWhenLoaded

        public bool OpenWhenLoaded { get { return (bool)GetValue(OpenWhenLoadedProperty); } set { SetValue(OpenWhenLoadedProperty, value); } }
        public static readonly DependencyProperty OpenWhenLoadedProperty =
            DependencyProperty.Register("OpenWhenLoaded", typeof(bool), typeof(RadialPicker), new PropertyMetadata(true));

        #endregion

        #endregion

        #region Events

        #region ValueChanged

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
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

        #region Closed

        public event EventHandler Closed;
        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            _toolTip = this.GetTemplateChild("PART_ToolTip") as ToolTip;
            _pointsPanel = this.GetTemplateChild("PART_PointsPanel") as Panel;
            _labelsPanel = this.GetTemplateChild("PART_LabelsPanel") as Panel;
            _indicator = this.GetTemplateChild("PART_Indicator") as Arc;
            _valueLine = this.GetTemplateChild("PART_ValueLine") as Line;
            _indicatorLine = this.GetTemplateChild("PART_IndicatorLine") as Line;
            _innerCircle = this.GetTemplateChild("PART_InnerCircle") as Ellipse;
            _outerCircle = this.GetTemplateChild("PART_OuterCircle") as Ellipse;
            _centerButton = this.GetTemplateChild("PART_CenterButton") as RadialImageButton;
            if (_innerCircle != null)
            {
                _innerCircle.PointerMoved += this.OnPointerMoved;
            }
            if (_outerCircle != null)
            {
                _outerCircle.PointerMoved += this.OnPointerMoved;
            }
            if (_centerButton != null)
            {
                _centerButton.Click += this.OnCenterButtonClick;
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (_indicatorLine != null)
                _indicatorLine.Visibility = Visibility.Visible;
            if (_toolTip != null)
                _toolTip.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(this, "MouseEnter", false);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (_indicatorLine != null)
                _indicatorLine.Visibility = Visibility.Collapsed;
            if (_toolTip != null)
                _toolTip.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "MouseLeave", false);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.OriginalSource is Image || e.OriginalSource is RadialImageButton)
                return;
            var pos = e.GetCurrentPoint(this).Position;
            this.Value = this.CalculateValue(pos);
            VisualStateManager.GoToState(this, "MouseLeave", false);
            this.OnClick();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (e.OriginalSource is Image || e.OriginalSource is RadialImageButton)
                return;
            var pos = e.GetCurrentPoint(this).Position;
            this.Value = this.CalculateValue(pos);
            VisualStateManager.GoToState(this, "MouseLeave", false);
            this.OnClick();
        }

        #endregion

        #region Private Methods

        private void CreatePoints()
        {
            if (_pointsPanel == null) return;
            _pointsPanel.Children.Clear();
            _labelsPanel.Children.Clear();
            var count = (this.Maximum - this.Minimum) / (this.Distance > 0 ? this.Distance : 1);
            var value = this.Minimum;
            for (int i = 0; i < count; i++)
            {
                var point = new Line()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 128, 57, 123)),
                    StrokeThickness = 2.5,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 3,
                };
                _pointsPanel.Children.Add(point);
                var label = new TextBlock()
                {
                    Text = value.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                };
                _labelsPanel.Children.Add(label);
                value += Distance;
            }
            this.PositionPoints();
        }

        private void PositionPoints()
        {
            if (_pointsPanel != null && _pointsPanel.Children.Count > 0)
            {
                var startAngle = 0;
                var count = (this.Maximum - this.Minimum) / (this.Distance > 0 ? this.Distance : 1);
                var angleDistance = 360 / count;
                for (int i = 0; i < count; i++)
                {
                    var point = _pointsPanel.Children[i] as Line;
                    var translate = new TranslateTransform();

                    var radius = (this.RenderSize.Height / 2) - OUTER_DISTANCE;
                    var angle = startAngle;
                    var radian = (angle * Math.PI) / 180;

                    translate.X = (int)radius * Math.Sin(radian);
                    translate.Y = (int)((radius * Math.Cos(radian)) * -1);

                    var group = new TransformGroup();
                    group.Children.Add(new RotateTransform() { Angle = angle });
                    group.Children.Add(translate);
                    point.RenderTransform = group;

                    if (_labelsPanel != null && _labelsPanel.Children.Count > 0)
                    {
                        var label = _labelsPanel.Children[i] as TextBlock;
                        var labelTranslate = new TranslateTransform();
                        var labelRadius = (this.RenderSize.Height / 2) - -20d;
                        var labelAngle = startAngle;
                        var labelRadian = (labelAngle * Math.PI) / 180;
                        labelTranslate.X = (int)labelRadius * Math.Sin(labelRadian);
                        labelTranslate.Y = (int)((labelRadius * Math.Cos(labelRadian)) * -1);
                        label.RenderTransform = labelTranslate;
                    }
                    startAngle += angleDistance;
                }
            }
            this.PositionValue();
        }

        private void PositionValue()
        {
            if (_valueLine == null) return;
            var startAngle = 0;
            var count = this.AcceptOnlyStepValues ? (this.Maximum - this.Minimum) / (this.Distance > 0 ? this.Distance : 1) : this.Maximum - this.Minimum;
            var angleDistance = 360 / count;
            this.StartAngle = startAngle;

            if (_indicatorLine != null && _indicatorLine.RenderTransform != null && _indicatorLine.RenderTransform is RotateTransform)
            {
                this.EndAngle = ((RotateTransform)_indicatorLine.RenderTransform).Angle;
            }
            else
            {
                this.EndAngle = this.AcceptOnlyStepValues ? (this.Value / (this.Distance > 0 ? this.Distance : 1)) * angleDistance : ((this.Value * angleDistance) * 1.19);
            }

            _valueLine.RenderTransform = new RotateTransform() { Angle = this.EndAngle };
        }

        private int CalculateValue(Point pos)
        {
            var origin = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
            var angle = this.AngleFromPoint(origin, pos);
            var count = this.AcceptOnlyStepValues ? (this.Maximum - this.Minimum) / (this.Distance > 0 ? this.Distance : 1) : this.Maximum - this.Minimum;
            var angleDistance = 360 / count;
            var value = 0;
            if (this.AcceptOnlyStepValues)
            {
                value = (int)Math.Round(((angle / angleDistance) * this.Distance) + (this.Distance / 2));
                var v = Math.Round((double)(value / (this.Distance > 0 ? this.Distance : 1))) * this.Distance;
                value = (int)v;
            }
            else
            {
                value = (int)((angle / angleDistance) / 1.19);
            }
            if (value > this.Maximum)
                value = this.Maximum;
            return value;
        }

        private double AngleFromPoint(Point origin, Point pos)
        {
            var angle = ((Math.Atan2(origin.Y - pos.Y, pos.X - origin.X) * (180.0 / Math.PI)) - 90) * -1;
            if (angle < 0)
                angle = 360 + angle;
            return angle;
        }

        private void ToggleIsEnabled()
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", false);
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CreatePoints();
            this.ToggleIsEnabled();
            if (this.OpenWhenLoaded)
            {
                VisualStateManager.GoToState(this, "Opened", false);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_valueLine != null)
                _valueLine.Y1 = (this.RenderSize.Height / 2) * -1;
            if (_indicatorLine != null)
                _indicatorLine.Y1 = (this.RenderSize.Height / 2) * -1;
            this.PositionPoints();
            if (_toolTip != null)
            {
                _toolTip.Margin = new Thickness(0, 0, (this.RenderSize.Width * 2.1) * -1, 0);
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ToggleIsEnabled();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_indicatorLine != null)
            {
                var origin = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
                var pos = e.GetCurrentPoint(this).Position;
                var angle = this.AngleFromPoint(origin, pos);
                _indicatorLine.RenderTransform = new RotateTransform() { Angle = angle };
                var value = this.CalculateValue(pos);
                if (_toolTip != null)
                {
                    _toolTip.Content = value;
                }
            }
        }

        private void OnCenterButtonClick(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Collapsed", false);
            this.OnClosed();
        }

        #endregion

    }
}
