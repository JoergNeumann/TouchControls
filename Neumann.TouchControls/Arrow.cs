using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class Arrow : ContentControl
    {
        #region Private Fields

        private Path _path;

        #endregion

        #region Constructors

        public Arrow()
        {
            _path = new Path() { VerticalAlignment = VerticalAlignment.Center };
            this.Content = _path;
            _path.Fill = new SolidColorBrush(Colors.Gray);
            this.SizeChanged += OnSizeChanged;
            this.RegisterPropertyChangedCallback(
                Arrow.BackgroundProperty, new DependencyPropertyChangedCallback((d, p) => _path.Fill = this.Background));
            this.RegisterPropertyChangedCallback(
                Arrow.BorderBrushProperty, new DependencyPropertyChangedCallback((d, p) => _path.Stroke = this.BorderBrush));
            this.RegisterPropertyChangedCallback(
                Arrow.BorderThicknessProperty, new DependencyPropertyChangedCallback((d, p) => _path.StrokeThickness = this.BorderThickness.Left));
            this.RegisterPropertyChangedCallback(
                Arrow.OpacityProperty, new DependencyPropertyChangedCallback((d, p) => this.UpdateLayout()));
            this.RegisterPropertyChangedCallback(
                Arrow.VisibilityProperty, new DependencyPropertyChangedCallback((d, p) => this.UpdateLayout()));
        }

        #endregion

        #region Properties

        #region ArrowLength

        public double ArrowLength
        {
            get { return (double)GetValue(ArrowLengthProperty); }
            set { SetValue(ArrowLengthProperty, value); }
        }

        public static readonly DependencyProperty ArrowLengthProperty =
            DependencyProperty.Register("ArrowLength", typeof(double), typeof(Arrow), new PropertyMetadata(20d, OnArrowLengthPropertyChanged));

        private static void OnArrowLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Arrow;
            element.UpdatePath(element.DesiredSize);
        }

        #endregion

        #endregion

        #region Private Methods

        private void UpdatePath(Size size)
        {
            size = new Size(size.Width - (this.BorderThickness.Left * 1), size.Height - (this.BorderThickness.Top * 1));
            var strokeOffset = Math.Max(0, _path.StrokeThickness / 2);
            var triangleWidth = this.ArrowLength;
            var triangleCenterY = size.Height / 2;
            var rectWidth = size.Width - triangleWidth;
            var triangleStartX = rectWidth;

            var figure = new PathFigure
            {
                StartPoint = new Point(strokeOffset, strokeOffset),
                IsClosed = true
            };

            var recLine1 = new LineSegment();
            recLine1.Point = new Point(rectWidth, strokeOffset);
            figure.Segments.Add(recLine1);

            var triangleLine1 = new LineSegment();
            triangleLine1.Point = new Point(triangleStartX + triangleWidth, triangleCenterY);
            figure.Segments.Add(triangleLine1);

            var triangleLine2 = new LineSegment();
            triangleLine2.Point = new Point(triangleStartX, size.Height);
            figure.Segments.Add(triangleLine2);

            var recLine2 = new LineSegment();
            recLine2.Point = new Point(strokeOffset, size.Height);
            figure.Segments.Add(recLine2);

            var recLine3 = new LineSegment();
            recLine3.Point = new Point(strokeOffset, strokeOffset);
            figure.Segments.Add(recLine3);

            _path.Data = new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    figure
                }
            };
            this.UpdateLayout();
        }

        #endregion

        #region Event Handling

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePath(e.NewSize);
        }

        #endregion

    }
}
