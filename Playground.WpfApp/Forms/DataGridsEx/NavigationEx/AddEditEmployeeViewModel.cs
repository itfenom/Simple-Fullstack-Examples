using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToAutoProperty

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class AddEditEmployeeViewModel : ValidationPropertyChangedBase
    {
        private readonly IDemoEmpRepository _repository;
        private bool _cancelledByUser;
        public override string Title => _title;
        private readonly string _title;

        private readonly string _backupFirstName;
        private readonly string _backupLastName;
        private readonly int _backupSalary;
        private readonly string _backupJobTitle;
        private readonly int _backupDepartmentId;
        private readonly int _backupRating;
        private readonly DateTime _backupStartDate;
        private readonly bool _backupIsActive;

        private DemoEmployeeModel _employeeModel;

        public DemoEmployeeModel EmployeeModel
        {
            get => _employeeModel;
            private set => _employeeModel = value;
        }


        private bool _isEditing;

        public bool IsEditing
        {
            get => _isEditing;
            set => SetPropertyValue(ref _isEditing, value);
        }

        //Dialog closer
        private bool? _dialogResultDependencyPropertyVal;

        public bool? DialogResultDependencyPropertyVal
        {
            get => _dialogResultDependencyPropertyVal;
            set => SetPropertyValue(ref _dialogResultDependencyPropertyVal, value);
        }

        #region Skills stuff

        private List<Predicate<TechnicalSkillModel>> _skillFilterCriteria;

        private ObservableCollection<TechnicalSkillModel> _skills;

        public ObservableCollection<TechnicalSkillModel> Skills
        {
            get => _skills;
            set => SetPropertyValue(ref _skills, value);
        }

        private TechnicalSkillModel _selectedSkill;

        public TechnicalSkillModel SelectedSkill
        {
            get => _selectedSkill;
            set => SetPropertyValue(ref _selectedSkill, value);
        }

        public CollectionView SkillView { get; set; }

        private string _skillCountLabel;

        public string SkillCountLabel
        {
            get => _skillCountLabel;
            set => SetPropertyValue(ref _skillCountLabel, value);
        }

        private bool _selectAllSkills;

        public bool SelectAllSkills
        {
            get => _selectAllSkills;
            set
            {
                SetPropertyValue(ref _selectAllSkills, value);
                CheckAllSkills(value);
            }
        }

        private void CheckAllSkills(bool value)
        {
            foreach (var item in Skills)
            {
                item.IsDirty = true;
                item.IsChecked = value;
            }

            _skillCountLabel = Skills.Where(x => x.IsChecked).ToList().Count + " Selected";

            if (value)
            {
                _showCheckedSkills = true;
            }
            else
            {
                _showCheckedSkills = null;
            }

            NotifyPropertyChanged("ShowCheckedSkills");
            NotifyPropertyChanged("SkillCountLabel");
            NotifyPropertyChanged("Skills");
            NotifyPropertyChanged("SkillView");
            ApplySkillsFilter();
            //RefreshSkillView();
        }

        private bool? _showCheckedSkills;

        public bool? ShowCheckedSkills
        {
            get => _showCheckedSkills;
            set
            {
                SetPropertyValue(ref _showCheckedSkills, value);
                ApplySkillsFilter();
            }
        }

        #endregion

        #region JobTitle Stuff

        private ObservableCollection<DemoJobTitleModel> _jobTitles;

        public ObservableCollection<DemoJobTitleModel> JobTitles
        {
            get => _jobTitles;
            set => SetPropertyValue(ref _jobTitles, value);
        }

        private DemoJobTitleModel _selectedJobTitle;

        [Required(ErrorMessage = "Job-Title is required!")]
        [DataType(DataType.Text)]
        public DemoJobTitleModel SelectedJobTitle
        {
            get => _selectedJobTitle;
            set
            {
                SetPropertyValue(ref _selectedJobTitle, value);
                ValidateProperty(value);
                if (value != null)
                {
                    EmployeeModel.JobTitle = value.JobTitle;
                }
            }
        }

        #endregion

        #region Department Stuff

        private ObservableCollection<DemoDepartmentModel> _departments;

        public ObservableCollection<DemoDepartmentModel> Departments
        {
            get => _departments;
            set
            {
                SetPropertyValue(ref _departments, value);
                ValidateProperty(value);
            }
        }

        private DemoDepartmentModel _selectedDepartment;

        [Required(ErrorMessage = "Department is required!")]
        public DemoDepartmentModel SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                SetPropertyValue(ref _selectedDepartment, value);
                ValidateProperty(value);
                if (value != null)
                {
                    EmployeeModel.DepartmentId = value.DepartmentId;
                    EmployeeModel.DepartmentName = value.DepartmentName;
                }
            }
        }

        #endregion

        public AddEditEmployeeViewModel(DemoEmployeeModel empModel)
        {
             _repository = new DemoEmpRepository();
            _skillFilterCriteria = new List<Predicate<TechnicalSkillModel>>();
            EmployeeModel = empModel;

            _backupFirstName = empModel.FirstName;
            _backupLastName = empModel.LastName;
            _backupSalary = empModel.Salary;
            _backupJobTitle = empModel.JobTitle;
            _backupRating = empModel.Rating;
            _backupStartDate = empModel.StartDate;
            _backupDepartmentId = empModel.DepartmentId;
            _backupIsActive = empModel.IsActive;

            ShowCheckedSkills = null;
            NotifyPropertyChanged("ShowCheckedSkills");

            //Set Title
            if (empModel.IsNew)
            {
                _isEditing = false;
                _title = "Create new Employee";
            }
            else
            {
                _isEditing = true;
                _title = $"Editing Employee: {EmployeeModel.Id} - {EmployeeModel.FirstName} {EmployeeModel.LastName}";
            }

            NotifyPropertyChanged("Title");
            NotifyPropertyChanged("IsEditing");

            //Instantiate commands
            SkillCheckBoxCommand = new DelegateCommand(() => OnSkillCheckBoxClick());
            SaveCommand = new DelegateCommand(() => OnSave(), () => CanSave);
            CancelCommand = new DelegateCommand(() => OnCancel());

            //Load Skills
            LoadSkills(EmployeeModel.SkillList);

            //Load JobTitles
            _jobTitles = new ObservableCollection<DemoJobTitleModel>(_repository.GetAllJobTitles());
            NotifyPropertyChanged("JobTitles");

            //Load Departments
            _departments = new ObservableCollection<DemoDepartmentModel>(_repository.GetAllDepartments());
            NotifyPropertyChanged("Departments");

            //Set values for JobTitle/DepartmentId drop-downs
            SelectedJobTitle = !string.IsNullOrEmpty(EmployeeModel.JobTitle) ? _jobTitles.FirstOrDefault(x => x.JobTitle == EmployeeModel.JobTitle) : null;
            SelectedDepartment = _departments.FirstOrDefault(x => x.DepartmentId == EmployeeModel.DepartmentId);

            PropertyChanged += AddEditEmployeeViewModel_PropertyChanged;
        }

        private void AddEditEmployeeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void LoadSkills(string employeeSkillList)
        {
            var allSkills = GetAllSkills();
            _skills = new ObservableCollection<TechnicalSkillModel>();
            if (string.IsNullOrEmpty(employeeSkillList))
            {
                _skills = new ObservableCollection<TechnicalSkillModel>(allSkills);
                _skillCountLabel = "0 Selected";
            }
            else
            {
                var selectedSkills = employeeSkillList.Split(',').ToList();
                foreach (var item in allSkills)
                {
                    _skills.Add(new TechnicalSkillModel
                    {
                        Skill = item.Skill,
                        IsChecked = selectedSkills.Contains(item.Skill)
                    });
                }

                _skillCountLabel = _skills.Where(x => x.IsChecked).ToList().Count + " Selected";
            }

            SkillView = (ListCollectionView)CollectionViewSource.GetDefaultView(_skills);
            NotifyPropertyChanged("SkillView");
            Skills.CollectionChanged += Skills_CollectionChanged;

            NotifyPropertyChanged("Skills");
            NotifyPropertyChanged("SelectAllSkills");
            NotifyPropertyChanged("SkillCountLabel");

        }

        private void Skills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshSkillView();
        }

        private List<TechnicalSkillModel> GetAllSkills()
        {
            var retVal = new List<TechnicalSkillModel>
            {
                new TechnicalSkillModel {Skill = "C#", IsChecked = false},
                new TechnicalSkillModel {Skill = "Excel", IsChecked = false},
                new TechnicalSkillModel {Skill = "JQuery", IsChecked = false},
                new TechnicalSkillModel {Skill = "JavaScript", IsChecked = false},
                new TechnicalSkillModel {Skill = "WPF", IsChecked = false},
                new TechnicalSkillModel {Skill = "MVVM", IsChecked = false},
                new TechnicalSkillModel {Skill = "MVC", IsChecked = false},
                new TechnicalSkillModel {Skill = "Asp.net", IsChecked = false},
                new TechnicalSkillModel {Skill = "Razor", IsChecked = false},
                new TechnicalSkillModel {Skill = "HTML", IsChecked = false},
                new TechnicalSkillModel {Skill = "Ajax", IsChecked = false},
                new TechnicalSkillModel {Skill = "MS Word", IsChecked = false},
                new TechnicalSkillModel {Skill = "Power Point", IsChecked = false},
                new TechnicalSkillModel {Skill = "Windows", IsChecked = false},
                new TechnicalSkillModel {Skill = "Linux", IsChecked = false},
                new TechnicalSkillModel {Skill = "PHP", IsChecked = false},
                new TechnicalSkillModel {Skill = "Python", IsChecked = false},
                new TechnicalSkillModel {Skill = "Visual Basic", IsChecked = false},
                new TechnicalSkillModel {Skill = "Entity Framework", IsChecked = false},
                new TechnicalSkillModel {Skill = "Oracle", IsChecked = false},
                new TechnicalSkillModel {Skill = "PL/SQL", IsChecked = false},
                new TechnicalSkillModel {Skill = "T-SQL", IsChecked = false},
                new TechnicalSkillModel {Skill = "Windows Forms", IsChecked = false},
                new TechnicalSkillModel {Skill = "Web Forms", IsChecked = false},
                new TechnicalSkillModel {Skill = "Visual Studio", IsChecked = false},
                new TechnicalSkillModel {Skill = "XAML", IsChecked = false},
                new TechnicalSkillModel {Skill = "XML", IsChecked = false},
                new TechnicalSkillModel {Skill = "JSON", IsChecked = false},
                new TechnicalSkillModel {Skill = "Spot Fire", IsChecked = false},
                new TechnicalSkillModel {Skill = "Crystal Reports", IsChecked = false},
                new TechnicalSkillModel {Skill = "SQL Developer", IsChecked = false},
            };

            return retVal.OrderBy(x => x.Skill).ToList();
        }

        #region Commands

        public ICommand SkillCheckBoxCommand { get; }

        private void OnSkillCheckBoxClick()
        {
            if (SelectedSkill != null)
            {
                SelectedSkill.IsDirty = true;
            }

            ApplySkillsFilter();
            var checkedSkillsCount = _skills.Where(x => x.IsChecked).ToList().Count;

            if (checkedSkillsCount == _skills.Count)
            {
                _selectAllSkills = true;
            }
            else
            {
                _selectAllSkills = false;
            }

            NotifyPropertyChanged("SelectAllSkills");

            _skillCountLabel = checkedSkillsCount + " Selected";
            NotifyPropertyChanged("SkillCountLabel");
        }

        public DelegateCommand SaveCommand { get; }

        private bool CanSave
        {
            get
            {
                if (EmployeeModel != null)
                {
                    return (!EmployeeModel.HasErrors &&
                            EmployeeModel.ErrorCount == 0 &&
                            !HasErrors &&
                            ErrorCount == 0);
                }

                return false;
            }
        }

        private void OnSave()
        {
            var checkedSkills = _skills.Where(x => x.IsChecked).Select(x => x.Skill).ToList();
            if (checkedSkills.Count == 0)
            {
                MessageBox.Show("You must select at least one skill!");
                return;
            }

            EmployeeModel.SkillList = string.Join(",", checkedSkills);
            if (EmployeeModel.IsNew)
            {
                EmployeeModel.Id = _repository.GetNextEmployeeId();
                _repository.AddEmployee(EmployeeModel);
            }
            else
            {
                _repository.UpdateEmployee(EmployeeModel);
            }
            _repository.InsertTechnicalSkills(EmployeeModel.Id, checkedSkills);

            //close this window
            _cancelledByUser = true;
            DialogResultDependencyPropertyVal = true;
        }

        public ICommand CancelCommand { get; }

        private void OnCancel()
        {
            _cancelledByUser = true;
            DialogResultDependencyPropertyVal = false;
        }
        #endregion

        #region Skills Filtering

        private string _skillFilterVal;

        public string SkillFilterVal
        {
            get => _skillFilterVal;
            set
            {
                if (_skillFilterVal == value) return;
                SetPropertyValue(ref _skillFilterVal, value);
                ApplySkillsFilter();
            }
        }

        private void RefreshSkillView()
        {
            try
            {
                if (SkillView != null)
                {
                    SkillView.Refresh();
                    NotifyPropertyChanged("SkillView");
                }
            }
            catch (Exception oEx)
            {
                if (SkillView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    SkillView.Filter = Skill_Filter;
                    NotifyPropertyChanged("SkillView");
                }
                else
                {
                    MessageBox.Show(oEx.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool Skill_Filter(object item)
        {
            if (_skillFilterCriteria.Count == 0)
                return true;

            var s = item as TechnicalSkillModel;
            return _skillFilterCriteria.TrueForAll(x => x(s));
        }

        private void ApplySkillsFilter()
        {
            try
            {
                if (SkillView == null) return;

                _skillFilterCriteria.Clear();

                if (!string.IsNullOrEmpty(SkillFilterVal))
                {
                    if (_showCheckedSkills == null)
                    {
                        _skillFilterCriteria.Add(x => x.Skill != null &&
                                                        x.Skill.ToLower().Contains(SkillFilterVal.ToLower()) &&
                                                        (x.IsChecked || !x.IsChecked));
                    }
                    else if (_showCheckedSkills == true)
                    {
                        _skillFilterCriteria.Add(x => x.Skill != null &&
                                                        x.Skill.ToLower().Contains(SkillFilterVal.ToLower()) &&
                                                        x.IsChecked);
                    }
                    else if (_showCheckedSkills == false)
                    {
                        _skillFilterCriteria.Add(x => x.Skill != null &&
                                                        x.Skill.ToLower().Contains(SkillFilterVal.ToLower()) &&
                                                        !x.IsChecked);
                    }
                }
                else
                {
                    if (_showCheckedSkills == null)
                    {
                        _skillFilterCriteria.Add(x => x.IsChecked || !x.IsChecked);
                    }
                    else if (_showCheckedSkills == true)
                    {
                        _skillFilterCriteria.Add(x => x.IsChecked);
                    }
                    else if (_showCheckedSkills == false)
                    {
                        _skillFilterCriteria.Add(x => !x.IsChecked);
                    }
                }
            }
            catch (Exception oEx)
            {
                if (SkillView is IEditableCollectionView editableCollectionView)
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
                    MessageBox.Show(oEx.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            finally
            {
                if (SkillView != null)
                {
                    SkillView.Filter = Skill_Filter;
                    NotifyPropertyChanged("SkillView");
                }
            }
        }

        #endregion

        #region Closing/Disposing

        public override bool HasUnSavedChanges()
        {
            if (!_isEditing)
            {
                return true;
            }

            var isDirtySkills = (from p in _skills where p.IsDirty select p).ToList().Count;
            if (isDirtySkills > 0)
            {
                return true;
            }

            if (EmployeeModel.FirstName != _backupFirstName ||
                EmployeeModel.LastName != _backupLastName ||
                EmployeeModel.Salary != _backupSalary ||
                EmployeeModel.JobTitle != _backupJobTitle ||
                EmployeeModel.Rating != _backupRating ||
                EmployeeModel.DepartmentId != _backupDepartmentId ||
                EmployeeModel.StartDate != _backupStartDate ||
                EmployeeModel.IsActive != _backupIsActive)
            {
                return true;
            }
            
            return false;
        }

        public override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_cancelledByUser)
            {
                e.Cancel = false; //go ahead and close!
                Dispose();
            }
            else
            {
                if (HasUnSavedChanges())
                {
                    var result = MessageBox.Show(
                        "Unsaved changes found, Discard changes and close?",
                        "Confirm Close",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        e.Cancel = true; //cancel closing!
                        return;
                    }
                }

                e.Cancel = false; //go ahead and close!
            }
        }

        protected override void DisposeManagedResources()
        {
            if (_skills != null)
            {
                Skills.CollectionChanged -= Skills_CollectionChanged;
                _skills = null;
                SkillView = null;
            }

            _selectedSkill = null;
            _skillFilterVal = null;
            _skillFilterCriteria = null;
            _selectedJobTitle = null;
            _jobTitles = null;
            _departments = null;
            _selectedDepartment = null;
            EmployeeModel = null;
            PropertyChanged -= AddEditEmployeeViewModel_PropertyChanged;
        }
        #endregion
    }

    public class TechnicalSkillModel : ValidationPropertyChangedBase
    {
        private string _skill;

        public string Skill
        {
            get => _skill;
            set
            {
                if (_skill == value) return;
                SetPropertyValue(ref _skill, value);
            }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                SetPropertyValue(ref _isChecked, value);
            }
        }
    }
}
