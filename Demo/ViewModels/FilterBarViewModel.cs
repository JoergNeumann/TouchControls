using Demo.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

namespace Demo.ViewModels
{
    public class FilterBarViewModel : ModelBase
    {
        private ObservableCollection<int> _years;
        private ICollectionView _yearsView;

        public FilterBarViewModel()
        {
            _years = new ObservableCollection<int>
            {
                2014,2015,2016,2017
            };
            this.YearsView = new CollectionViewSource() { Source = _years }.View;
            this.YearsView.MoveCurrentToFirst();
        }

        public ICollectionView YearsView
        {
            get { return _yearsView; }
            set { _yearsView = value; this.OnPropertyChanged(); }
        }
    }
}
