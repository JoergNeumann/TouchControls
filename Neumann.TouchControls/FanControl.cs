using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Neumann.TouchControls
{
    public class FanControl : ContentControl
    {

        #region Private Fields

        private TextBlock _label;

        #endregion

        #region Constructors

        public FanControl()
        {
            this.DefaultStyleKey = typeof(FanControl);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(FanControl), new PropertyMetadata(0d));

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(FanControl), new PropertyMetadata(0d));

        #endregion

        #region Radius

        public double Radius { get { return (double)GetValue(RadiusProperty); } set { SetValue(RadiusProperty, value); } }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(FanControl), new PropertyMetadata(0d));

        #endregion

        #region ContentStyle

        public Style ContentStyle { get { return (Style)GetValue(ContentStyleProperty); } set { SetValue(ContentStyleProperty, value); } }
        public static readonly DependencyProperty ContentStyleProperty =
            DependencyProperty.Register("ContentStyle", typeof(Style), typeof(FanControl), new PropertyMetadata(null));

        #endregion

        #region Command

        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FanControl),
            new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FanControl;
            var command = e.NewValue as ICommand;
            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.IsEnabled = this.Command.CanExecute(null);
        }

        #endregion

        #region DisplayMode

        public DisplayMode DisplayMode { get { return (DisplayMode)GetValue(DisplayModeProperty); } set { SetValue(DisplayModeProperty, value); } }
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode ", typeof(DisplayMode), typeof(FanControl), new PropertyMetadata(DisplayMode.Quarter));

        #endregion

        #region IsRotatingContent

        public bool IsRotatingContent { get { return (bool)GetValue(IsRotatingContentProperty); } set { SetValue(IsRotatingContentProperty, value); } }
        public static readonly DependencyProperty IsRotatingContentProperty =
            DependencyProperty.Register("IsRotatingContent", typeof(bool), typeof(FanControl), new PropertyMetadata(true));

        #endregion

        #region Distance

        internal int Distance { get; set; }

        #endregion

        #endregion

        #region Events

        #region Click

        public event EventHandler Click;

        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
            if (this.Command != null)
                this.Command.Execute(null);
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _label = this.GetTemplateChild("PART_Content") as TextBlock;
            this.PositionLabel();
        }

        #endregion

        #region Public Functions

        public void PositionLabel2()
        {
            if (_label == null || _label.RenderTransform == null || !(_label.RenderTransform is TransformGroup))
                return;
            var group = _label.RenderTransform as TransformGroup;
            var rotate = group.Children[0] as RotateTransform;
            var translate = group.Children[1] as TranslateTransform;

            var outerSpacing = (this.Radius) - 40;
            var h = this.Distance / 2;
            var mi = this.StartAngle + h + 90;
            Double radian = (mi * Math.PI) / 180;

            if (this.IsRotatingContent)
            {
                rotate.Angle = mi;
            }

            translate.X = (int)(outerSpacing * Math.Cos(radian) * -1);
            translate.Y = (int)(outerSpacing * Math.Sin(radian) * -1);
        }
        public void PositionLabel()
        {
            if (_label == null || _label.RenderTransform == null || !(_label.RenderTransform is TransformGroup))
                return;
            if (this.DisplayMode == DisplayMode.Quarter)
            {
                _label.HorizontalAlignment = HorizontalAlignment.Right;
                _label.VerticalAlignment = VerticalAlignment.Bottom;
            }
            var group = _label.RenderTransform as TransformGroup;
            var rotate = group.Children[0] as RotateTransform;
            var translate = group.Children[1] as TranslateTransform;

            var outerSpacing = (this.Radius) - 40;
            if (this.DisplayMode == DisplayMode.Quarter)
                outerSpacing = (this.Radius) - 80;
            var h = this.Distance / 2;
            var mi = this.StartAngle + h + 90;
            Double radian = (mi * Math.PI) / 180;

            if (this.IsRotatingContent)
            {
                rotate.Angle = mi;
            }

            translate.X = (int)(outerSpacing * Math.Cos(radian) * -1);
            translate.Y = (int)(outerSpacing * Math.Sin(radian) * -1);
        }
        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Command != null)
            {
                this.IsEnabled = this.Command.CanExecute(null);
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerEnter", false);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerPress", false);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerEnter", false);
            this.OnClick();
            var selector = this.GetParent<FanSelector>();
            if (selector != null)
            {
                selector.OnSelectionChanged();
            }
        }

        #endregion

    }
}
