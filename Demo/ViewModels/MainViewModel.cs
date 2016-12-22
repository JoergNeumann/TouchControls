using Demo.Models;
using Demo.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Demo.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private ICollectionView _categoriesView;
        private ObservableCollection<Sample> _samples;
        private ICollectionView _samplesView;
        
        public MainViewModel()
        {
            this.CategoriesView = new CollectionViewSource { Source = Enum.GetNames(typeof(SideBarImage)).ToList() }.View;
            this.CategoriesView.CurrentChanged += this.OnCategoryChanged;

            _samples = new ObservableCollection<Sample>
            {
                new Sample { Name="Range Slider", Category=SideBarImage.Input, PageType = typeof(RangeSliderPage) },
                new Sample { Name="Radial Picker", Category=SideBarImage.Input, PageType = typeof(RadialPickerPage) },
                new Sample { Name="Number Pad", Category=SideBarImage.Input, PageType = typeof(NumberPadPage) },
                new Sample { Name="Spinner", Category=SideBarImage.Input, PageType = typeof(SpinnerPage) },

                new Sample { Name="Radial Menu", Category=SideBarImage.Commands, PageType = typeof(RadialMenuPage) },
                new Sample { Name="Radial Presenter", Category=SideBarImage.Commands, PageType = typeof(RadialPresenterPage) },
                new Sample { Name="Fan Selector", Category=SideBarImage.Commands, PageType = typeof(FanSelectorPage) },
                new Sample { Name="Filter Bar", Category=SideBarImage.Commands, PageType = typeof(FilterBarPage) },

                new Sample { Name="Miller Columns", Category=SideBarImage.Container, PageType = typeof(ColumnViewPage) },
                new Sample { Name="Radial Items Control", Category=SideBarImage.Container, PageType = typeof(RadialItemsControlPage) },
                new Sample { Name="Expander", Category=SideBarImage.Container, PageType = typeof(ExpanderPage) },
                new Sample { Name="Arrow Selection", Category=SideBarImage.Container, PageType = typeof(ArrowPage) },
            };
            this.SamplesView = new CollectionViewSource() { Source = _samples }.View;
            this.CategoriesView.MoveCurrentToFirst();
        }

        public ICollectionView CategoriesView
        {
            get { return _categoriesView; }
            set { _categoriesView = value; this.OnPropertyChanged(); }
        }

        public ICollectionView SamplesView
        {
            get { return _samplesView; }
            set { _samplesView = value; this.OnPropertyChanged(); }
        }

        private void OnCategoryChanged(object sender, object e)
        {
            if (this.CategoriesView.CurrentItem == null) return;
            var category = (SideBarImage)Enum.Parse(typeof(SideBarImage), this.CategoriesView.CurrentItem.ToString());
            var list = new ObservableCollection<Sample>(
                (_samples.Where(s => s.Category == category).Select(s => s)).ToList());
            this.SamplesView = new CollectionViewSource() { Source = list }.View;
        }
    }
}
