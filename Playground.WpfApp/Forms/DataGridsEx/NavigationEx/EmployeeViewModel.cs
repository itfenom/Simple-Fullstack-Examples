using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Playground.WpfApp.Forms.DataGridsEx.NavigationEx.DateRangeStuff;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;
#pragma warning disable 414

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class EmployeeViewModel : ValidationPropertyChangedBase
    {
        private readonly IDemoEmpRepository _repository;

        private DataTable _skillsDt;

        private DateRange? _dateRange;

        private bool _isEmployeeBeingDeleted;

        public override string Title => "Current Selection: Employees";

        private ObservableCollection<DemoEmployeeModel> _employees;
        public ObservableCollection<DemoEmployeeModel> Employees => _employees;

        private DemoEmployeeModel _selectedEmployee;

        public DemoEmployeeModel SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (value == _selectedEmployee) return;
                SetPropertyValue(ref _selectedEmployee, value);
            }
        }

        private Dictionary<string, Predicate<DemoEmployeeModel>> _filters;

        public CollectionView EmployeesView { get; set; }

        //For Department dropdown
        private ObservableCollection<DemoDepartmentModel> _departments;

        public ObservableCollection<DemoDepartmentModel> Departments => _departments;

        private string _formattedRangeDate;

        public string FormattedRangeDate
        {
            get => _formattedRangeDate;
            set => SetPropertyValue(ref _formattedRangeDate, value);
        }

        public EmployeeViewModel(IDemoEmpRepository repository)
        {
            _repository = repository;
            _filters = new Dictionary<string, Predicate<DemoEmployeeModel>>();

            //Initialize commands
            ClearFiltersCommand = new DelegateCommand(() => ClearFilters());
            GetJobTitleCommand = new DelegateCommand(() => GetJobTitle(), () => (SelectedEmployee != null));
            DeleteEmployeeCommand = new DelegateCommand(() => DeleteEmployee(), () => (SelectedEmployee != null));
            ReloadDataCommand = new DelegateCommand(() => LoadData());

            ShowDateRangeDialogCommand = new DelegateCommand(() => OnShowDateRangeDialog());
            ClearDateRangeCommand = new DelegateCommand(() => OnClearDateRange());
            ViewDetailsMenuCommand = new DelegateCommand<object>(param => OnViewDetailsContextMenu_Click(param));
            EditEmployeeInDialogCommand = new DelegateCommand(() => OnEditInDialog(), () => SelectedEmployee != null);
            AddNewEmployeeInDialogCommand = new DelegateCommand(() => OnAddNewInDialog());

            LoadData();

            PropertyChanged += NavigationEmployeeViewModel_PropertyChanged;
        }

        public override void ReloadData()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (_employees != null)
            {
                Employees.CollectionChanged -= Employees_CollectionChanged;
                EmployeesView.CurrentChanged -= EmployeesView_CurrentChanged;
                EmployeesView.CurrentChanging -= EmployeesView_CurrentChanging;
                EmployeesView = null;
                _employees = null;
            }

            _departments = null;
            _filters.Clear();

            //clear filter properties
            _empFirstNameFilter = string.Empty;
            _empJobTitleFilter = string.Empty;
            _empSalaryFilter = string.Empty;
            _selectedDepartmentFilter = null;

            //Get all skills 
            _skillsDt = _repository.GetAllSkills();

            //Load Departments
            _departments = new ObservableCollection<DemoDepartmentModel>(_repository.GetAllDepartments());

            //Load Employees
            var empList = _repository.GetAllEmployees();
            foreach (var item in empList)
            {
                item.SkillList = GetSkills(item.Id);
                item.PropertyChanged += NavigationEmployeeViewModel_PropertyChanged;
            }

            _employees = new ObservableCollection<DemoEmployeeModel>(empList);
            EmployeesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_employees);
            NotifyPropertyChanged("EmployeesView");

            EmployeesView.CurrentChanged += EmployeesView_CurrentChanged;
            EmployeesView.CurrentChanging += EmployeesView_CurrentChanging;
            EmployeesView.Filter = FilterEmployees;

            Employees.CollectionChanged += Employees_CollectionChanged;
        }

        private string GetSkills(int employeeId)
        {
            if (_skillsDt.Rows.Count == 0) return "";

            var skillsRows = _skillsDt.Select("EMPLOYEE_ID = " + employeeId);
            if (skillsRows.Length == 0) return "";

            var retVal = "";
            var skills = skillsRows.AsEnumerable().Select(x => x.Field<string>("SKILL")).ToList();
            if (skills.Count > 0)
            {
                retVal = string.Join(",", skills);
            }

            return retVal;
        }

        private void EmployeesView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            //Check if there were any unsaved MaterialProperties that must be saved before loading properties for the selected material
            //if (HasUnSavedChanges())
            //{
            //    if (Employees.Count == 0) return;

            //    if (_isEmployeeBeingDeleted) return;

            //    var result = MessageBox.Show("There are unsaved changes.\nDiscard changes and continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //    if (result == MessageBoxResult.No)
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }

        private void EmployeesView_CurrentChanged(object sender, EventArgs e)
        {
            SelectedEmployee = (DemoEmployeeModel)EmployeesView.CurrentItem;
        }

        private void Employees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            RefreshView();
        }

        private void NavigationEmployeeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                DeleteEmployeeCommand.RaiseCanExecuteChanged();
                GetJobTitleCommand.RaiseCanExecuteChanged();
                EditEmployeeInDialogCommand.RaiseCanExecuteChanged();
            }
        }

        #region Button-Commands/Methods

        public ICommand ClearFiltersCommand { get; }

        public DelegateCommand DeleteEmployeeCommand { get; }

        private void DeleteEmployee()
        {
            var result = MessageBox.Show(
                $"Are you sure, you want to delete employee {SelectedEmployee.FirstName} {SelectedEmployee.LastName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _repository.DeleteEmployee(SelectedEmployee.Id);
                Employees.Remove(SelectedEmployee);
            }
        }

        public DelegateCommand GetJobTitleCommand { get; }

        private void GetJobTitle()
        {
            if (SelectedEmployee == null) return;

            var jobTitleDialogView = new JobTitleDialogView { DataContext = new JobTitleDialogViewModel(_repository) };
            jobTitleDialogView.ShowDialog();

            var jobTitleVm = (JobTitleDialogViewModel)jobTitleDialogView.DataContext;

            if (jobTitleVm.DialogResultDependencyPropertyVal != null && jobTitleVm.DialogResultDependencyPropertyVal == true)
            {
                //SelectedEmployee.JobTitle = jobTitleVm.SelectedJobTitle.JobTitle;
                //SelectedEmployee.ValidateProperty(SelectedEmployee.JobTitle, $"JobTitle");
                //SelectedEmployee.IsDirty = true;
            }

            jobTitleVm.Dispose();
        }

        public ICommand ClearDateRangeCommand { get; }

        private void OnClearDateRange()
        {
            AddFilterAndRefresh("StartDate", x => x.StartDate >= DateTime.MinValue && x.StartDate <= DateTime.MaxValue);
            FormattedRangeDate = string.Empty;
            NotifyPropertyChanged("FormattedRangeDate");
        }

        public ICommand ShowDateRangeDialogCommand { get; }

        private void OnShowDateRangeDialog()
        {
            var dateRangeView = new DateRangeView();
            var dateRangeViewModel = new DateRangeViewModel(new SystemDateTimeProvider());

            dateRangeView.DataContext = dateRangeViewModel;
            dateRangeView.ShowDialog();

            if (dateRangeViewModel.UserCancelled == false)
            {
                _dateRange = dateRangeViewModel.Range;
                SetFormattedRunDateFilterValueAndApplyFilters(_dateRange);
            }
        }

        private void SetFormattedRunDateFilterValueAndApplyFilters(DateRange? dateRange)
        {
            if (dateRange == null)
            {
                FormattedRangeDate = null;
            }
            else
            {
                FormattedRangeDate = $"{dateRange.Value.StartDate.ToShortDateString()}-{dateRange.Value.EndDate.ToShortDateString()}";
            }

            NotifyPropertyChanged("FormattedRangeDate");

            if (EmployeesView == null || !dateRange.HasValue) return;
            AddFilterAndRefresh("StartDate", x => x.StartDate >= dateRange.Value.StartDate && x.StartDate <= dateRange.Value.EndDate);
        }

        public ICommand ViewDetailsMenuCommand { get; }

        private void OnViewDetailsContextMenu_Click(object param)
        {
            if (SelectedEmployee != null && param != null)
            {
                if (param.ToString() == "ViewDetails")
                {
                    var details = $"Id: {SelectedEmployee.Id}" + Environment.NewLine
                                + $"Name: {SelectedEmployee.FirstName} {SelectedEmployee.LastName}" + Environment.NewLine
                                + $"Salary: {SelectedEmployee.Salary.ToString("C", CultureInfo.CurrentCulture)}" + Environment.NewLine
                                + $"IsActive: {SelectedEmployee.IsActive}";
                    MessageBox.Show(details, "View Employee Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public ICommand ReloadDataCommand { get; }

        public DelegateCommand EditEmployeeInDialogCommand { get; }

        private void OnEditInDialog()
        {
            DemoEmployeeModel selectedEmp = SelectedEmployee;
            selectedEmp.BeginEdit();

            var viewModel = new AddEditEmployeeViewModel(selectedEmp);
            var view = new AddEditEmployeeView { DataContext = viewModel };
            view.Closing += viewModel.OnWindowClosing;
            view.ShowDialog();

            if (viewModel.DialogResultDependencyPropertyVal == null || viewModel.DialogResultDependencyPropertyVal == false)
            {
                selectedEmp.CancelEdit();
                selectedEmp.IsDirty = false;
            }
            else if (viewModel.DialogResultDependencyPropertyVal == true)
            {
                selectedEmp.EndEdit();

                selectedEmp.IsDirty = false;
                SelectedEmployee = selectedEmp;
                
                NotifyPropertyChanged("Employees");
                NotifyPropertyChanged("EmployeesView");
                NotifyPropertyChanged("SelectedEmployee");
            }

            viewModel.Dispose();
        }

        public ICommand AddNewEmployeeInDialogCommand { get; }

        private void OnAddNewInDialog()
        {
            var newEmployee = new DemoEmployeeModel
            {
                Id = -999,
                FirstName = "",
                LastName = "",
                Salary = -999,
                IsActive = true,
                Rating = 5,
                StartDate = DateTime.Today,
                IsNew = true,
                IsDirty = true
            };
            
            var viewModel = new AddEditEmployeeViewModel(newEmployee);
            var view = new AddEditEmployeeView { DataContext = viewModel };
            view.Closing += viewModel.OnWindowClosing;
            view.ShowDialog();

            if (viewModel.DialogResultDependencyPropertyVal == true)
            {
                newEmployee.IsDirty = false;
                newEmployee.IsNew = false;
                newEmployee.EndEdit();

                SelectedEmployee = newEmployee;
                NotifyPropertyChanged("SelectedEmployee");
                Employees.Add(SelectedEmployee);
            }

            viewModel.Dispose();
        }

        #endregion Button-Commands/Methods

        #region Filtering

        private DemoDepartmentModel _selectedDepartmentFilter;

        public DemoDepartmentModel SelectedDepartmentFilter
        {
            get => _selectedDepartmentFilter;
            set
            {
                if (_selectedDepartmentFilter == value) return;
                SetPropertyValue(ref _selectedDepartmentFilter, value);
                if (value == null) return;

                if (EmployeesView == null) return;
                AddFilterAndRefresh("DepartmentId", e => e.DepartmentId == value.DepartmentId);
            }
        }

        private string _empFirstNameFilter;

        public string EmpFirstNameFilter
        {
            get => _empFirstNameFilter;
            set
            {
                if (value == _empFirstNameFilter) return;
                SetPropertyValue(ref _empFirstNameFilter, value);

                if (EmployeesView == null) return;
                AddFilterAndRefresh("FirstName", e => e.FirstName.ToLower().Contains(value.ToLower()));
            }
        }

        private string _empLastNameFilter;

        public string EmpLastNameFilter
        {
            get => _empLastNameFilter;
            set
            {
                if (value == _empLastNameFilter) return;
                SetPropertyValue(ref _empLastNameFilter, value);

                if (EmployeesView == null) return;
                AddFilterAndRefresh("LastName", e => e.LastName.ToLower().Contains(value.ToLower()));
            }
        }

        private string _empJobTitleFilter;

        public string EmpJobTitleFilter
        {
            get => _empJobTitleFilter;
            set
            {
                if (value == _empJobTitleFilter) return;
                SetPropertyValue(ref _empJobTitleFilter, value);

                if (EmployeesView == null) return;
                AddFilterAndRefresh("JobTitle", e => e.JobTitle.ToLower().Contains(value.ToLower()));
            }
        }

        private string _empSalaryFilter;

        public string EmpSalaryFilter
        {
            get => _empSalaryFilter;
            set
            {
                if (value == _empSalaryFilter) return;
                SetPropertyValue(ref _empSalaryFilter, value);

                if (EmployeesView == null) return;
                AddFilterAndRefresh("Salary", e => e.Salary.ToString().ToLower().Contains(value.ToLower()));
            }
        }

        private string _empSkillListFilter;

        public string EmpSkillListFilter
        {
            get => _empSkillListFilter;
            set
            {
                if (value == _empSkillListFilter) return;
                SetPropertyValue(ref _empSkillListFilter, value);

                if (EmployeesView == null) return;
                AddFilterAndRefresh("SkillList", e => e.SkillList.ToString().ToLower().Contains(value.ToLower()));
            }
        }

        private bool FilterEmployees(object obj)
        {
            if (_filters == null) return false;

            var c = (DemoEmployeeModel)obj;
            return _filters.Values.Aggregate(true, (prevValue, predicate) => prevValue && predicate(c));
        }

        public void RemoveFilter(string filterName)
        {
            if (_filters.Remove(filterName))
            {
                RefreshView();
            }
        }

        private void AddFilterAndRefresh(string name, Predicate<DemoEmployeeModel> predicate)
        {
            try
            {
                if (_filters.Keys.Contains(name))
                {
                    _filters.Remove(name);

                    _filters.Add(name, predicate);
                }
                else if (!_filters.Keys.Contains(name))
                {
                    _filters.Add(name, predicate);
                }
            }
            catch (Exception oEx)
            {
                if (EmployeesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }
                }
                else
                {
                    MessageBox.Show(oEx.Message, "Employees", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            finally
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            try
            {
                EmployeesView.Refresh();
                NotifyPropertyChanged("EmployeesView");
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFilters()
        {
            try
            {
                _filters.Clear();
                FormattedRangeDate = null;
                EmployeesView.Refresh();

                EmpFirstNameFilter = string.Empty;
                EmpLastNameFilter = string.Empty;
                EmpJobTitleFilter = string.Empty;
                EmpSalaryFilter = string.Empty;
                EmpSkillListFilter = string.Empty;
                SelectedDepartmentFilter = null;

                // Bring the current record back into view in case it moved
                if (SelectedEmployee != null)
                {
                    DemoEmployeeModel current = SelectedEmployee;
                    EmployeesView.MoveCurrentToFirst();
                    EmployeesView.MoveCurrentTo(current);
                }
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion Filtering

        #region Closing

        public override bool HasUnSavedChanges()
        {
            var isDirtyObjects = (from e in Employees where e.IsDirty select e).ToList();

            if (isDirtyObjects.Any()) return true;

            return false;
        }

        protected override void DisposeManagedResources()
        {
            PropertyChanged -= NavigationEmployeeViewModel_PropertyChanged;

            if (_employees != null)
            {
                Employees.CollectionChanged -= Employees_CollectionChanged;
                EmployeesView.CurrentChanged -= EmployeesView_CurrentChanged;
                EmployeesView.CurrentChanging -= EmployeesView_CurrentChanging;
                EmployeesView = null;
                _employees = null;
            }

            _departments = null;
            _filters = null;
            _selectedEmployee = null;

            _selectedDepartmentFilter = null;
            _empFirstNameFilter = null;
            _empLastNameFilter = null;
            _empJobTitleFilter = null;
            _empSalaryFilter = null;
            _formattedRangeDate = null;

        }

        #endregion Closing
    }
}
