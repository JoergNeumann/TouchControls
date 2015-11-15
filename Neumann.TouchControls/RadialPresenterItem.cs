using System;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Neumann.TouchControls
{
    public class RadialPresenterItem : ContentControl
    {
        #region Private Fields

        private bool _isLoaded;

        #endregion

        #region Constructors

        public RadialPresenterItem()
        {
            this.DefaultStyleKey = typeof(RadialPresenterItem);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region ImageSource

        public ImageSource ImageSource { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadialPresenterItem),
            new PropertyMetadata(null, OnImageSourcePropertyChanged));

        private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPresenterItem;
            element.ToggleImage();
        }

        #endregion

        #region ImagePath

        public PathGeometry ImagePathGeometry { get { return (PathGeometry)GetValue(ImagePathGeometryProperty); } set { SetValue(ImagePathGeometryProperty, value); } }
        public static readonly DependencyProperty ImagePathGeometryProperty =
            DependencyProperty.Register("ImagePathGeometry", typeof(PathGeometry), typeof(RadialPresenterItem),
            new PropertyMetadata(null, OnImagePathGeometryPropertyChanged));

        private static void OnImagePathGeometryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPresenterItem;
            element.ToggleImage();
        }

        #endregion

        #region ImagePath

        public string ImagePath { get { return (string)GetValue(ImagePathProperty); } set { SetValue(ImagePathProperty, value); } }
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(RadialPresenterItem),
            new PropertyMetadata(null, OnImagePathPropertyChanged));

        private static void OnImagePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            var element = d as RadialPresenterItem;
            element.ToggleImage();
            var path = e.NewValue as string;
            element.ImagePathGeometry = new PathGeometryParser().Parse(path);
        }

        #endregion

        #region StrokeThickness

        public double StrokeThickness { get { return (double)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RadialPresenterItem), new PropertyMetadata(4d));

        #endregion

        #region Command

        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RadialPresenterItem),
            new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPresenterItem;
            var command = e.NewValue as ICommand;
            if (command != null)
            {
                element.IsEnabled = command.CanExecute(element.CommandParameter);
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }
        }

        #endregion

        #region CommandParameter

        public object CommandParameter { get { return (object)GetValue(CommandParameterProperty); } set { SetValue(CommandParameterProperty, value); } }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(RadialPresenterItem), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        #region Click

        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }

            if (this.Command != null)
            {
                this.Command.Execute(this.DataContext);
            }
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ToggleImage();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            VisualStateManager.GoToState(this, "MouseEnter", false);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            VisualStateManager.GoToState(this, "MouseLeave", false);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            VisualStateManager.GoToState(this, "MousePress", false);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            VisualStateManager.GoToState(this, "MouseLeave", false);
            this.OnClick();
            if (((Topic)this.DataContext).Children.Count > 0)
            {
                var presenter = GetParent<RadialPresenter>(this);
                if (presenter != null)
                {
                    presenter.SelectedItem = this.DataContext as Topic;
                }
            }
        }

        #endregion

        #region Private Methods

        private void ToggleImage()
        {
            if (DesignMode.DesignModeEnabled) return;
            if (!_isLoaded) return;
            var image = this.GetTemplateChild("PART_CenterImage") as Image;
            var pathPresenter = this.GetTemplateChild("PART_CenterImagePath") as ContentControl;
            if (image != null && pathPresenter != null)
            {
                image.Visibility = this.ImagePath != null ? Visibility.Collapsed : (this.ImageSource != null ? Visibility.Visible : Visibility.Collapsed);
                pathPresenter.Visibility = this.ImagePath != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static T GetParent<T>(DependencyObject element) where T : DependencyObject
        {
            if (DesignMode.DesignModeEnabled) return null;
            if (element != null)
            {
                if (typeof(T).Equals(element.GetType()))
                {
                    return (T)element;
                }
                else
                {
                    var parent = VisualTreeHelper.GetParent(element);
                    var result = GetParent<T>(parent);
                    if (result != null)
                    {
                        return (T)result;
                    }
                }
            }
            return default(T);
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            this.ToggleImage();
            if (this.Command != null)
            {
                this.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                this.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        #endregion

    }
}
