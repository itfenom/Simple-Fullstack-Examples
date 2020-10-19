using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class ReportingViewModel : ValidationPropertyChangedBase
    {
        public override string Title => "Current Selection: Report";

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IDemoEmpRepository _repository;

        private ObservableCollection<DemoEmployeeModel> _employees;

        public ObservableCollection<DemoEmployeeModel> Employees
        {
            get => _employees;
            set => SetPropertyValue(ref _employees, value);
        }

        public ReportingViewModel(IDemoEmpRepository repository)
        {
            _repository = repository;

            var empList = _repository.GetAllEmployees();
            _employees = new ObservableCollection<DemoEmployeeModel>(empList);
        }

        // ReSharper disable once MergeConditionalExpression
        public int EmployeeCount => _employees == null ? 0 : _employees.Count;

        public string EmployeeReportTitle => "Report:  Employee Report";
    }
}
