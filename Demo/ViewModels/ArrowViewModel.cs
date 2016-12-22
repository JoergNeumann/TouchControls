using Demo.Models;
using System.Collections.ObjectModel;

namespace Demo.ViewModels
{
    public class ArrowViewModel : ModelBase
    {
        private ObservableCollection<string> _items;

        public ArrowViewModel()
        {
            _items = new ObservableCollection<string>
            {
                "ITEM 1",
                "ITEM 2",
                "ITEM 3",
            };
        }

        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { _items = value; this.OnPropertyChanged(); }
        }
    }
}
