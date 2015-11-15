using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Neumann.TouchControls
{
    [ContentProperty(Name = "Sections")]
    public class ColumnView : Control
    {

        #region Private Fields

        private Panel _panel;
        private bool _updating;
        private bool IsLoaded;

        #endregion

        #region Constructors

        public ColumnView()
        {
            this.DefaultStyleKey = typeof(ColumnView);
            this.Sections = new ColumnViewSectionCollection();
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties

        #region Sections

        public ColumnViewSectionCollection Sections { get { return (ColumnViewSectionCollection)GetValue(SectionsProperty); } set { SetValue(SectionsProperty, value); } }
        public static readonly DependencyProperty SectionsProperty =
            DependencyProperty.Register("Sections", typeof(ColumnViewSectionCollection), typeof(ColumnView),
            new PropertyMetadata(null));

        #endregion

        #region CurrentSection

        public ColumnViewSection CurrentSection { get { return (ColumnViewSection)GetValue(CurrentSectionProperty); } set { SetValue(CurrentSectionProperty, value); } }
        public static readonly DependencyProperty CurrentSectionProperty =
            DependencyProperty.Register("CurrentSection", typeof(ColumnViewSection), typeof(ColumnView),
            new PropertyMetadata(null, OnCurrentSectionPropertyChanged));

        private static void OnCurrentSectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ColumnView;
            var section = e.NewValue as ColumnViewSection;
            if (section != null && !element._updating)
            {
                element.VisibleSectionCount = element.Sections.IndexOf(section) + 1;
                element.OnCurrentSectionChanged();
            }
        }

        #endregion

        #region SelectedIndex

        public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ColumnView),
            new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ColumnView;
            var index = (int)e.NewValue;
            if (index < element.Sections.Count)
            {
                var p = element.Sections[index];
                if (p == null || p.Parent == null) return;
                element.VisibleSectionCount = (int)e.NewValue + 2;
            }
        }

        #endregion

        #region VisibleSectionCount

        public int VisibleSectionCount { get { return (int)GetValue(VisibleSectionCountProperty); } set { SetValue(VisibleSectionCountProperty, value); } }
        public static readonly DependencyProperty VisibleSectionCountProperty =
            DependencyProperty.Register("VisibleSectionCount", typeof(int), typeof(ColumnView),
            new PropertyMetadata(1, OnVisibleSectionCountPropertyChanged));

        private static void OnVisibleSectionCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ColumnView;
            var count = (int)e.NewValue;
            if (element == null || element.Sections == null || element._panel == null) return;
            if (count > element.Sections.Count) return;
            for (int i = 0; i < element.Sections.Count; i++)
            {
                var section = element.Sections[i];
                if (i < count)
                {
                    section.Width = 0;
                    if (!element._panel.Children.Contains(section))
                    {
                        element._panel.Children.Add(section);
                    }
                    element._updating = true;
                    element.CurrentSection = section;
                    element.OnCurrentSectionChanged();
                    element._updating = false;
                }
            }

            element.CalculateSectionWidth();

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += element.RemoveSection;
            timer.Start();
        }

        private void RemoveSection(object sender, object e)
        {
            var timer = sender as DispatcherTimer;
            timer.Stop();
            timer.Tick -= this.RemoveSection;
            for (int i = 0; i < this.Sections.Count; i++)
            {
                var section = this.Sections[i];
                if (i >= this.VisibleSectionCount &&
                    this._panel.Children.Contains(section))
                {
                    this._panel.Children.Remove(section);
                }
            }
        }

        #endregion

        #region NormalizeColumnWidth

        public bool NormalizeColumnWidth { get { return (bool)GetValue(NormalizeColumnWidthProperty); } set { SetValue(NormalizeColumnWidthProperty, value); } }
        public static readonly DependencyProperty NormalizeColumnWidthProperty =
            DependencyProperty.Register("NormalizeColumnWidth", typeof(bool), typeof(ColumnView),
            new PropertyMetadata(false));

        #endregion

        #endregion

        #region Events

        public event EventHandler CurrentSectionChanged;
        protected virtual void OnCurrentSectionChanged()
        {
            if (CurrentSectionChanged != null)
                CurrentSectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = this.GetTemplateChild("PART_Panel") as Panel;

        }

        #endregion

        #region Private Methods

        private void AssignTransformation(ColumnViewSection section)
        {
            section.RenderTransformOrigin = new Point(0.5, 0.5);
            var trans = new ScaleTransform() { ScaleX = 0 };
            var group = new TransformGroup();
            group.Children.Add(trans);
            section.RenderTransform = group;
        }

        private void AssignAndStartStoryboard(ColumnViewSection section, Timeline animation)
        {
            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            //Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");
            //Storyboard.SetTarget(animation, section);

            Storyboard.SetTargetProperty(animation, "Width");
            Storyboard.SetTarget(storyboard, section);
            storyboard.Begin();
        }

        private void CalculateSectionWidth()
        {
            if (this.Sections.Count == 0 || this.VisibleSectionCount > this.Sections.Count) return;

            var width = this.RenderSize.Width;
            if (this.VisibleSectionCount > 1)
            {
                var normalWidth = (width / (this.VisibleSectionCount));
                var expandedWidth = this.NormalizeColumnWidth ? normalWidth * 1 : normalWidth * 1.8;
                width -= expandedWidth;
                width = width / (this.VisibleSectionCount - 1);
                if (this.VisibleSectionCount == 2)
                {
                    width = this.NormalizeColumnWidth ? (this.RenderSize.Width / 2) : (this.RenderSize.Width / 2) * 0.8;
                    expandedWidth = this.RenderSize.Width - width;
                }
                for (int i = 0; i < this.VisibleSectionCount - 1; i++)
                {
                    var section = this.Sections[i];
                    //this.AssignTransformation(section);
                    //section.Width = width;

                    var ani1 = new DoubleAnimation();
                    ani1.From = section.RenderSize.Width;
                    ani1.To = width;// 1;
                    ani1.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                    ani1.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                    ani1.EnableDependentAnimation = true;

                    this.AssignAndStartStoryboard(section, ani1);
                }

                this.Sections[this.VisibleSectionCount - 1].Width = expandedWidth;

                var ani2 = new DoubleAnimation();
                ani2.To = expandedWidth;// 1.8;
                ani2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                ani2.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
                ani2.EnableDependentAnimation = true;
                var sec = this.Sections[this.VisibleSectionCount - 1];

                //var sboard = new Storyboard();
                //sboard.Children.Add(ani2);
                //Storyboard.SetTargetProperty(ani2, "Width");
                //Storyboard.SetTarget(sboard, sec);
                //sboard.Begin();

                //this.AssignTransformation(sec);
                this.AssignAndStartStoryboard(sec, ani2);
            }
            else
            {
                var ani1 = new DoubleAnimation();
                ani1.To = width;
                ani1.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 300));
                ani1.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                ani1.EnableDependentAnimation = true;
                //this.Sections[0].BeginAnimation(ColumnViewSection.WidthProperty, ani1);
                var section = this.Sections[0];
                var storyboard = new Storyboard();
                storyboard.Children.Add(ani1);
                Storyboard.SetTargetProperty(ani1, "Width");
                Storyboard.SetTarget(storyboard, section);
                storyboard.Begin();
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            if (_panel == null) return;
            _panel.Children.Clear();

            for (int i = 0; i < this.Sections.Count; i++)
            {
                var section = this.Sections[i];
                if (i < this.VisibleSectionCount)
                {
                    _panel.Children.Add(section);
                }
                section.Width = 100;
            }
            this.CalculateSectionWidth();
            if (this.Sections.Count > 0 && this.SelectedIndex < this.Sections.Count)
            {
                _updating = true;
                if (this.SelectedIndex > -1)
                    this.CurrentSection = this.Sections[this.SelectedIndex];
                else
                    this.CurrentSection = this.Sections[0];
                _updating = false;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded)
                this.CalculateSectionWidth();
        }

        #endregion

    }

    #region ColumnViewSectionCollection

    public class ColumnViewSectionCollection : Collection<ColumnViewSection>
    {
    }

    #endregion

}
