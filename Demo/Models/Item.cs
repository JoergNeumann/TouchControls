namespace Demo.Models
{
    public class Item : ModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; this.OnPropertyChanged(); }
        }
    }
}
