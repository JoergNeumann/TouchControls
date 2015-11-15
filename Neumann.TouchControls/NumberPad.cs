using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Neumann.TouchControls
{
    public class NumberPad : Control
    {

        #region Private Fields

        private RepeatButton _minusPad;
        private RepeatButton _plusPad;
        private bool _isInitializing = true;

        #endregion

        #region Constructors

        public NumberPad()
        {
            this.DefaultStyleKey = typeof(NumberPad);
        }

        #endregion

        #region Properties

        #region Minimum

        public int Minimum { get { return (int)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumberPad),
            new PropertyMetadata(0, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as NumberPad;
            element.CheckRange();
        }

        #endregion

        #region Maximum

        public int Maximum { get { return (int)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(NumberPad),
            new PropertyMetadata(0, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as NumberPad;
            element.CheckRange();
        }

        #endregion

        #region Value

        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumberPad),
            new PropertyMetadata(0, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as NumberPad;
            element.CheckRange();
        }

        #endregion

        #region PointerOverBrush

        public Brush PointerOverBrush { get { return (Brush)GetValue(PointerOverBrushProperty); } set { SetValue(PointerOverBrushProperty, value); } }
        public static readonly DependencyProperty PointerOverBrushProperty =
            DependencyProperty.Register("PointerOverBrush", typeof(Brush), typeof(NumberPad),
            new PropertyMetadata(null));

        #endregion

        #region Subtext

        public string SubText { get { return (string)GetValue(SubTextProperty); } set { SetValue(SubTextProperty, value); } }
        public static readonly DependencyProperty SubTextProperty =
            DependencyProperty.Register("SubText", typeof(string), typeof(NumberPad),
            new PropertyMetadata(" "));

        #endregion

        #region SubTextTemplate

        public DataTemplate SubTextTemplate { get { return (DataTemplate)GetValue(SubTextTemplateProperty); } set { SetValue(SubTextTemplateProperty, value); } }
        public static readonly DependencyProperty SubTextTemplateProperty =
            DependencyProperty.Register("SubTextTemplate", typeof(DataTemplate), typeof(NumberPad), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _isInitializing = true;
            _minusPad = this.GetTemplateChild("PART_MinusPad") as RepeatButton;
            _plusPad = this.GetTemplateChild("PART_PlusPad") as RepeatButton;
            if (_minusPad != null && _plusPad != null)
            {
                _minusPad.Click += this.OnPadClick;
                _plusPad.Click += this.OnPadClick;
                _minusPad.PointerEntered += this.OnPadPointerEntered;
                _plusPad.PointerEntered += this.OnPadPointerEntered;
                _minusPad.PointerExited += this.OnPadPointerExited;
                _plusPad.PointerExited += this.OnPadPointerExited;
                _minusPad.PointerReleased += this.OnPadPointerReleased;
                _plusPad.PointerReleased += this.OnPadPointerReleased;
            }
            this.CheckRange();
        }
        
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _isInitializing = false;
        }

        #endregion

        #region Private Methods

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

        private void OnPadPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (_isInitializing && e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }
            if (sender == _minusPad)
            {
                VisualStateManager.GoToState(this, "MinusPointerOver", false);
            }
            else if (sender == _plusPad)
            {
                VisualStateManager.GoToState(this, "PlusPointerOver", false);
            }
        }

        private void OnPadPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void OnPadClick(object sender, RoutedEventArgs e)
        {
            if (sender == _minusPad)
            {
                VisualStateManager.GoToState(this, "MinusPressed", false);
                if (this.Value > this.Minimum)
                {
                    this.Value--;
                }
            }
            else if (sender == _plusPad)
            {
                VisualStateManager.GoToState(this, "PlusPressed", false);
                if (this.Value < this.Maximum)
                {
                    this.Value++;
                }
            }
        }

        private void OnPadPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender == _minusPad)
            {
                VisualStateManager.GoToState(this, "MinusPointerOver", false);
            }
            else if (sender == _plusPad)
            {
                VisualStateManager.GoToState(this, "PlusPointerOver", false);
            }
        }

        #endregion

    }
}
