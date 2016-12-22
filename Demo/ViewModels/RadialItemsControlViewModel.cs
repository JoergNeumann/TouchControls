using Demo.Infrastructure;
using Demo.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Demo.ViewModels
{
    public class RadialItemsControlViewModel : ModelBase
    {
        private ObservableCollection<string> _items;
        private bool _reset;
        private ICommand _resetCommand;
        private ICommand _addCommand;
        private ICommand _deleteCommand;

        public RadialItemsControlViewModel()
        {
            _items = new ObservableCollection<string>
            {
                "ITEM 1",
                "ITEM 2",
                "ITEM 3",
            };
            this.ResetCommand = new RelayCommand(this.OnExecuteResetCommand);
            this.AddCommand = new RelayCommand(this.OnExecuteAddCommand);
            this.DeleteCommand = new RelayCommand(this.OnExecuteDeleteCommand);
        }

        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { _items = value; this.OnPropertyChanged(); }
        }

        public bool Reset
        {
            get { return _reset; }
            set { _reset = value; this.OnPropertyChanged(); }
        }

        public ICommand ResetCommand
        {
            get { return _resetCommand; }
            set { _resetCommand = value; this.OnPropertyChanged(); }
        }

        public ICommand AddCommand
        {
            get { return _addCommand; }
            set { _addCommand = value; this.OnPropertyChanged(); }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
            set { _deleteCommand = value; this.OnPropertyChanged(); }
        }

        private void OnExecuteResetCommand(object parameter)
        {
            this.Reset = true;
        }

        private void OnExecuteAddCommand(object parameter)
        {
            _items.Add("ITEM " + (_items.Count + 1).ToString());
        }

        private void OnExecuteDeleteCommand(object parameter)
        {
            if (_items.Count > 0)
            {
                _items.RemoveAt(_items.Count - 1);
            }
        }
    }
}
