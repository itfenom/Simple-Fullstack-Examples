using System.Collections.ObjectModel;
using System.Linq;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.OtherEx.ItemsControlEx.Ex3
{
    public class ItemsCtrlViewModel : PropertyChangedBase
    {
        public override string Title => "Items Control Ex3";
        private IDemoEmpRepository _repository;

        private ObservableCollection<DemoEmployeeModel> _employees;

        public ObservableCollection<DemoEmployeeModel> Employees
        {
            get => _employees;
            set => SetPropertyValue(ref _employees, value);
        }

        public ItemsCtrlViewModel()
        {
            _repository = new DemoEmpRepository();
            _employees = new ObservableCollection<DemoEmployeeModel>(_repository.GetAllEmployees().Take(10));
        }

        protected override void DisposeManagedResources()
        {
            _employees = null;
        }
    }
}
