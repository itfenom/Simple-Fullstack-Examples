using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class NavigationMainViewModel : ValidationPropertyChangedBase
    {
        private readonly Timer _timer;

        private readonly IDemoEmpRepository _repository;

        private ValidationPropertyChangedBase _selectedChild;

        public ValidationPropertyChangedBase SelectedChild
        {
            get => _selectedChild;
            set => SetPropertyValue(ref _selectedChild, value);
        }

        public List<ValidationPropertyChangedBase> Children { get; protected set; }

        public override string Title => "Navigation: Main";

        public string Version => GetType().Assembly.GetName().Version.ToString();

        #region Properties:

        private string _time;

        public string Time
        {
            get => _time;
            set => SetPropertyValue(ref _time, value);
        }

        private bool _isEnabledBlank;

        public bool IsEnabledBlank
        {
            get => _isEnabledBlank;
            set => SetPropertyValue(ref _isEnabledBlank, value);
        }

        private bool _isEnabledEmployees;

        public bool IsEnabledEmployees
        {
            get => _isEnabledEmployees;
            set => SetPropertyValue(ref _isEnabledEmployees, value);
        }

        private bool _isEnabledDepartments;

        public bool IsEnabledDepartments
        {
            get => _isEnabledDepartments;
            set => SetPropertyValue(ref _isEnabledDepartments, value);
        }

        private bool _isEnabledReporting;

        public bool IsEnabledReporting
        {
            get => _isEnabledReporting;
            set => SetPropertyValue(ref _isEnabledReporting, value);
        }

        #endregion Properties

        public NavigationMainViewModel()
        {
            _repository = new DemoEmpRepository();
            Children = new List<ValidationPropertyChangedBase>();

            GoToEmployeesCommand = new DelegateCommand(() => GoToEmployees());
            GoToDepartmentsCommand = new DelegateCommand(() => GoToDepartments());
            GoToViewReportCommand = new DelegateCommand(() => GotoViewReport());
            GoToBlankCommand = new DelegateCommand(() => GoToBlank());

            //By default, select employees
            LoadEmployees();

            _timer = new Timer(s => Time = DateTime.Now.ToLongTimeString(), this, 500, 500);
        }

        #region Goto Commands/Methods

        private bool PromptForUnsavedChanges()
        {
            var result = MessageBox.Show("There are unsaved changes waiting to be saved, discard changes?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SelectedChild.ReloadData();
                return true;
            }

            return false;
        }

        public ICommand GoToEmployeesCommand { get; }

        private void GoToEmployees()
        {
            if (HasChanges())
            {
                if (!PromptForUnsavedChanges()) return;
            }
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var empVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(EmployeeViewModel));
            if (empVm == null)
            {
                empVm = new EmployeeViewModel(_repository);
                Children.Add(empVm);
            }

            SelectedChild = empVm;
            IsEnabledEmployees = false;
            IsEnabledDepartments = true;
            IsEnabledReporting = true;
            IsEnabledBlank = true;
        }

        public ICommand GoToDepartmentsCommand { get; }

        private void GoToDepartments()
        {
            if (HasChanges())
            {
                if (!PromptForUnsavedChanges()) return;
            }

            LoadDepartments();
        }

        private void LoadDepartments()
        {
            var deptVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(DepartmentViewModel));
            if (deptVm == null)
            {
                deptVm = new DepartmentViewModel(_repository);
                Children.Add(deptVm);
            }

            SelectedChild = deptVm;
            IsEnabledEmployees = true;
            IsEnabledDepartments = false;
            IsEnabledReporting = true;
            IsEnabledBlank = true;
        }

        public ICommand GoToViewReportCommand { get; }

        private void GotoViewReport()
        {
            if (HasChanges())
            {
                if (!PromptForUnsavedChanges()) return;
            }
            LoadReport();
        }

        private void LoadReport()
        {
            var reportVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(ReportingViewModel));
            if (reportVm == null)
            {
                reportVm = new ReportingViewModel(_repository);
                Children.Add(reportVm);
            }

            SelectedChild = reportVm;
            IsEnabledEmployees = true;
            IsEnabledDepartments = true;
            IsEnabledReporting = false;
            IsEnabledBlank = true;
        }

        public ICommand GoToBlankCommand { get; }

        private void GoToBlank()
        {
            if (HasChanges())
            {
                if (!PromptForUnsavedChanges()) return;
            }
            LoadBlank();
        }

        private void LoadBlank()
        {
            var blankVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(BlankViewModel));
            if (blankVm == null)
            {
                blankVm = new BlankViewModel();
                Children.Add(blankVm);
            }

            SelectedChild = blankVm;
            IsEnabledEmployees = true;
            IsEnabledDepartments = true;
            IsEnabledReporting = true;
            IsEnabledBlank = false;
        }

        #endregion Goto Commands/Methods

        #region HasChanges/Dispose

        private bool HasChanges()
        {
            if (SelectedChild == null) return false;

            return SelectedChild.HasUnSavedChanges();
        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var viewModel = Children[i];
                viewModel.Dispose();
            }

            _timer.Dispose();
        }

        #endregion HasChanges/Dispose
    }
}
