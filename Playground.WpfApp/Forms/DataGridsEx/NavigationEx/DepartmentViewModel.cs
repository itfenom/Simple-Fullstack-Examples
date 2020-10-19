using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class DepartmentViewModel : ValidationPropertyChangedBase
    {
        public override string Title => "Current Selection: Departments";

        private readonly IDemoEmpRepository _repository;

        private ObservableCollection<DemoDepartmentModel> _departments;
        public ObservableCollection<DemoDepartmentModel> Departments => _departments;

        public CollectionView DepartmentsView { get; set; }

        private DemoDepartmentModel _selectedDepartment;

        public DemoDepartmentModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => SetPropertyValue(ref _selectedDepartment, value);
        }

        private List<int> _deletedDepartments;

        private List<Predicate<DemoDepartmentModel>> _departmentFilterCriteria;

        public DepartmentViewModel(IDemoEmpRepository repository)
        {
            _repository = repository;
            _deletedDepartments = new List<int>();
            _departmentFilterCriteria = new List<Predicate<DemoDepartmentModel>>();

            //Initialize Commands
            AddNewDepartmentCommand = new DelegateCommand(() => AddNewDepartment());
            ClearFilterCommand = new DelegateCommand(() => ClearFilters());
            SaveDepartmentCommand = new DelegateCommand(() => Save(), () => CanSave);
            DeleteDepartmentCommand = new DelegateCommand(() => DeleteDepartment(), () => (SelectedDepartment != null));
            ReloadDataCommand = new DelegateCommand(() => LoadData());

            LoadData();

            PropertyChanged += NavigationDepartmentViewModel_PropertyChanged;
        }

        public override void ReloadData()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (_departments != null)
            {
                Departments.CollectionChanged -= Departments_CollectionChanged;
                _departments = null;
                DepartmentsView = null;
            }

            _deletedDepartments.Clear();
            _departmentFilterCriteria.Clear();
            SelectedDepartment = null;
            _deptNameFilter = null;

            //Load Departments
            var allDepts = _repository.GetAllDepartments();
            foreach (var item in allDepts)
            {
                item.PropertyChanged += NavigationDepartmentViewModel_PropertyChanged;
            }

            _departments = new ObservableCollection<DemoDepartmentModel>(allDepts);
            DepartmentsView = (ListCollectionView)CollectionViewSource.GetDefaultView(_departments);
            //DepartmentsView = (CollectionView)new CollectionViewSource { Source = _departments }.View;
            NotifyPropertyChanged("DepartmentsView");

            Departments.CollectionChanged += Departments_CollectionChanged;
        }

        private void Departments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void NavigationDepartmentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                SaveDepartmentCommand.RaiseCanExecuteChanged();
                DeleteDepartmentCommand.RaiseCanExecuteChanged();
            }
        }

        #region ICommand/Button methods

        public ICommand ClearFilterCommand { get; }

        public ICommand AddNewDepartmentCommand { get; }

        private void AddNewDepartment()
        {
            var deptModel = new DemoDepartmentModel
            {
                DepartmentId = -999,
                DepartmentName = string.Empty,
                IsDirty = true,
                IsNew = true
            };

            deptModel.PropertyChanged += NavigationDepartmentViewModel_PropertyChanged;
            deptModel.ValidateProperty(-999, $"DepartmentId");
            deptModel.ValidateProperty("", $"DepartmentName");
            _departments.Add(deptModel);
            NotifyPropertyChanged("DepartmentsView");
            SelectedDepartment = deptModel;
        }

        public DelegateCommand DeleteDepartmentCommand { get; }

        private void DeleteDepartment()
        {
            if (SelectedDepartment != null)
            {
                if (!_deletedDepartments.Contains(SelectedDepartment.DepartmentId))
                {
                    _deletedDepartments.Add(SelectedDepartment.DepartmentId);
                }

                SelectedDepartment.IsDeleted = true;
                SelectedDepartment.IsDirty = true;
                Departments.Remove(SelectedDepartment);
            }
        }

        public DelegateCommand SaveDepartmentCommand { get; }

        public bool CanSave
        {
            get
            {
                NotifyPropertyChanged("HasErrors"); //Call NotifyPropertyChanged on "HasErrors" for displaying/hiding tooltip
                NotifyPropertyChanged("AllErrors"); //Call NotifyPropertyChanged on "AllErrors" to update errors in tooltip

                //if (SelectedDepartment == null) return false;

                if (HasUnSavedChanges() == false) return false;

                var errObj = (from d in _departments where d.ErrorCount > 0 select d).ToList();

                if (errObj.Any()) return false;

                return true;
            }
        }

        private void Save()
        {
            try
            {
                var dirtyObjects = (from d in Departments where d.IsDirty select d).ToList();

                foreach (DemoDepartmentModel item in dirtyObjects)
                {
                    if (item.IsNew)
                    {
                        _repository.AddDepartment(item);
                        item.DepartmentId = _repository.GetDepartmentId(item.DepartmentName);
                        item.IsNew = false;
                    }
                    else if (item.IsDeleted)
                    {
                        _repository.DeleteDepartment(item.DepartmentId);
                        item.IsDeleted = false;
                    }
                    else //its an update
                    {
                        _repository.UpdateDepartment(item);
                    }

                    item.EndEdit();
                    item.IsDirty = false;
                }

                if (_deletedDepartments.Count > 0)
                {
                    foreach (var item in _deletedDepartments)
                    {
                        _repository.DeleteDepartment(item);
                    }
                    _deletedDepartments.Clear();
                }

                MessageBox.Show("All changes saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception on Save", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public ICommand ReloadDataCommand { get; }

        #endregion ICommand/Button methods

        #region Filters

        private string _deptNameFilter;

        public string DeptNameFilter
        {
            get => _deptNameFilter;
            set
            {
                if (value == _deptNameFilter) return;
                SetPropertyValue(ref _deptNameFilter, value);
                ExecuteApplyFilters();
            }
        }

        private void ExecuteApplyFilters()
        {
            if (DepartmentsView == null) return;

            if (string.IsNullOrEmpty(DeptNameFilter))
            {
                RefreshView();
                return;
            }

            try
            {
                _departmentFilterCriteria.Clear();

                if (!string.IsNullOrEmpty(DeptNameFilter))
                {
                    _departmentFilterCriteria.Add(x =>
                        x.DepartmentName != null && x.DepartmentName.ToLower().Contains(DeptNameFilter.ToLower()));
                }
            }
            catch (Exception oEx)
            {
                if (DepartmentsView is IEditableCollectionView editableCollectionView)
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
                    MessageBox.Show(oEx.Message, "Departments", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            finally
            {
                DepartmentsView.Filter = dynamic_Filter;
                NotifyPropertyChanged("DepartmentsView");
                RefreshView();
            }
        }

        private bool dynamic_Filter(object item)
        {
            if (_departmentFilterCriteria.Count == 0)
            {
                return true;
            }

            var emp = item as DemoDepartmentModel;
            return _departmentFilterCriteria.TrueForAll(x => x(emp));
        }

        private void RefreshView()
        {
            try
            {
                DepartmentsView.Refresh();
                NotifyPropertyChanged("DepartmentsView");
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Departments", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFilters()
        {
            try
            {
                _departmentFilterCriteria.Clear();
                RefreshView();

                DeptNameFilter = string.Empty;

                // Bring the current record back into view in case it moved
                if (SelectedDepartment != null && (SelectedDepartment.IsDeleted == false))
                {
                    DemoDepartmentModel current = SelectedDepartment;
                    DepartmentsView.MoveCurrentToFirst();
                    DepartmentsView.MoveCurrentTo(current);
                }

                RefreshView();
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Departments", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion Filters

        #region Closing/Disposing

        public override bool HasUnSavedChanges()
        {
            if (Departments == null) return false;

            var isDirtyObjects = (from d in Departments where d.IsDirty select d).ToList();

            if (isDirtyObjects.Any()) return true;

            if (_deletedDepartments.Count > 0) return true;

            return false;
        }

        protected override void DisposeManagedResources()
        {
            PropertyChanged -= NavigationDepartmentViewModel_PropertyChanged;

            if (_departments != null)
            {
                Departments.CollectionChanged -= Departments_CollectionChanged;
                DepartmentsView = null;
                _departments = null;
            }

            _deletedDepartments = null;
            _departmentFilterCriteria = null;
            _selectedDepartment = null;
        }

        #endregion Closing
    }
}
