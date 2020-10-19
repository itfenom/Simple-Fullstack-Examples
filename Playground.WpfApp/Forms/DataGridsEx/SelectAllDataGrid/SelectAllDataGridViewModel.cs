using System;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Forms.DataGridsEx.CheckBoxDataGrid;
using Playground.WpfApp.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Playground.WpfApp.Forms.DataGridsEx.SelectAllDataGrid
{
    public class SelectAllDataGridViewModel : PropertyChangedBase
    {
        public override string Title => "DataGrid with Select All CheckBox";

        private readonly IDialogCoordinator _dialogCoordinator;

        private List<Predicate<CheckBoxDataGridModel>> _employeeFilterCriteria;

        private ObservableCollection<CheckBoxDataGridModel> _employeeList;

        public ObservableCollection<CheckBoxDataGridModel> EmployeeList
        {
            get => _employeeList;
            set => SetPropertyValue(ref _employeeList, value);
        }

        public CollectionView EmployeesView { get; set; }

        public SelectAllDataGridViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            _employeeFilterCriteria = new List<Predicate<CheckBoxDataGridModel>>();

            ShowSelectedCommand = new DelegateCommand(() => OnShowSelected());

            var emp = new List<CheckBoxDataGridModel>
            {
                new CheckBoxDataGridModel ("King", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Blake", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Clark", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Jones", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Scott", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Martin", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Turner", false, OnEmployeeSelectionChanged),
                new CheckBoxDataGridModel ("Ward", false, OnEmployeeSelectionChanged)
            };

            _employeeList = new ObservableCollection<CheckBoxDataGridModel>(emp);
            EmployeesView = (CollectionView)new CollectionViewSource { Source = _employeeList }.View;
            NotifyPropertyChanged("EmployeesView");
        }

        private void OnEmployeeSelectionChanged(bool val)
        {
            if (_selectAll && !val)
            { // all are selected, and one gets turned off
                _selectAll = false;
                NotifyPropertyChanged("SelectAll");
            }
            else if (!_selectAll && EmployeeList.All(e => e.IsEmployeeChecked))
            { // last one off one gets turned on, resulting in all being selected
                _selectAll = true;
                NotifyPropertyChanged("SelectAll");
            }
        }

        private void OnShowSelected()
        {
            if (_employeeList != null && _employeeList.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in _employeeList)
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

        public ICommand ShowSelectedCommand { get; }

        private bool _selectAll;

        public bool SelectAll
        {
            get => _selectAll;
            set
            {
                SetPropertyValue(ref _selectAll, value);


                foreach (var item in _employeeList)
                {
                    item.IsEmployeeChecked = value;
                }
                NotifyPropertyChanged("EmployeeList");

                if(value)
                {
                    _showSelected = true;
                    NotifyPropertyChanged("ShowSelected");
                }
            }
        }

        private bool? _showSelected;

        public bool? ShowSelected
        {
            get => _showSelected;
            set
            {
                SetPropertyValue(ref _showSelected, value);
                ApplyFilters();
            }
        }


        private string _employeeFilterVal;

        public string EmployeeFilterVal
        {
            get => _employeeFilterVal;
            set
            {
                SetPropertyValue(ref _employeeFilterVal, value);
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (EmployeesView == null) return;

                _employeeFilterCriteria.Clear();

                if (string.IsNullOrEmpty(EmployeeFilterVal))
                {
                    if (_showSelected == null)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null && (x.IsEmployeeChecked || !x.IsEmployeeChecked));
                    }
                    else if (_showSelected == false)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null && !x.IsEmployeeChecked);
                    }
                    else if (_showSelected == true)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null && x.IsEmployeeChecked);
                    }
                }
                else
                {
                    if (_showSelected == null)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null &&
                                                         x.EmployeeName.ToLower().Contains(EmployeeFilterVal.ToLower()) &&
                                                         (x.IsEmployeeChecked || !x.IsEmployeeChecked));
                    }
                    else if (_showSelected == false)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null &&
                                                         x.EmployeeName.ToLower().Contains(EmployeeFilterVal.ToLower()) &&
                                                         !x.IsEmployeeChecked);
                    }
                    else if (_showSelected == true)
                    {
                        _employeeFilterCriteria.Add(x => x.EmployeeName != null &&
                                                         x.EmployeeName.ToLower().Contains(EmployeeFilterVal.ToLower()) &&
                                                         x.IsEmployeeChecked);
                    }
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

        private void RefreshEmployeeView()
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
        }

        // ReSharper disable once UnusedMember.Local
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
    }
}
