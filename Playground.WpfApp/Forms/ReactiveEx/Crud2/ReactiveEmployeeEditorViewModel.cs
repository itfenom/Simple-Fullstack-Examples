using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Playground.Core.AdoNet;
using Playground.WpfApp.Mvvm.AttributedValidation;
using ReactiveUI;
using MessageBox = System.Windows.MessageBox;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud2
{
    public class ReactiveEmployeeEditorViewModel : ValidatableBindableBase
    {
        private enum EditorType
        {
            Edit,
            New
        }

        private string _title;
        public override string Title => _title;

        private bool _isDataLoading;

        public bool SaveSuccessfulFlag { get; set; }

        private readonly EditorType _editorType;

        public List<ReactiveEmployeeJobTitleModel> AllJobTitles { get; private set; }

        private ReactiveEmployeeJobTitleModel _selectedJobTitle;

        [Required(ErrorMessage = "Job Title is required!")]
        public ReactiveEmployeeJobTitleModel SelectedJobTitle
        {
            get => _selectedJobTitle;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedJobTitle, value);
                if (value != null && Model != null)
                {
                    Model.JobTitle = value;
                }
            }
        }

        public List<ReactiveEmployeeDeptModel> AllDepartments { get; private set; }

        private ReactiveEmployeeDeptModel _selectedDepartment;

        [Required(ErrorMessage = "Department is required!")]
        public ReactiveEmployeeDeptModel SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDepartment, value);
                if (value != null && Model != null)
                {
                    Model.DepartmentId = value.DepartmentId;
                }
            }
        }

        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        #region Skills

        private ReactiveEmployeeSkillsModel _selectedSkill;

        public ReactiveEmployeeSkillsModel SelectedSkill
        {
            get => _selectedSkill;
            set => this.RaiseAndSetIfChanged(ref _selectedSkill, value);
        }

        public ICollectionView SkillsView { get; set; }
        #endregion

        [ValidateObject]
        public ReactiveEmployeeModel Model { get; }

        public ReactiveEmployeeEditorViewModel(int id, string editType)
        {
            _isDataLoading = true;

            AllJobTitles = new List<ReactiveEmployeeJobTitleModel>();
            AllJobTitles.AddRange(GetAllJobTitles());

            AllDepartments = new List<ReactiveEmployeeDeptModel>();
            AllDepartments.AddRange(GetAllDepartments());

            switch (editType)
            {
                case "NEW":
                    _editorType = EditorType.New;
                    _title = "Creating new Employee";
                    break;
                case "EDIT":
                    _editorType = EditorType.Edit;
                    _title = $"Editing Employee: {id}";
                    break;
            }

            this.RaisePropertyChanged(nameof(Title));

            Model = new ReactiveEmployeeModel();

            if (_editorType == EditorType.New)
            {
                Model.EmployeeId = Convert.ToInt32(DAL.Seraph.ExecuteScalar("SELECT DEMO_EMPLOYEE_SEQ.NEXTVAL FROM DUAL"));
                Model.FirstName = string.Empty;
                Model.LastName = string.Empty;
                Model.Salary = -0;
                Model.DepartmentId = 0;
                Model.JobTitle = null;
                Model.IsActive = 1;
                Model.Rating = 0;
                Model.StartDate = DateTime.Today;
            }
            else
            {
                var modelForId = GetModel(id);
                Model.EmployeeId = id;
                Model.FirstName = modelForId.FirstName;
                Model.LastName = modelForId.LastName;
                Model.Salary = modelForId.Salary;

                //Model.DepartmentId = modelForId.DepartmentId;
                //Model.JobTitle = modelForId.JobTitle;

                SelectedDepartment = AllDepartments.FirstOrDefault(d => d.DepartmentId == modelForId.DepartmentId);
                SelectedJobTitle = AllJobTitles.FirstOrDefault(j => j.JobTitle == modelForId.JobTitle.JobTitle);

                Model.IsActive = modelForId.IsActive;
                Model.Rating = modelForId.Rating;
                Model.StartDate = modelForId.StartDate;
            }

            Model.Skills = new List<ReactiveEmployeeSkillsModel>(GetSkills(Model.EmployeeId));
            this.WhenAnyValue(x => x.SelectedSkill.Skill).Subscribe(_ =>
            {
                Model.SkillList = Model.Skills.Select(s => s.Skill).ToList();
            }).DisposeWith(Disposables.Value);

            /*
             We need to wire-up the listening to SelectedSkill instead!!!! (KASHIF: 02/27/2020)
            Model.Skills.ForEach(x => x.WhenAnyValue(
                s => s.Skill,
                s => s.IsFavorite).Subscribe(_ =>
                {
                    Console.WriteLine($@"Skill clicked: '{x.Skill}'");
                    Model.SkillList = Model.Skills.Select(ss => ss.Skill).ToList();
                    this.RaisePropertyChanged(nameof(SelectedSkill));

                }).DisposeWith(Disposables.Value));
            */

            if(_editorType == EditorType.Edit)
            {
                //wire-up listening to events on changes in SKILLS
                Model.Skills.ForEach(s => s.WhenAnyValue(sk => sk.Skill).Subscribe(_ =>
                {
                    if (!_isDataLoading)
                    {
                        s.EditState = EditState.Changed;
                        SelectedSkill = null;

                        this.RaisePropertyChanged(nameof(SelectedSkill));
                        
                        SelectedSkill = s;
                        this.RaisePropertyChanged(nameof(SelectedSkill));
                    }
                }).DisposeWith(Disposables.Value));
            }

            SkillsView = CollectionViewSource.GetDefaultView(Model.Skills);
            this.RaisePropertyChanged(nameof(SkillsView));

            //Set first skill as selected
            SelectedSkill = Model.Skills.FirstOrDefault(s => s.SortIndex == 1);
            this.RaisePropertyChanged(nameof(SelectedSkill));

            //Sort skills by sortIndex in the View
            SkillsView.SortDescriptions.Clear();
            SkillsView.SortDescriptions.Add(new SortDescription("EmployeeId", ListSortDirection.Ascending));
            SkillsView.SortDescriptions.Add(new SortDescription("SortIndex", ListSortDirection.Ascending));

            //AddNew Skill
            AddNewSkillCommand = ReactiveCommand.Create(() =>
            {
                int nextIndexVal = 0;

                if (Model.Skills.Count > 0)
                {
                    nextIndexVal = (from x in Model.Skills select x.SortIndex).Max();
                }

                var newSkill = new ReactiveEmployeeSkillsModel()
                {
                    EmployeeId = Model.EmployeeId,
                    Skill = string.Empty,
                    SortIndex = nextIndexVal + 1,
                    IsFavorite = false,
                    EditState = EditState.New
                };

                Model.Skills.Add(newSkill);
                SelectedSkill = null;
                SelectedSkill = newSkill;
                this.RaisePropertyChanged(nameof(SelectedSkill));

                UpdateSkillList();
                RefreshSkillsView();

            }).DisposeWith(Disposables.Value);

            //Delete
            var selectedNotNullObservable = this.WhenAnyValue(x => x.SelectedSkill,
                (ReactiveEmployeeSkillsModel p) => p != null);

            DeleteSkillCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedSkill == null) return;

                Model.Skills.Remove(SelectedSkill);
                SelectedSkill = null;
                this.RaisePropertyChanged(nameof(SelectedSkill));

                var count = 1;
                foreach (var path in Model.Skills)
                {
                    path.SortIndex = count;
                    path.EditState = EditState.Changed;
                    count++;
                }

                UpdateSkillList();
                RefreshSkillsView();

            }, selectedNotNullObservable)
                .DisposeWith(Disposables.Value);

            //Move skill up
            MoveSkillUpCommand = ReactiveCommand.Create(() => { MoveSkillUpOrDown("UP"); }, selectedNotNullObservable).DisposeWith(Disposables.Value);

            //Move skill down
            MoveSkillDownCommand = ReactiveCommand.Create(() => { MoveSkillUpOrDown("DOWN"); }, selectedNotNullObservable).DisposeWith(Disposables.Value);

            //IsFavorite
            IsFavoriteCheckBoxCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedSkill != null && SelectedSkill.EditState != EditState.New)
                {
                    var selectedSkillRow = SelectedSkill;
                    selectedSkillRow.EditState = EditState.Changed;

                    SelectedSkill = null;
                    this.RaisePropertyChanged(nameof(SelectedSkill));

                    SelectedSkill = selectedSkillRow;
                    this.RaisePropertyChanged(nameof(SelectedSkill));
                    ValidateProperty(nameof(SelectedSkill));
                }

            }).DisposeWith(Disposables.Value);

            //Save
            var canExecuteSave = this.WhenAnyValue(
                x => x.Model.Skills,
                x => x.SelectedSkill,
                x => x.SelectedSkill.IsFavorite,
                x => x.HasErrors,
                (skills, selectedSkill, isFav, hasErr)
                    =>
                {
                    var emptySkillCount = Model.Skills.Where(s => string.IsNullOrEmpty(s.Skill)).ToList().Count;

                    return (skills != null && skills.Count() > 0) &&
                           selectedSkill != null &&
                           !hasErr &&
                           HasUnsavedChanges();
                }
            );

            SaveCommand = ReactiveCommand.Create(() => Save(), canExecuteSave).DisposeWith(Disposables.Value);

            //Cancel/Close window Command
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });

            Model.ErrorsChanged += Model_ErrorsChanged;
            Model.BeginEdit();
            UpdateSkillList();
            _isDataLoading = false;
        }

        private void Model_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ValidateProperty(nameof(Model));
        }

        private List<ReactiveEmployeeJobTitleModel> GetAllJobTitles()
        {
            var retVal = new List<ReactiveEmployeeJobTitleModel>();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM DEMO_JOB_TITLE ORDER BY JOB_TITLE");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new ReactiveEmployeeJobTitleModel(row["JOB_TITLE"].ToString()));
            }

            return retVal;
        }

        private List<ReactiveEmployeeDeptModel> GetAllDepartments()
        {
            var retVal = new List<ReactiveEmployeeDeptModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"SELECT * FROM DEMO_DEPARTMENT ORDER BY DEPARTMENT_NAME");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new ReactiveEmployeeDeptModel(Convert.ToInt32(row["DEPARTMENT_ID"]), row["DEPARTMENT_NAME"].ToString()));
            }

            return retVal;
        }

        public ReactiveEmployeeModel GetModel(int id)
        {
            var retVal = new ReactiveEmployeeModel();
            var dt = DAL.Seraph.ExecuteQuery($@"
            SELECT A.*, 
                   C.DEPARTMENT_NAME,
                   B.SKILLS
            FROM DEMO_EMPLOYEE A
                LEFT JOIN 
                (
                  SELECT EMPLOYEE_ID,
                    LISTAGG(SKILL, ',') WITHIN GROUP(ORDER BY SORT_INDEX) AS SKILLS
                  FROM DEMO_TECHNICAL_SKILL
                  GROUP BY EMPLOYEE_ID
                )B
                    ON A.ID = B.EMPLOYEE_ID
                LEFT JOIN DEMO_DEPARTMENT C
                    ON A.DEPARTMENT_ID = C.DEPARTMENT_ID 
            WHERE A.ID = {id}");

            if (dt.Rows.Count > 0)
            {
                retVal.EmployeeId = Convert.ToInt32(dt.Rows[0]["ID"]);
                retVal.FirstName = dt.Rows[0]["FIRST_NAME"].ToString();
                retVal.LastName = dt.Rows[0]["LAST_NAME"].ToString();
                retVal.Salary = Convert.ToInt32(dt.Rows[0]["SALARY"]);
                retVal.DepartmentId = Convert.ToInt32(dt.Rows[0]["DEPARTMENT_ID"]);
                retVal.JobTitle = new ReactiveEmployeeJobTitleModel(dt.Rows[0]["JOB_TITLE"].ToString());
                retVal.IsActive = Convert.ToInt32(dt.Rows[0]["IS_ACTIVE"]);
                retVal.Rating = Convert.ToInt32(dt.Rows[0]["RATING"]);
                retVal.StartDate = Convert.ToDateTime(dt.Rows[0]["START_DATE"]);
            }

            return retVal;
        }

        private List<ReactiveEmployeeSkillsModel> GetSkills(int id)
        {
            var retVal = new List<ReactiveEmployeeSkillsModel>();
            var dt = DAL.Seraph.ExecuteQuery($"SELECT * FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = {id} ORDER BY EMPLOYEE_ID, SORT_INDEX");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = new ReactiveEmployeeSkillsModel
                    {
                        EmployeeId = Convert.ToInt32(row["EMPLOYEE_ID"]),
                        IsFavorite = row["IS_FAVORITE"].ToString() == "Y",
                        Skill = row["SKILL"].ToString(),
                        SortIndex = Convert.ToInt32(row["SORT_INDEX"])
                    };
                    retVal.Add(item);
                }

                return retVal.OrderBy(x => x.SortIndex).ToList();
            }

            var emptyItem = new ReactiveEmployeeSkillsModel
            {
                EmployeeId = id,
                IsFavorite = false,
                Skill = string.Empty,
                SortIndex = 1
            };

            retVal.Add(emptyItem);

            return retVal;
        }

        private void UpdateSkillList()
        {
            Model.SkillList = (from p in Model.Skills select p.Skill).ToList();

            this.RaisePropertyChanged(nameof(HasErrors));
            this.RaisePropertyChanged(nameof(AllErrors));
        }

        private void RefreshSkillsView()
        {
            try
            {
                SkillsView.Refresh();
                this.RaisePropertyChanged(nameof(SkillsView));
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region Commands
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> AddNewSkillCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteSkillCommand { get; }

        public ReactiveCommand<Unit, Unit> MoveSkillUpCommand { get; }

        public ReactiveCommand<Unit, Unit> MoveSkillDownCommand { get; }

        private void MoveSkillUpOrDown(string direction)
        {
            //If there's no skill or this is the only skill in the list ==> do nothing!
            if (SelectedSkill == null || Model.Skills == null || Model.Skills.Count <= 1) return;

            var newSkillIndexValue = 0;
            var otherSkillIndexValue = 0;
            ReactiveEmployeeSkillsModel otherRow = new ReactiveEmployeeSkillsModel();

            if (direction == "UP")
            {
                //If this skill is already at the top ==> do nothing!
                if (SelectedSkill.SortIndex == 1) return;

                otherRow =
                    (from p in Model.Skills
                     where p.SortIndex == SelectedSkill.SortIndex - 1
                     select p).FirstOrDefault();

                if (otherRow == null) return;

                newSkillIndexValue = otherRow.SortIndex;
                otherSkillIndexValue = SelectedSkill.SortIndex;
            }

            if (direction == "DOWN")
            {
                //If this index is already at the bottom ==> do nothing!
                if (SelectedSkill.SortIndex == Model.Skills.Count) return;

                otherRow =
                    (from p in Model.Skills
                     where p.EmployeeId == Model.EmployeeId &&
                           p.SortIndex == SelectedSkill.SortIndex + 1
                     select p).FirstOrDefault();

                if (otherRow == null) return;

                newSkillIndexValue = otherRow.SortIndex;
                otherSkillIndexValue = SelectedSkill.SortIndex;
            }

            var currentRow = SelectedSkill;
            var currentRowPath = SelectedSkill.Skill;
            var currentRowServerOnlyVal = SelectedSkill.IsFavorite;

            SelectedSkill = null;
            this.RaisePropertyChanged(nameof(SelectedSkill));

            otherRow.SortIndex = otherSkillIndexValue;
            otherRow.EditState = EditState.Changed;

            currentRow.SortIndex = newSkillIndexValue;
            currentRow.Skill = currentRowPath;
            currentRow.IsFavorite = currentRowServerOnlyVal;
            currentRow.EditState = EditState.Changed;

            SelectedSkill = currentRow;
            ValidateProperty(nameof(SelectedSkill));

            //Raise Property Changed events
            this.RaisePropertyChanged(nameof(SelectedSkill));

            RefreshSkillsView();
        }

        public ReactiveCommand<Unit, Unit> IsFavoriteCheckBoxCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private void Save()
        {
            if (!HasUnsavedChanges())
            {
                return;
            }

            var errors = GetValidationErrors();

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errors), "Save", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("DECLARE");
            queryBuilder.AppendLine("BEGIN");

            if (_editorType == EditorType.New)
            {
                queryBuilder.AppendLine($@"
                    INSERT INTO DEMO_EMPLOYEE(ID, FIRST_NAME, LAST_NAME, SALARY, JOB_TITLE, DEPARTMENT_ID, RATING, START_DATE, IS_ACTIVE)
                    VALUES({Model.EmployeeId},
                            '{Model.FirstName}',
                            '{Model.LastName}',
                             {Model.Salary},
                            '{Model.JobTitle.JobTitle}',
                             {Model.DepartmentId},
                             {Model.Rating},
                             SYSDATE,
                             {Model.IsActive});");
            }
            else if (_editorType == EditorType.Edit)
            {
                queryBuilder.AppendLine($@"
                    UPDATE DEMO_EMPLOYEE 
                    SET 
                        FIRST_NAME = '{Model.FirstName}',
                        LAST_NAME = '{Model.LastName}',
                        SALARY = {Model.Salary},
                        JOB_TITLE = '{Model.JobTitle.JobTitle}',
                        DEPARTMENT_ID = {Model.DepartmentId},
                        RATING = {Model.Rating},
                        IS_ACTIVE = {Model.IsActive}
                    WHERE ID = {Model.EmployeeId};");

                queryBuilder.AppendLine($@"DELETE FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = {Model.EmployeeId};");
            }

            foreach (var item in Model.Skills)
            {
                var isFavorite = item.IsFavorite ? "Y" : "N";
                queryBuilder.AppendLine($@"
                    INSERT INTO 
                        DEMO_TECHNICAL_SKILL(EMPLOYEE_ID, SKILL, SORT_INDEX, IS_FAVORITE)
                        VALUES({Model.EmployeeId}, '{item.Skill}', {item.SortIndex}, '{isFavorite}');");
            }

            try
            {
                queryBuilder.AppendLine();
                queryBuilder.AppendLine("COMMIT;");
                queryBuilder.AppendLine("END;");
                DAL.Seraph.ExecuteNonQuery(queryBuilder.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Model.SkillList = Model.Skills.OrderBy(x => x.SortIndex).Select(x => x.Skill).ToList();

            MessageBox.Show("Changes saved Successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            SaveSuccessfulFlag = true;
            CloseWindowFlag = true;
        }

        private List<string> GetValidationErrors()
        {
            var retVal = new List<string>();

            if (HasErrors)
            {
                foreach (var err in AllErrors)
                {
                    retVal.Add(err.ErrorMessage);
                }
            }

            if (Model.Skills.Count == 0)
            {
                retVal.Add("Invalid Skill(s)!");
            }

            foreach (var sk in Model.Skills)
            {
                if (string.IsNullOrEmpty(sk.Skill))
                {
                    retVal.Add("Skill cannot be empty.");
                    break;
                }
            }

            var isFavCount = (from p in Model.Skills
                              where p.IsFavorite
                              select p).ToList().Count;

            if (isFavCount > 5)
            {
                retVal.Add("Max allowed Favorites is 5!");
            }

            return retVal;
        }

        #endregion

        #region Closing

        public bool HasUnsavedChanges()
        {
            if (CloseWindowFlag == true) return false;

            if (_editorType == EditorType.New) return true;

            //if (Model.EditState == EditState.Changed) return true;

            //Validate empty skill
            var emptySkills = (from p in Model.Skills
                               where string.IsNullOrEmpty(p.Skill)
                               select p).ToList();

            if (emptySkills.Count > 0) return true;

            //Validate if skill was added/removed or moved up/down
            var changedSkills = (from p in Model.Skills
                                 where p.EditState == EditState.Changed ||
                                       p.EditState == EditState.Deleted ||
                                       p.EditState == EditState.New
                                 select p).ToList();

            if (changedSkills.Count > 0) return true;

            if (SelectedSkill != null)
            {
                if (SelectedSkill.EditState == EditState.Changed ||
                    SelectedSkill.EditState == EditState.Deleted ||
                    SelectedSkill.EditState == EditState.New)
                    return true;
            }

            if (Model.IsChanged)
            {
                return true;
            }

            return false;
        }

        protected override void DisposeManagedResources()
        {
            ErrorsChanged -= Model_ErrorsChanged;

            _selectedDepartment = null;
            _selectedJobTitle = null;
            SkillsView = null;
            Model.Dispose();
        }

        #endregion

    }
}
