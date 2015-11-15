using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Neumann.TouchControls
{
    public class RadialPresenter : ItemsControl
    {

        #region Private Fields

        private ContentPresenter _centerImage;
        private Button _backButton;
        private Ellipse _ellipse;
        private Storyboard _expandStoryboard;
        private Storyboard _collapsedStoryboard;

        #endregion

        #region Constructors

        public RadialPresenter()
        {
            this.DefaultStyleKey = typeof(RadialPresenter);
            this.Loaded += this.OnLoaded;
            this.IsEnabledChanged += this.OnIsEnabledChanged;
        }

        #endregion

        #region Properties

        #region ImageSource

        public ImageSource ImageSource { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadialPresenter),
            new PropertyMetadata(null));

        #endregion

        #region StrokeThickness

        public double StrokeThickness { get { return (double)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RadialPresenter), new PropertyMetadata(4d));

        #endregion

        #region IsOpen

        public bool IsOpen { get { return (bool)GetValue(IsOpenProperty); } set { SetValue(IsOpenProperty, value); } }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadialPresenter),
            new PropertyMetadata(true, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            var element = d as RadialPresenter;
            var value = (bool)e.NewValue;
            if (value)
            {
                VisualStateManager.GoToState(element, "Opened", false);
            }
            else
            {
                VisualStateManager.GoToState(element, "Closed", false);
            }
        }

        #endregion

        #region SelectedItem

        public Topic SelectedItem { get { return (Topic)GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Topic), typeof(RadialPresenter),
            new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            var element = d as RadialPresenter;
            var item = e.NewValue as Topic;
            element.BindItems(item);
        }

        #endregion

        #region ItemHierarchy

        public TopicCollection ItemHierarchy { get { return (TopicCollection)GetValue(ItemHierarchyProperty); } set { SetValue(ItemHierarchyProperty, value); } }
        public static readonly DependencyProperty ItemHierarchyProperty =
            DependencyProperty.Register("ItemHierarchy", typeof(TopicCollection), typeof(RadialPresenter),
            new PropertyMetadata(null, OnItemHierachyPropertyChanged));

        private static void OnItemHierachyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            var element = d as RadialPresenter;
            var itemHierarchy = e.NewValue as TopicCollection;
            if (itemHierarchy != null)
            {
                element.BindItems(null);
            }
        }

        #endregion

        #region CenterContent

        public object CenterContent { get { return (object)GetValue(CenterContentProperty); } set { SetValue(CenterContentProperty, value); } }
        public static readonly DependencyProperty CenterContentProperty =
            DependencyProperty.Register("CenterContent", typeof(object), typeof(RadialPresenter), new PropertyMetadata(null));

        #endregion

        #region CenterContentPresenter

        public DataTemplate CenterContentTemplate { get { return (DataTemplate)GetValue(CenterContentTemplateProperty); } set { SetValue(CenterContentTemplateProperty, value); } }
        public static readonly DependencyProperty CenterContentTemplateProperty =
            DependencyProperty.Register("CenterContentTemplate", typeof(DataTemplate), typeof(RadialPresenter), new PropertyMetadata(null));
        
        #endregion
        
        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            if (DesignMode.DesignModeEnabled) return;
            var grid = this.GetTemplateChild("root") as Grid;
            _ellipse = this.GetTemplateChild("Ellipse") as Ellipse;
            _expandStoryboard = grid.Resources["ExpandStoryboard"] as Storyboard;
            _collapsedStoryboard = grid.Resources["CollapsedStoryboard"] as Storyboard;
            _centerImage = this.GetTemplateChild("PART_CenterImage") as ContentPresenter;
            _backButton = this.GetTemplateChild("PART_BackButton") as Button;
            if (_backButton != null)
            {
                _backButton.Click += this.OnBackButtonClick;
            }
        }

        #endregion

        #region Private Methods

        private void BindItems(Topic parent, bool animate = true)
        {
            if (_collapsedStoryboard == null || _expandStoryboard == null ||
                (parent != null && (parent.Children == null || parent.Children.Count == 0)) ||
                DesignMode.DesignModeEnabled)
                return;

            if (animate)
            {
                _collapsedStoryboard.Completed += (s, e) =>
                    {
                        var backVisible = false;
                        if (parent != null)
                        {
                            this.ItemsSource = parent.Children;
                            backVisible = true;
                        }
                        else
                        {
                            this.ItemsSource = this.ItemHierarchy;
                            backVisible = false;
                        }
                        if (_centerImage != null && _backButton != null)
                        {
                            _centerImage.Visibility = backVisible ? Visibility.Collapsed : Visibility.Visible;
                            _centerImage.Opacity = backVisible ? 0 : 1;
                            _backButton.Visibility = backVisible ? Visibility.Visible : Visibility.Collapsed;
                            _backButton.Opacity = backVisible ? 1 : 0;
                        }
                        _expandStoryboard.Begin();
                    };
                _collapsedStoryboard.Begin();
            }
            else
            {
                this.ItemsSource = this.ItemHierarchy;
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            if (this.ItemsSource == null && this.ItemHierarchy != null)
            {
                this.BindItems(this.SelectedItem, false);
            }
            if (IsOpen)
            {
                VisualStateManager.GoToState(this, "Opened", false);
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.IsHitTestVisible = (bool)e.NewValue;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItem = this.SelectedItem.Parent;
            }
        }

        #endregion

    }

    #region TopicCollection

    public class TopicCollection : ObservableCollection<Topic>
    {
    }

    #endregion

    #region Topic

    public class Topic : INotifyPropertyChanged
    {
        public Topic()
        {
            this.Children = new TopicCollection();
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; this.OnPropertyChanged(); }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; this.OnPropertyChanged(); }
        }
        
        private string _imagePathResourceName;
        public string ImagePathResourceName
        {
            get { return _imagePathResourceName; }
            set { _imagePathResourceName = value; this.OnPropertyChanged(); }
        }
        
        private TopicCollection _children;
        public TopicCollection Children
        {
            get { return _children; }
            set
            {
                if (value != null && value != _children)
                {
                    _children = value;
                    foreach (var item in _children)
                    {
                        item.Parent = this;
                    }
                    this.OnPropertyChanged();
                }
            }
        }

        private Topic _parent;
        public Topic Parent
        {
            get { return _parent; }
            set { _parent = value; this.OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

}
