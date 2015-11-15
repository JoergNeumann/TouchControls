using Demo.Models;
using Neumann.TouchControls;
using System;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Demo.ViewModels
{
    public class RadialPresenterViewModel : ModelBase
    {
        private TopicCollection _topicHierarchy;
        private ICommand _selectionCommand;

        public RadialPresenterViewModel()
        {
            this.SelectionCommand = new Demo.Infrastructure.RelayCommand(this.ExecuteRadialPickerCommand);
            this.TopicHierarchy = new TopicCollection()
                {
                    new Topic { Name = "Container", ImagePathResourceName="Container",
                        Children =  new TopicCollection()
                        {
                            new Topic { Name = "Column View", ImagePathResourceName="Container"}
                        },
                    },
                    new Topic { Name = "Input", ImagePathResourceName="Input",
                        Children =  new TopicCollection()
                        {
                            new Topic { Name = "Range Slider", ImagePathResourceName="Input"},
                            new Topic { Name = "Radial Picker", ImagePathResourceName="Input"},
                            new Topic { Name = "Number Pad", ImagePathResourceName="Input"},
                            new Topic { Name = "Spinner", ImagePathResourceName="Input"}
                        },
                    },
                    new Topic { Name = "Commands", ImagePathResourceName="Commands",
                        Children =  new TopicCollection()
                        {
                            new Topic { Name = "Radial Menu", ImagePathResourceName="Commands"},
                            new Topic { Name = "Radial Presenter", ImagePathResourceName="Commands"},
                            new Topic { Name = "Fan Selector", ImagePathResourceName="Commands"}
                        },
                    },
                };
        }

        public ICommand SelectionCommand
        {
            get { return _selectionCommand; }
            set { _selectionCommand = value; this.OnPropertyChanged(); }
        }

        public TopicCollection TopicHierarchy
        {
            get { return _topicHierarchy; }
            set { _topicHierarchy = value; this.OnPropertyChanged(); }
        }

        private async void ExecuteRadialPickerCommand(object parameter)
        {
            var topic = parameter as Topic;
            if (topic != null && topic.Children.Count == 0)
            {
                await new MessageDialog(topic.Name).ShowAsync();
            }
        }
    }
}
