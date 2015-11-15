using System;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class PieSlice : Path
    {
        private bool _isUpdating;

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                "StartAngle", typeof(double), typeof(PieSlice),
                new PropertyMetadata(0d, OnStartAngleChanged));

        private static void OnStartAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = (PieSlice)sender;
            target.OnStartAngleChanged();
        }

        private void OnStartAngleChanged()
        {
            UpdatePath();
        }

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(PieSlice),
                new PropertyMetadata(0d, OnEndAngleChanged));

        private static void OnEndAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = (PieSlice)sender;
            target.OnEndAngleChanged();
        }

        private void OnEndAngleChanged()
        {
            UpdatePath();
        }

        #endregion

        #region Radius

        public double Radius { get { return (double)GetValue(RadiusProperty); } set { SetValue(RadiusProperty, value); } }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(PieSlice),
                new PropertyMetadata(0d, OnRadiusChanged));

        private static void OnRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = (PieSlice)sender;
            target.OnRadiusChanged();
        }

        private void OnRadiusChanged()
        {
            Width = Height = 2 * Radius;
            UpdatePath();
        }

        #endregion

        #region Public Methods

        public PieSlice()
        {
            SizeChanged += OnSizeChanged;
            new PropertyChangeEventSource<double>(
                 this, "StrokeThickness", BindingMode.OneWay).ValueChanged +=
                 OnStrokeThicknessChanged;
        }

        #endregion

        #region Private Methods

        private void OnStrokeThicknessChanged(object sender, double e)
        {
            UpdatePath();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePath();
        }

        public void BeginUpdate()
        {
            _isUpdating = true;
        }

        public void EndUpdate()
        {
            _isUpdating = false;
            UpdatePath();
        }

        private void UpdatePath()
        {
            //if (DesignMode.DesignModeEnabled) return;
            var radius = Radius - StrokeThickness / 2;

            if (_isUpdating ||
                ActualWidth.Equals(0) ||
                radius <= 0)
            {
                return;
            }

            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure { StartPoint = new Point(radius, radius), IsClosed = true };

            var lineSegment =
                new LineSegment
                {
                    Point = new Point(
                        radius + Math.Sin(StartAngle * Math.PI / 180) * radius,
                        radius - Math.Cos(StartAngle * Math.PI / 180) * radius)
                };

            var arcSegment = new ArcSegment
            {
                IsLargeArc = (EndAngle - StartAngle) >= 180.0,
                Point = new Point(
                    radius + Math.Sin(EndAngle * Math.PI / 180) * radius,
                    radius - Math.Cos(EndAngle * Math.PI / 180) * radius),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise
            };
            pathFigure.Segments.Add(lineSegment);
            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            Data = pathGeometry;
            InvalidateArrange();
        }

        #endregion

        class PropertyChangeEventSource<TPropertyType> : FrameworkElement
        {
            public event EventHandler<TPropertyType> ValueChanged;
            private readonly DependencyObject _source;

            #region Constructors

            public PropertyChangeEventSource(DependencyObject source, string propertyName, BindingMode bindingMode = BindingMode.TwoWay)
            {
                _source = source;
                var binding = new Binding
                {
                    Source = source,
                    Path = new PropertyPath(propertyName),
                    Mode = bindingMode
                };

                SetBinding(
                    ValueProperty,
                    binding);
            }

            #endregion

            #region Value

            private TPropertyType Value
            {
                get { return (TPropertyType)GetValue(ValueProperty); }
            }

            private static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(TPropertyType), typeof(PropertyChangeEventSource<TPropertyType>),
                    new PropertyMetadata(default(TPropertyType), OnValueChanged));

            private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var target = (PropertyChangeEventSource<TPropertyType>)d;
                TPropertyType newValue = target.Value;
                target.OnValueChanged(newValue);
            }

            private void OnValueChanged(TPropertyType newValue)
            {
                var handler = ValueChanged;
                if (handler != null)
                {
                    handler(_source, newValue);
                }
            }

            #endregion
        }
    }

}