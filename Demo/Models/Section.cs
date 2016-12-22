using System.Collections.ObjectModel;

namespace Demo.Models
{
    public class Section : ModelBase
    {
        private string _title;
        private ObservableCollection<Item> _items = new ObservableCollection<Item>();

        public string Title
        {
            get { return _title; }
            set { _title = value; this.OnPropertyChanged(); }
        }

        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set { _items = value; this.OnPropertyChanged(); }
        }
    }
}
