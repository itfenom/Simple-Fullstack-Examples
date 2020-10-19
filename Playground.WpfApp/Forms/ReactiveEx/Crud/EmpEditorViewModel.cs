using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Playground.Core.AdoNet;
using Playground.WpfApp.Mvvm.AttributedValidation;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud
{
    public class EmpEditorViewModel : ValidatableBindableBase
    {
        private enum EditorType
        {
            Edit,
            New
        }

        public override string Title => _title;
        private string _title;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly EditorType _editorType;

        private readonly List<DemoSkillsModel> _allSkills = new List<DemoSkillsModel>();
        private string _skillFilter;

        [ValidateObject]
        public DemoEmpModel Model { get; }

        public string SkillFilter
        {
            get => _skillFilter;
            set => this.RaiseAndSetIfChanged(ref _skillFilter, value);
        }

        private bool _filterBySelectedSkills;

        public bool FilterBySelectedSkills
        {
            get => _filterBySelectedSkills;
            set => this.RaiseAndSetIfChanged(ref _filterBySelectedSkills, value);
        }

        private ICollectionView _skillsCollection;

        public ICollectionView SkillCollectionView
        {
            get
            {
                if (_skillsCollection == null)
                {
                    _skillsCollection = CollectionViewSource.GetDefaultView(_allSkills);
                    _skillsCollection.Filter += s => Filter_Skills(s as DemoSkillsModel);
                }

                return _skillsCollection;
            }
        }

        private bool Filter_Skills(DemoSkillsModel demoSkillsModel)
        {
            if (demoSkillsModel == null)
            {
                return true;
            }

            return demoSkillsModel.Skill.IndexOf(SkillFilter ?? "", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   (!FilterBySelectedSkills || demoSkillsModel.IsChecked);
        }

        public List<DemoDeptModel> AllDepartments { get; } = new List<DemoDeptModel>();

        public List<DemoEmpJobTitleModel> AllJobTitles { get; } = new List<DemoEmpJobTitleModel>();

        public int SelectedSkillCount => _allSkills.Count(s => s.IsChecked);

        public EmpEditorViewModel(int id, string transactionType)
        {
            var alljobTitles = GetAllJobTitles();
            var allDepts = GetAllDepartments();
            var allSkils = GetAllSkills();
            var selectedSkill = new List<string>();

            AllDepartments.AddRange(allDepts);
            AllJobTitles.AddRange(alljobTitles);

            if (transactionType == "NEW")
            {
                _editorType = EditorType.New;
                _title = "Adding new Employee";

                id = Convert.ToInt32(DAL.Seraph.ExecuteScalar("SELECT DEMO_EMPLOYEE_SEQ.NEXTVAL FROM DUAL"));

                Model = new DemoEmpModel(id)
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Salary = -999,
                    DepartmentId = 101,
                    EditState = EditState.New,
                    IsActive = 1,
                    Rating = 0,
                    StartDate = DateTime.Today
                };
            }
            else
            {
                _editorType = EditorType.Edit;
                _title = $"Editing Employee: {id}";

                var dt = DAL.Seraph.ExecuteQuery($@"
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
                WHERE A.ID = {id}");

                Model = new DemoEmpModel(id)
                {
                    FirstName = dt.Rows[0]["FIRST_NAME"].ToString(),
                    LastName = dt.Rows[0]["LAST_NAME"].ToString(),
                    Salary = Convert.ToInt32(dt.Rows[0]["SALARY"].ToString()),
                    DepartmentId = Convert.ToInt32(dt.Rows[0]["DEPARTMENT_ID"]),
                    JobTitle = alljobTitles.Find(x => x.JobTitle == dt.Rows[0]["JOB_TITLE"].ToString()),
                    IsActive = Convert.ToInt32(dt.Rows[0]["IS_ACTIVE"]),
                    Rating = Convert.ToInt32(dt.Rows[0]["RATING"])
                };

                if (dt.Rows[0]["SKILLS"] != DBNull.Value)
                {
                    selectedSkill = dt.Rows[0]["SKILLS"].ToString().Split(',').ToList();
                }
            }

            _allSkills.AddRange(allSkils.Distinct());
            _allSkills.ForEach(s => s.WhenAnyValue(p => p.IsChecked).Skip(1).Subscribe(_ =>
            {
                Model.Skills = _allSkills.Where(sk => sk.IsChecked).Select(sk => sk.Skill).ToList();

                // notify that properties associated with the Skills have changed
                this.RaisePropertyChanged(nameof(SkillFilter));
                this.RaisePropertyChanged(nameof(SelectedSkillCount));
            }).DisposeWith(Disposables.Value));

            // when a filter changes, refresh the Skills collection
            this.WhenAnyValue(x => x.SkillFilter, x => x.FilterBySelectedSkills)
                .ObserveOnDispatcher()
                .Subscribe(_ => SkillCollectionView.Refresh());

            if (selectedSkill.Count > 0)
            {
                using (Model.DelayChangeNotifications())
                {
                    foreach (var skill in _allSkills)
                    {
                        if (selectedSkill.Contains(skill.Skill))
                        {
                            skill.IsChecked = true;
                        }
                    }
                }

                Model.EndEdit();
            }

            Model.BeginEdit();

            //Save
            var canSave = this.WhenAnyValue(x => x.HasErrors, hasError => !hasError);
            SaveCommand = ReactiveCommand.Create(() =>
            {
                if (!Model.IsChanged)
                {
                    return;
                }

                if (HasErrors)
                {
                    MessageBox.Show("Validation errors must be corrected before saving!", "Save", MessageBoxButton.OK,
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
                    VALUES({Model.Id},
                            '{Model.FirstName}',
                            '{Model.LastName}',
                             {Model.Salary},
                            '{Model.JobTitle}',
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
                        JOB_TITLE = '{Model.JobTitle}',
                        DEPARTMENT_ID = {Model.DepartmentId},
                        RATING = {Model.Rating},
                        IS_ACTIVE = {Model.IsActive}
                    WHERE ID = {Model.Id};");

                    queryBuilder.AppendLine($@"DELETE FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = {Model.Id};");
                }

                foreach (var item in _allSkills.Where(x => x.IsChecked))
                {
                    queryBuilder.AppendLine($@"
                        INSERT INTO DEMO_TECHNICAL_SKILL(EMPLOYEE_ID, SKILL)VALUES({Model.Id}, '{item.Skill}');");
                }

                queryBuilder.AppendLine("COMMIT;");
                queryBuilder.AppendLine("END;");
                DAL.Seraph.ExecuteNonQuery(queryBuilder.ToString());

                MessageBox.Show("Employee saved successfully!", "Save", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Model.AcceptChanges();
                Model.EndEdit();
                CloseWindowFlag = true;

            }, canSave).DisposeWith(Disposables.Value);

            //Cancel/Close
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });

            Model.ErrorsChanged += Model_ErrorsChanged;
            Model.BeginEdit();
            ObserveModel(Model);
        }

        private List<DemoDeptModel> GetAllDepartments()
        {
            var retVal = new List<DemoDeptModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"SELECT * FROM DEMO_DEPARTMENT ORDER BY DEPARTMENT_NAME");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoDeptModel(Convert.ToInt32(row["DEPARTMENT_ID"]), row["DEPARTMENT_NAME"].ToString()));
            }

            return retVal;
        }

        private List<DemoEmpJobTitleModel> GetAllJobTitles()
        {
            var retVal = new List<DemoEmpJobTitleModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"SELECT * FROM DEMO_JOB_TITLE ORDER BY JOB_TITLE");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoEmpJobTitleModel(row["JOB_TITLE"].ToString()));
            }

            return retVal;
        }

        private List<DemoSkillsModel> GetAllSkills()
        {
            var retVal = new List<DemoSkillsModel>
            {
                new DemoSkillsModel("C#"),
                new DemoSkillsModel("Excel"),
                new DemoSkillsModel("JQuery"),
                new DemoSkillsModel("JavaScript"),
                new DemoSkillsModel("WPF"),
                new DemoSkillsModel("MVVM"),
                new DemoSkillsModel("MVC"),
                new DemoSkillsModel("Asp.net"),
                new DemoSkillsModel("Razor"),
                new DemoSkillsModel("HTML"),
                new DemoSkillsModel("AJAX"),
                new DemoSkillsModel("Words"),
                new DemoSkillsModel("Power Point"),
                new DemoSkillsModel("Windows"),
                new DemoSkillsModel("Linux"),
                new DemoSkillsModel("PHP"),
                new DemoSkillsModel("Python"),
                new DemoSkillsModel("Visual Basic"),
                new DemoSkillsModel("Entity Framework"),
                new DemoSkillsModel("Oracle"),
                new DemoSkillsModel("PL/SQL"),
                new DemoSkillsModel("Windows Forms"),
                new DemoSkillsModel("Web Forms"),
                new DemoSkillsModel("Visual Studio"),
                new DemoSkillsModel("XAML"),
                new DemoSkillsModel("XML"),
                new DemoSkillsModel("JSON"),
                new DemoSkillsModel("Spot Fire"),
                new DemoSkillsModel("SQL Developer"),
                new DemoSkillsModel("ReactiveUI"),
                new DemoSkillsModel("T-SQL")
            };

            return retVal.OrderBy(x => x.Skill).ToList();
        }

        private void Model_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ValidateProperty(nameof(Model));
        }

        private void ObserveModel(DemoEmpModel model)
        {
            #region Commented-out
            //// watch for changes to the call-level
            //var callLevelChanged = model.WhenAny(x => x.CallLevel, x => new { x.Sender, CallLevel = x.Value }).Skip(1);

            //Predicate<string> callLevelIsProduct = callLevel => "product".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            //Predicate<string> callLevelIsLot = callLevel => "lot".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            //Predicate<string> callLevelIsState = callLevel => "state".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            //Predicate<string> callLevelIsWafer = callLevel => "wafer".Equals(callLevel, StringComparison.OrdinalIgnoreCase);

            //callLevelChanged
            //    .Where(x => callLevelIsProduct(x.CallLevel))
            //    .Select(x => x.Sender)
            //    .Subscribe(x =>
            //    {
            //        x.Lot = "*";
            //        x.Wafer = "*";
            //        x.State = "*";
            //    })
            //    .DisposeWith(Disposables.Value);

            //callLevelChanged
            //    .Where(x => callLevelIsLot(x.CallLevel))
            //    .Select(x => x.Sender)
            //    .Subscribe(x =>
            //    {
            //        x.Wafer = "*";
            //        x.State = "*";
            //    })
            //    .DisposeWith(Disposables.Value);

            //callLevelChanged
            //    .Where(x => callLevelIsState(x.CallLevel) || callLevelIsWafer(x.CallLevel))
            //    .Select(x => x.Sender)
            //    .Subscribe(x =>
            //    {
            //        x.Wafer = "*";
            //    })
            //    .DisposeWith(Disposables.Value);

            //// the lot can be edited if not in read-only mode, and the call-level is not PRODUCT
            //_canEditLot = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditLot)
            //    .DisposeWith(Disposables.Value);

            //// the wafer can be edited if not in read-only mode, and the call-level is not PRODUCT, LOT, or STATE
            //_canEditWafer = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel) && !callLevelIsState(callLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditWafer)
            //    .DisposeWith(Disposables.Value);

            //// the wafer can be edited if not in read-only mode, and the call-level is not PRODUCT or LOT
            //_canEditState = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditState)
            //    .DisposeWith(Disposables.Value);

            //model.WhenAnyValue(x => x.Function, x => x.Lot, x => x.Products)
            //    .Subscribe(_ =>
            //    {
            //        /* NOTE: When the function, lot, or product changes, we need to refresh the categories that can be used
            //        * based on the current context, which is made up of those values */
            //        CalculateLowestRankInCurrentContext();
            //        Categories.Refresh();
            //    })
            //    .DisposeWith(Disposables.Value);

            //// when the lot changes, select the product (if any) associated with that lot
            //var productForLot = model.WhenAnyValue(x => x.Lot)
            //    .Select(lot => lot?.Trim())
            //    .Select(lot => string.IsNullOrEmpty(lot) || lot.Length != 7 ? null : DAL.TQTGAAS.GetProductByLot(lot));

            //* When the Read-Only state changes, or the product associated with the lot changes, we want to update whether or not
            // * products are selectable based on those values. */
            //_canSelectProducts = Observable.CombineLatest(
            //    this.WhenAnyValue(x => x.IsReadOnly),
            //    productForLot.Select(product => !string.IsNullOrEmpty(product) && _allProducts.Any(prod => prod.Product == product)),
            //    (readOnly, validProduct) => !readOnly && !validProduct)
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanSelectProducts)
            //    .DisposeWith(Disposables.Value);

            //productForLot
            //    .Skip(1)
            //    .DistinctUntilChanged()
            //    .Subscribe(product =>
            //    {
            //        /* When the product changes in response to a change in the lot, we want to select the product associated with the lot.
            //         * If there is no product associated with the lot, we'll de-select all products */
            //        foreach (var prod in _allProducts)
            //        {
            //            prod.IsChecked = prod.Product == product;
            //        }
            //    })
            //    .DisposeWith(Disposables.Value);
            #endregion

            ValidateProperty(nameof(Model));
        }

        #region Commands
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        #endregion

        #region Closing

        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        public bool HasUnsavedChanges()
        {
            if (!Model.IsChanged)
            {
                return true;
            }

            return false;
        }

        protected override void DisposeManagedResources()
        {
            Model.ErrorsChanged -= Model_ErrorsChanged;
        }

        #endregion
    }
}
