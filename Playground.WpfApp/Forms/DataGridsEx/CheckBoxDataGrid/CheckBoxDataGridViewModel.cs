using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.CheckBoxDataGrid
{
    public enum ChooseRoleEnum
    {
        Admin,
        Guest
    }

    public class CheckBoxDataGridViewModel : ValidationPropertyChangedBase
    {
        private string _title;

        public override string Title => _title;

        private readonly IDialogCoordinator _dialogCoordinator;

        //Filtering Criteria
        private List<Predicate<CheckBoxDataGridModel>> _employeeFilterCriteria;

        //EmployeeModel variable
        private List<CheckBoxDataGridModel> _employeeModel = new List<CheckBoxDataGridModel>();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly string _selectedRole;

        public CheckBoxDataGridViewModel(IDialogCoordinator dialogCoordinator, string selectedRole)
        {
            _dialogCoordinator = dialogCoordinator;
            _selectedRole = selectedRole;
            LoadData();
            LoadCommands();

            if (_selectedRole.ToUpper() == "ADMIN")
            {
                foreach (var item in _employeeModel)
                {
                    item.CanEditEmployee = true;
                    item.IsEmployeeChecked = true;
                }

                _title = "DataGrid with CheckBoxes Binding | Role: ADMIN";
            }
            else if (_selectedRole.ToUpper() == "GUEST")
            {
                foreach (var item in _employeeModel)
                {
                    if (item.EmployeeName.ToUpper() == "KING" || item.EmployeeName.ToUpper() == "FORD")
                        item.CanEditEmployee = false;
                    item.IsEmployeeChecked = false;
                }

                _title = "DataGrid with CheckBoxes Binding | Role: GUEST";
            }

            //Instantiate filter object
            _employeeFilterCriteria = new List<Predicate<CheckBoxDataGridModel>>();

            //Instantiate Commands
            _employeeCheckBoxCommand = new DelegateCommand(() => EmployeeCheckBoxClick());

            //Set DataGrid binding.
            _employeesList = new ObservableCollection<CheckBoxDataGridModel>(_employeeModel);
            EmployeesView = (CollectionView)new CollectionViewSource { Source = _employeesList }.View;
            NotifyPropertyChanged("EmployeesView");
            EmployeesList.CollectionChanged += EmployeesList_CollectionChanged;
            SetEmployeeCountLabel();

            PropertyChanged += CheckBoxDataGridViewModel_PropertyChanged;
        }

        private void CheckBoxDataGridViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _employeeCheckBoxCommand.RaiseCanExecuteChanged();
            }
        }

        private void LoadCommands()
        {
            //Employee Commands
            var empCommands = new List<ActionCommand>();
            empCommands.Add(new ActionCommand { Title = "Select All (*)", Command = EmployeeActionCommand, ParameterText = "SelectAllEmployees" });
            empCommands.Add(new ActionCommand { Title = "Select Filtered", Command = EmployeeActionCommand, ParameterText = "SelectFilteredEmployees" });
            empCommands.Add(new ActionCommand { Title = "Clear Selection", Command = EmployeeActionCommand, ParameterText = "ClearEmployeeSelection" });
            empCommands.Add(new ActionCommand { Title = "Clear Filters", Command = EmployeeActionCommand, ParameterText = "ClearEmployeeFilters" });
            EmployeeActionCommands = empCommands;
        }

        private void LoadData()
        {
            _employeeModel = new List<CheckBoxDataGridModel>();
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "King" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Blake" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Clark" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Jones" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Scott" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Martin" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "Turner" });
            _employeeModel.Add(new CheckBoxDataGridModel { EmployeeName = "ward" });
        }

        //ObservableCollection/CollectionView
        private ObservableCollection<CheckBoxDataGridModel> _employeesList;

        public ObservableCollection<CheckBoxDataGridModel> EmployeesList
        {
            get => _employeesList;
            set => SetPropertyValue(ref _employeesList, value);
        }

        public CollectionView EmployeesView { get; set; }

        public List<ActionCommand> EmployeeActionCommands { get; private set; }

        private CheckBoxDataGridModel _selectedEmployee;

        public CheckBoxDataGridModel SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetPropertyValue(ref _selectedEmployee, value);
        }

        private string _employeeFilterVal;

        public string EmployeeFilterVal
        {
            get => _employeeFilterVal;
            set
            {
                if (value == _employeeFilterVal) return;
                SetPropertyValue(ref _employeeFilterVal, value);
                ApplyEmployeeFilters();
            }
        }

        private string _employeeCountLabel;

        public string EmployeeCountLabel
        {
            get => _employeeCountLabel;
            set => SetPropertyValue(ref _employeeCountLabel, value);
        }

        private void EmployeesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshEmployeeView();
        }

        private DelegateCommand _employeeCheckBoxCommand;

        public DelegateCommand EmployeeCheckBoxCommand => _employeeCheckBoxCommand;

        private void EmployeeCheckBoxClick()
        {
            if (SelectedEmployee != null)
            {
                SelectedEmployee.IsDirty = true;
            }

            NotifyPropertyChanged("ShowCheckedEmployees");
            ApplyEmployeeFilters();
            SetEmployeeCountLabel();
        }

        private bool _showCheckedEmployees;

        public bool ShowCheckedEmployees
        {
            get => _showCheckedEmployees;
            set
            {
                SetPropertyValue(ref _showCheckedEmployees, value);
                ApplyEmployeeFilters();
            }
        }

        private void SetEmployeeCountLabel()
        {
            // ReSharper disable once UseMethodAny.2
            if (_employeesList == null || _employeesList.Count() == 0)
            {
                _employeeCountLabel = "0 Selected";
            }
            else
            {
                var checkedRows = (from e in _employeesList where e.IsEmployeeChecked select e).ToList();
                _employeeCountLabel = checkedRows.Count + " Selected";
            }

            NotifyPropertyChanged("EmployeeCountLabel");
        }

        public ICommand EmployeeActionCommand
        {
            get { return new DelegateCommand<object>(obj => EmployeeActions_Click(obj), p => true); }
        }

        private void EmployeeActions_Click(object obj)
        {
            if (obj.ToString() == "SelectAllEmployees")
            {
                SelectAllEmployees();
            }
            else if (obj.ToString() == "SelectFilteredEmployees")
            {
                SelectFilteredEmployees();
            }
            else if (obj.ToString() == "ClearEmployeeSelection")
            {
                ClearAllEmployeeSelection();
            }
            else if (obj.ToString() == "ClearEmployeeFilters")
            {
                ClearEmployeeFilters();
            }
        }

        private void SelectAllEmployees()
        {
            if (_employeesList != null)
            {
                foreach (var item in _employeesList)
                {
                    item.IsEmployeeChecked = true;
                }

                _employeeCountLabel = "*";
                NotifyPropertyChanged("EmployeeCountLabel");
                NotifyPropertyChanged("EmployeesList");
                NotifyPropertyChanged("EmployeesView");
                RefreshEmployeeView();
            }
        }

        private void SelectFilteredEmployees()
        {
            if (EmployeesView != null && !string.IsNullOrEmpty(EmployeeFilterVal))
            {
                //First clear the selection
                ClearAllEmployeeSelection();

                //Now Select filtered products
                foreach (var item in EmployeesView)
                {
                    var empModel = (CheckBoxDataGridModel)item;
                    if (empModel != null)
                    {
                        var empToSelect = (from e in _employeesList where e.EmployeeName == empModel.EmployeeName select e).FirstOrDefault();
                        if (empToSelect != null) empToSelect.IsEmployeeChecked = true;
                    }
                }

                SetEmployeeCountLabel();
                RefreshEmployeeView();
            }
        }

        private void ClearAllEmployeeSelection()
        {
            if (_employeesList != null && _employeesList.Count > 0)
            {
                foreach (var item in _employeesList)
                {
                    item.IsEmployeeChecked = false;
                }

                _employeeCountLabel = "0 Selected";
                NotifyPropertyChanged("EmployeeCountLabel");
                NotifyPropertyChanged("EmployeesList");
                NotifyPropertyChanged("EmployeesView");
                RefreshEmployeeView();
            }
        }

        public ICommand DetailCommand
        {
            get { return new DelegateCommand(() => DisplayDetails()); }
        }

        private void DisplayDetails()
        {
            if (_employeesList != null && _employeesList.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in _employeesList)
                {
                    if (item.IsEmployeeChecked)
                    {
                        sb.AppendLine(item.EmployeeName);
                    }
                }

                if (sb.ToString().Length > 0)
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Details", sb.ToString());
                    return;
                }
            }
            _dialogCoordinator.ShowMessageAsync(this, "Details", "Nothing selected!");
        }

        #region Employee Filters

        private void ApplyEmployeeFilters()
        {
            try
            {
                if (EmployeesView == null) return;

                if (string.IsNullOrEmpty(EmployeeFilterVal) && _showCheckedEmployees == false)
                {
                    ClearEmployeeFilters();
                    return;
                }

                _employeeFilterCriteria.Clear();

                if (!string.IsNullOrEmpty(EmployeeFilterVal) && _showCheckedEmployees == false)
                {
                    _employeeFilterCriteria.Add(x => x.EmployeeName != null && x.EmployeeName.ToLower().Contains(EmployeeFilterVal.ToLower()));
                }
                else if (string.IsNullOrEmpty(EmployeeFilterVal) && _showCheckedEmployees)
                {
                    _employeeFilterCriteria.Add(x => x.IsEmployeeChecked);
                }
                else if (!string.IsNullOrEmpty(EmployeeFilterVal) && _showCheckedEmployees)
                {
                    _employeeFilterCriteria.Add(x => x.EmployeeName != null && x.EmployeeName.ToLower().Contains(EmployeeFilterVal.ToLower())
                                                                            && x.IsEmployeeChecked);
                }

                EmployeesView.Filter = Employee_Filter;
                NotifyPropertyChanged("EmployeesView");
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

                    EmployeesView.Filter = Employee_Filter;
                    NotifyPropertyChanged("EmployeesView");
                }
                else
                {
                    MessageBox.Show(oEx.Message, "CheckBoxDataGrid", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool Employee_Filter(object item)
        {
            if (_employeeFilterCriteria.Count == 0)
            {
                return true;
            }

            var p = item as CheckBoxDataGridModel;
            return _employeeFilterCriteria.TrueForAll(x => x(p));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private object RefreshEmployeeView()
        {
            try
            {
                if (EmployeesView != null)
                {
                    EmployeesView.Refresh();
                    NotifyPropertyChanged("EmployeesView");
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

                    EmployeesView.Filter = Employee_Filter;
                    NotifyPropertyChanged("EmployeesView");
                }
                else
                {
                    MessageBox.Show(oEx.Message, "CheckBoxDataGrid", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            return null;
        }

        private void ClearEmployeeFilters()
        {
            try
            {
                _employeeFilterCriteria.Clear();
                RefreshEmployeeView();

                EmployeeFilterVal = string.Empty;
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "CheckBoxDataGrid", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion Employee Filters

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _employeeFilterCriteria = null;
                    _employeeModel = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}