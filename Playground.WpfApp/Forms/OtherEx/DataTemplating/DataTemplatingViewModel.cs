using System.Collections.Generic;
using System.Collections.ObjectModel;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.DataTemplating
{
    public class DataTemplatingViewModel : PropertyChangedBase
    {
        public override string Title => "Data Templating example";

        private ObservableCollection<DataTemplatingModel> _taskList;

        public ObservableCollection<DataTemplatingModel> TaskList
        {
            get => _taskList;
            set => SetPropertyValue(ref _taskList, value);
        }

        public DataTemplatingViewModel()
        {
            var list = GetAllTasks();
            _taskList = new ObservableCollection<DataTemplatingModel>(list);
            NotifyPropertyChanged("TaskList");
        }

        private List<DataTemplatingModel> GetAllTasks()
        {
            var retVal = new List<DataTemplatingModel>() {
                new DataTemplatingModel{ TaskName = "Shopping", Description = "Pickup groceries.", Priority = 2, TaskType = TaskType.Home },
                new DataTemplatingModel{ TaskName = "Laundry", Description = "Do my laundry.", Priority = 2, TaskType = TaskType.Home },
                new DataTemplatingModel{ TaskName = "Email", Description = "Email clients.", Priority = 1, TaskType = TaskType.Work },
                new DataTemplatingModel{ TaskName = "Clean", Description = "Clean my office.", Priority = 3, TaskType = TaskType.Work },
                new DataTemplatingModel{ TaskName = "Dinner", Description = "Dinner with family.", Priority = 2, TaskType = TaskType.Home },
                new DataTemplatingModel{ TaskName = "Workout", Description = "Go to the gym this weekend.", Priority = 1, TaskType = TaskType.Home }
            };

            return retVal;
        }
    }
}
