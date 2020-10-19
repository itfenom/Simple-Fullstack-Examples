using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using DynamicData;
using Playground.Core.AdoNet;
using Playground.WpfApp.Mvvm.AttributedValidation;
using ReactiveUI;
// ReSharper disable RedundantNameQualifier
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable UnusedVariable

namespace Playground.WpfApp.Forms.ReactiveEx.Crud
{
    public class EmpViewModel : ValidatableBindableBase
    {
        public override string Title => "CRUD using ReactiveUI...";

        private List<int> _deletedEmployeeList;

        private readonly ReadOnlyObservableCollection<DemoEmpModel> _employees;
        public ReadOnlyObservableCollection<DemoEmpModel> Employees => _employees;

        private readonly ReadOnlyObservableCollection<DemoDeptModel> _departments;
        public ReadOnlyObservableCollection<DemoDeptModel> Departments => _departments;

        private DemoEmpModel _selectedEmployee;
        public DemoEmpModel SelectedEmployee
        {
            get => _selectedEmployee;
            set => this.RaiseAndSetIfChanged(ref _selectedEmployee, value);
        }

        public EmpViewModel()
        {
            _deletedEmployeeList = new List<int>();
            // ReSharper disable once IdentifierTypo
            var depts = GetAllDepartments().Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _departments)
                .Subscribe().DisposeWith(Disposables.Value);

            _empFirstNameFilter = string.Empty;
            _empLastNameFilter = string.Empty;
            _empJobTitleFilter = string.Empty;
            _empSalaryFilter = string.Empty;
            _empSkillListFilter = string.Empty;
            _selectedDepartmentFilter = null;

            //using reactive ui operator to respond to any change 
            var multipleFilters = this.WhenAnyValue(
                    x => x.EmpFirstNameFilter,
                    x => x.EmpLastNameFilter,
                    x => x.EmpJobTitleFilter,
                    x => x.EmpSalaryFilter,
                    x => x.EmpSkillListFilter,
                    x => x.SelectedDepartmentFilter)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Do(s =>
                {
                    Console.WriteLine($@"\r\nSearching for: {s}");
                }).Select(
                    searchTerm =>
                    {
                        var filters = BuildGroupFilter();

                        return row => filters.All(filter => filter(row as DemoEmpModel));

                        bool Searcher(DemoEmpModel item) => item.FirstName.ToLower().Contains(searchTerm.Item1.ToLower()) ||
                                                            item.LastName.ToLower().Contains(searchTerm.Item2.ToLower()) ||
                                                            item.JobTitle.JobTitle.ToLower().Contains(searchTerm.Item3.ToLower()) ||
                                                            item.Salary.ToString().ToLower().Contains(searchTerm.Item4.ToLower()) ||
                                                            item.SkillList.ToLower().Contains(searchTerm.Item5.ToLower()) ||
                                                            item.DepartmentId.ToString().Contains(searchTerm.Item6.DepartmentId.ToString());

                        return (Func<DemoEmpModel, bool>)Searcher;
                    });

            var employees = GetAllEmployees();
            var observableEmployees = employees.Connect()
                .Filter(multipleFilters)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _employees)
                .Subscribe().DisposeWith(Disposables.Value);

            //Clear filters
            ClearFiltersCommand = ReactiveCommand.Create(() =>
            {
                EmpFirstNameFilter = string.Empty;
                EmpLastNameFilter = string.Empty;
                EmpJobTitleFilter = string.Empty;
                EmpSalaryFilter = string.Empty;
                EmpSkillListFilter = string.Empty;
                SelectedDepartmentFilter = null;
            }).DisposeWith(Disposables.Value);

            //Delete Employee
            var isSelected = this.WhenAnyValue(x => x.SelectedEmployee, (DemoEmpModel c) => c != null);
            DeleteEmployeeCommand = ReactiveCommand.Create(
                () =>
                {
                    var result = MessageBox.Show($@"Are you sure, you want to delete {SelectedEmployee.FirstName} ?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SelectedEmployee.EditState = EditState.Deleted;
                        _deletedEmployeeList.Add(Convert.ToInt32(SelectedEmployee.Id));
                        employees.Remove(SelectedEmployee);
                        SelectedEmployee = null;
                    }
                }, isSelected).DisposeWith(Disposables.Value);

            //Add
            AddNewEmployeeCommand = ReactiveCommand.Create(() =>
            {
                var view = new EmpEditorView(-999, "NEW");
                view.ShowDialog();

                var viewModel = (EmpEditorViewModel)view.DataContext;
                if (viewModel.CloseWindowFlag == true)
                {
                    employees.Add(viewModel.Model);
                }

                viewModel.Dispose();

            }).DisposeWith(Disposables.Value);

            //Edit
            EditEmployeeCommand = ReactiveCommand.Create(() =>
            {
                var view = new EmpEditorView(Convert.ToInt32(SelectedEmployee.Id), "EDIT");
                view.ShowDialog();

                var viewModel = (EmpEditorViewModel)view.DataContext;
                if (viewModel.CloseWindowFlag == true)
                {
                    var updatedItem = new DemoEmpModel(Convert.ToInt32(SelectedEmployee.Id));
                    updatedItem.FirstName = viewModel.Model.FirstName;
                    updatedItem.LastName = viewModel.Model.LastName;
                    updatedItem.Salary = viewModel.Model.Salary;
                    updatedItem.DepartmentId = viewModel.Model.DepartmentId;
                    updatedItem.JobTitle = viewModel.Model.JobTitle;
                    updatedItem.Rating = viewModel.Model.Rating;
                    updatedItem.StartDate = viewModel.Model.StartDate;
                    updatedItem.IsActive = viewModel.Model.IsActive;
                    updatedItem.Skills = viewModel.Model.SkillList.Split(',').ToList();

                    employees.Remove(SelectedEmployee);
                    employees.Add(updatedItem);

                    SelectedEmployee = null;
                    SelectedEmployee = updatedItem;
                }

                viewModel.Dispose();
            }, isSelected).DisposeWith(Disposables.Value);

            //Save
            var canExecuteSave = this.WhenAny(
                x => x.SelectedEmployee,
                (e) =>
                    _deletedEmployeeList.Count > 0);

            SaveCommand = ReactiveCommand.Create(() => Save(), canExecuteSave).DisposeWith(Disposables.Value);

            //Cancel/Close
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });
        }

        private SourceList<DemoEmpModel> GetAllEmployees()
        {
            var empList = new SourceList<DemoEmpModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"
            SELECT A.*, 
                   C.DEPARTMENT_NAME,
                   B.SKILLS
            FROM DEMO_EMPLOYEE A
                LEFT JOIN 
                (
                  SELECT EMPLOYEE_ID,
                    LISTAGG(SKILL, ',') WITHIN GROUP(ORDER BY SKILL) AS SKILLS
                  FROM DEMO_TECHNICAL_SKILL
                  GROUP BY EMPLOYEE_ID
                )B
                    ON A.ID = B.EMPLOYEE_ID
                LEFT JOIN DEMO_DEPARTMENT C
                    ON A.DEPARTMENT_ID = C.DEPARTMENT_ID 
            ORDER BY A.ID");

            foreach (DataRow row in dt.Rows)
            {
                var skills = new List<string>();

                if (row["SKILLS"] != DBNull.Value)
                {
                    skills = row["SKILLS"].ToString().Split(',').Select(x => x).ToList();
                }

                var item = new DemoEmpModel(Convert.ToInt32(row["ID"]))
                {
                    FirstName = row["FIRST_NAME"].ToString(),
                    LastName = row["LAST_NAME"].ToString(),
                    Salary = Convert.ToInt32(row["SALARY"]),
                    JobTitle = new DemoEmpJobTitleModel(row["JOB_TITLE"].ToString()),
                    DepartmentId = Convert.ToInt32(row["DEPARTMENT_ID"]),
                    Rating = Convert.ToInt32(row["RATING"]),
                    IsActive = Convert.ToInt32(row["IS_ACTIVE"]),
                    StartDate = Convert.ToDateTime(row["START_DATE"]),
                    Skills = skills
                };

                empList.Add(item);
            }

            return empList;
        }

        private SourceList<DemoDeptModel> GetAllDepartments()
        {
            var retVal = new SourceList<DemoDeptModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"SELECT * FROM DEMO_DEPARTMENT ORDER BY DEPARTMENT_NAME");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoDeptModel(Convert.ToInt32(row["DEPARTMENT_ID"]), row["DEPARTMENT_NAME"].ToString()));
            }

            return retVal;
        }

        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        #region Filtering
        private string _empFirstNameFilter;

        public string EmpFirstNameFilter
        {
            get => _empFirstNameFilter;
            set => this.RaiseAndSetIfChanged(ref _empFirstNameFilter, value);
        }

        private string _empLastNameFilter;
        public string EmpLastNameFilter
        {
            get => _empLastNameFilter;
            set => this.RaiseAndSetIfChanged(ref _empLastNameFilter, value);
        }

        private string _empJobTitleFilter;
        public string EmpJobTitleFilter
        {
            get => _empJobTitleFilter;
            set => this.RaiseAndSetIfChanged(ref _empJobTitleFilter, value);
        }

        private string _empSalaryFilter;
        public string EmpSalaryFilter
        {
            get => _empSalaryFilter;
            set => this.RaiseAndSetIfChanged(ref _empSalaryFilter, value);
        }

        private string _empSkillListFilter;
        public string EmpSkillListFilter
        {
            get => _empSkillListFilter;
            set => this.RaiseAndSetIfChanged(ref _empSkillListFilter, value);
        }

        private DemoDeptModel _selectedDepartmentFilter;

        public DemoDeptModel SelectedDepartmentFilter
        {
            get => _selectedDepartmentFilter;
            set => this.RaiseAndSetIfChanged(ref _selectedDepartmentFilter, value);
        }

        private IEnumerable<Predicate<DemoEmpModel>> BuildGroupFilter()
        {
            if (!string.IsNullOrEmpty(EmpFirstNameFilter))
            {
                yield return rowView => rowView.FirstName.ToLower().Contains(EmpFirstNameFilter.ToLower());
            }

            if (!string.IsNullOrEmpty(EmpLastNameFilter))
            {
                yield return rowView => rowView.LastName.ToLower().Contains(EmpLastNameFilter.ToLower());
            }

            if (!string.IsNullOrEmpty(EmpJobTitleFilter))
            {
                yield return rowView => rowView.JobTitle.JobTitle.ToLower().Contains(EmpJobTitleFilter.ToLower());
            }

            if (!string.IsNullOrEmpty(EmpSalaryFilter))
            {
                yield return rowView => rowView.Salary.ToString().Contains(EmpSalaryFilter);
            }

            if (!string.IsNullOrEmpty(EmpSkillListFilter))
            {
                yield return rowView => rowView.SkillList.ToLower().Contains(EmpSkillListFilter.ToLower());
            }

            if (SelectedDepartmentFilter != null)
            {
                yield return rowView => rowView.DepartmentId.ToString().Contains(SelectedDepartmentFilter.DepartmentId.ToString());
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteEmployeeCommand { get; }

        public ReactiveCommand<Unit, Unit> AddNewEmployeeCommand { get; }

        public ReactiveCommand<Unit, Unit> EditEmployeeCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private void Save()
        {
            if (_deletedEmployeeList.Count == 0) return;

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("DECLARE");
            queryBuilder.AppendLine("BEGIN");

            foreach (var id in _deletedEmployeeList)
            {
                queryBuilder.AppendLine($@"DELETE FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = {id};");
                queryBuilder.AppendLine($@"DELETE FROM DEMO_EMPLOYEE WHERE ID = {id};");
            }

            queryBuilder.AppendLine("COMMIT;");
            queryBuilder.AppendLine("END;");
            DAL.Seraph.ExecuteNonQuery(queryBuilder.ToString());

            MessageBox.Show("Employee saved successfully!", "Save", MessageBoxButton.OK,
                MessageBoxImage.Information);

            _deletedEmployeeList.Clear();
        }

        #endregion

        #region Closing
        public bool HasUnsavedChanges()
        {
            if (CloseWindowFlag == true) return false;

            if (_deletedEmployeeList.Count > 0) return true;

            return false;
        }

        protected override void DisposeManagedResources()
        {
            _deletedEmployeeList = null;
            _selectedEmployee = null;
        }

        #endregion
    }

    public sealed class DemoEmpModel : EditableBindableBase, IEquatable<DemoEmpModel>
    {
        private string _firstName;
        private string _lastName;
        private int _salary;
        private DemoEmpJobTitleModel _jobTitle;
        private int _departmentId;
        private int _rating;
        private DateTime _startDate;
        private int _isActive;
        private IEnumerable<string> _skills = Enumerable.Empty<string>();

        public string SkillList
        {
            get
            {
                if (_skills.Any())
                {
                    return string.Join(",", _skills);
                }

                return string.Empty;
            }
        }

        public DemoEmpModel()
        {
            EditState = EditState.New;
        }

        public DemoEmpModel(int id)
        {
            Id = id;
            EditState = EditState.NotChanged;
        }

        public int? Id { get; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required!", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "First Name Should be minimum 2 characters and a maximum of 30 characters!")]
        [DataType(DataType.Text)]
        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required!", AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name Should be minimum 2 characters and a maximum of 30 characters!")]
        [DataType(DataType.Text)]
        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        [Display(Name = "Salary")]
        [Required(ErrorMessage = "Salary is required!", AllowEmptyStrings = false)]
        [Range(100, 300000, ErrorMessage = "Salary must be between $100 and $300,000")]
        public int Salary
        {
            get => _salary;
            set => this.RaiseAndSetIfChanged(ref _salary, value);
        }

        [Display(Name = "Rating")]
        [Required(ErrorMessage = "Rating is required!", AllowEmptyStrings = false)]
        [Range(1, 10, ErrorMessage = "Invalid Rating, Rating must be between 1 and 10.")]
        public int Rating
        {
            get => _rating;
            set => this.RaiseAndSetIfChanged(ref _rating, value);
        }

        public int IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start-Date is required!")]
        public DateTime StartDate
        {
            get => _startDate;
            set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }

        [Required]
        [Display(Name = "Job Title")]
        public DemoEmpJobTitleModel JobTitle
        {
            get => _jobTitle;
            set => this.RaiseAndSetIfChanged(ref _jobTitle, value);
        }

        [Required]
        public int DepartmentId
        {
            get => _departmentId;
            set => this.RaiseAndSetIfChanged(ref _departmentId, value);
        }

        // Gets a value indicating whether or not this instance has changes.
        public override bool IsChanged =>
            base.IsChanged
            && (Memento?.State == null || !Equals(Memento.State));

        [Required(ErrorMessage = "You must select at least one Skill.")]
        [CollectionMinimumLength(length: 1, ErrorMessage = "You must select at least one Skill.")]
        public IEnumerable<string> Skills
        {
            get => _skills;
            set => this.RaiseAndSetIfChanged(ref _skills, value);
        }

        /// <summary>
        /// Create the memento representing the objects state.
        /// </summary>
        /// <returns>The memento representing the objects state.</returns>
        protected override Memento CreateMemento()
        {
            return new Memento(new DemoEmpModel
            {
                FirstName = FirstName,
                LastName = LastName,
                Salary = Salary,
                StartDate = StartDate,
                Skills = Skills,
                Rating = Rating,
                IsActive = IsActive,
                DepartmentId = DepartmentId,
                JobTitle = JobTitle
            });
        }

        /// <summary>
        /// Restore the state of the object from the memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        protected override void RestoreMemento(Memento memento)
        {
            var oldState = memento?.State as DemoEmpModel;
            if (oldState == null)
            {
                return;
            }

            FirstName = oldState.FirstName;
            LastName = oldState.LastName;
            Salary = oldState.Salary;
            StartDate = oldState.StartDate;
            Skills = oldState.Skills;
            Rating = oldState.Rating;
            IsActive = oldState.IsActive;
            DepartmentId = oldState.DepartmentId;
            JobTitle = oldState.JobTitle;
        }

        #region Equality

        public static bool operator !=(DemoEmpModel model1, DemoEmpModel model2)
        {
            return !(model1 == model2);
        }

        public static bool operator ==(DemoEmpModel model1, DemoEmpModel model2)
        {
            if (ReferenceEquals(model1, model2))
            {
                return true;
            }

            if (ReferenceEquals(null, model1))
            {
                return false;
            }

            return model1.Equals(model2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DemoEmpModel);
        }

        public bool Equals(DemoEmpModel other)
        {
            // is the other item null?
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            // is the other item the same object as this instance?
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // check if all properties are equal
            return
                string.Equals(FirstName, other.FirstName)
                && string.Equals(LastName, other.LastName)
                && string.Equals(Salary, other.Salary)
                && string.Equals(Rating, other.Rating)
                && string.Equals(IsActive, other.IsActive)
                && string.Equals(StartDate.ToShortDateString(), other.StartDate.ToShortDateString())
                && JobTitle?.JobTitle == other.JobTitle?.JobTitle
                && DepartmentId == other.DepartmentId
                && ((Skills == null && other.Skills == null) || (Skills != null && other.Skills != null && Skills.SequenceEqual(other.Skills)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, FirstName) ? FirstName.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, LastName) ? LastName.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Salary.ToString()) ? Salary.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Rating.ToString()) ? Rating.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, IsActive.ToString()) ? IsActive.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, StartDate.ToShortDateString()) ? StartDate.ToShortDateString().GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, JobTitle) ? JobTitle.JobTitle.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, DepartmentId.ToString()) ? DepartmentId.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Skills) ? Skills.Distinct().Aggregate(HashingBase, (x, y) => (x * HashingMultiplier) ^ y.GetHashCode()) : 0);

                return hash;
            }
        }

        #endregion
    }

    public sealed class DemoEmpJobTitleModel : IEquatable<DemoEmpJobTitleModel>
    {
        public DemoEmpJobTitleModel(string jobTitle)
        {
            JobTitle = jobTitle;
        }

        public string JobTitle { get; }

        public override string ToString()
        {
            return JobTitle;
        }

        #region Equality
        public static bool operator !=(DemoEmpJobTitleModel jobTitleA, DemoEmpJobTitleModel jobTitleB)
        {
            return !(jobTitleA == jobTitleB);
        }

        public static bool operator ==(DemoEmpJobTitleModel jobTitleA, DemoEmpJobTitleModel jobTitleB)
        {
            if (ReferenceEquals(jobTitleA, jobTitleB))
            {
                return true;
            }

            if (ReferenceEquals(null, jobTitleA))
            {
                return false;
            }

            return jobTitleA.Equals(jobTitleB);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DemoEmpJobTitleModel);
        }

        public bool Equals(DemoEmpJobTitleModel other)
        {
            return !ReferenceEquals(null, other) && JobTitle == other.JobTitle;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // choose some large prime numbers to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ JobTitle.GetHashCode();
                //hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, CallLabel) ? CallLabel.GetHashCode() : 0);
                //hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, DefaultCategory) ? DefaultCategory.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion
    }

    public sealed class DemoDeptModel : IEquatable<DemoDeptModel>
    {
        public DemoDeptModel(int deptId, string deptName)
        {
            DepartmentId = deptId;
            DepartmentName = deptName;
        }

        public int DepartmentId { get; }
        public string DepartmentName { get; }

        #region Equality
        public static bool operator !=(DemoDeptModel deptA, DemoDeptModel deptB)
        {
            return !(deptA == deptB);
        }

        public static bool operator ==(DemoDeptModel deptA, DemoDeptModel deptB)
        {
            if (ReferenceEquals(deptA, deptB))
            {
                return true;
            }

            if (ReferenceEquals(null, deptA))
            {
                return false;
            }

            return deptA.Equals(deptB);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DemoDeptModel);
        }

        public bool Equals(DemoDeptModel other)
        {
            return !ReferenceEquals(null, other)
                   && DepartmentId == other.DepartmentId
                   && string.Equals(DepartmentName, other.DepartmentName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // choose some large prime numbers to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ DepartmentId.GetHashCode();
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, DepartmentName) ? DepartmentName.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion
    }

    public class DemoSkillsModel : BindableBase
    {
        private bool _isChecked;

        public DemoSkillsModel(string skill)
        {
            Skill = skill;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public string Skill { get; }
    }
}
