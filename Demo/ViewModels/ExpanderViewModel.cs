using Demo.Infrastructure;
using Demo.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Demo.ViewModels
{
    public class ExpanderViewModel : ModelBase
    {
        private ObservableCollection<Section> _sections;
        private Section _currentSelection;
        private ICommand _selectionCommand;

        public ExpanderViewModel()
        {
            this.Sections = new ObservableCollection<Section>
            {
                new Section { Title = "SECTION 1", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
                new Section { Title = "SECTION 2", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
                new Section { Title = "SECTION 3", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
                new Section { Title = "SECTION 4", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
                new Section { Title = "SECTION 5", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
                new Section { Title = "SECTION 6", Items = new ObservableCollection<Item> { new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }, new Item { Name = "Item 4" }, new Item { Name = "Item 5" }, new Item { Name = "Item 6" } } },
            };
            this.SelectionCommand = new RelayCommand(this.ExecuteSelectionCommand);
        }

        public ObservableCollection<Section> Sections
        {
            get { return _sections; }
            set { _sections = value; this.OnPropertyChanged(); }
        }

        public Section CurrentSelection
        {
            get { return _currentSelection; }
            set { _currentSelection = value; this.OnPropertyChanged(); }
        }

        public ICommand SelectionCommand
        {
            get { return _selectionCommand; }
            set { _selectionCommand = value; this.OnPropertyChanged(); }
        }

        private void ExecuteSelectionCommand(object parameter)
        {
            // command handling...
        }
    }
}
