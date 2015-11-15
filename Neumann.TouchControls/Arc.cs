using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class Arc : Control
    {

        #region Private Fields

        private const string TrailPartName = "PART_Trail";
        private const double Degrees2Radians = Math.PI / 180;

        #endregion

        #region Constructors

        public Arc()
        {
            this.DefaultStyleKey = typeof(Arc);
            this.Loaded += Arc_Loaded;
        }

        void Arc_Loaded(object sender, RoutedEventArgs e)
        {
            this.Draw();
        }

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Draw();
        }

        #endregion

        #region Private Methods

        private void Draw()
        {
            if (this.DesiredSize.Width == 0)
                return;
            var middleOfScale = (this.DesiredSize.Width - this.StrokeThickness) / 2;
            var trail = this.GetTemplateChild(TrailPartName) as Path;
            if (trail != null)
            {
                trail.Visibility = Visibility.Visible;
                var pg = new PathGeometry();
                var pf = new PathFigure();
                pf.IsClosed = false;
                pf.StartPoint = this.ScalePoint(this.StartAngle, middleOfScale);
                var seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Clockwise;
                seg.IsLargeArc = (this.EndAngle - this.StartAngle) > 180;
                seg.Size = new Size(middleOfScale, middleOfScale);
                seg.Point = this.ScalePoint(this.EndAngle, middleOfScale);
                pf.Segments.Add(seg);
                pg.Figures.Add(pf);
                trail.Data = pg;
            }
        }

        private Point ScalePoint(double angle, double middleOfScale)
        {
            return new Point(this.Radius + Math.Sin(Degrees2Radians * angle) * middleOfScale, this.Radius - Math.Cos(Degrees2Radians * angle) * middleOfScale);
        }

        #endregion

        #region Properties

        #region StrokeThickness

        public Double StrokeThickness { get { return (Double)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(Double), typeof(Arc), new PropertyMetadata(10.0));

        #endregion

        #region Stroke

        public Brush Stroke { get { return (Brush)GetValue(StrokeProperty); } set { SetValue(StrokeProperty, value); } }
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Arc), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(Arc), new PropertyMetadata(0d, OnStartAngleChanged));

        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Arc)d).Draw();
        }

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(Arc), new PropertyMetadata(180d, OnEndAngleChanged));
        private static void OnEndAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Arc)d).Draw();
        }

        #endregion

        #region Radius

        public double Radius { get { return (double)GetValue(RadiusProperty); } set { SetValue(RadiusProperty, value); } }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(Arc), new PropertyMetadata(50d));

        #endregion

        #endregion

    }
}
