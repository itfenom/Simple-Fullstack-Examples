using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Repositories
{
    public interface IDemoEmpRepository
    {
        List<DemoJobTitleModel> GetAllJobTitles();

        #region Departments
        List<DemoDepartmentModel> GetAllDepartments();
        int GetDepartmentId(string departmentName);
        void AddDepartment(DemoDepartmentModel model);
        void UpdateDepartment(DemoDepartmentModel model);
        void DeleteDepartment(int departmentId);
        #endregion

        #region Employees
        List<DemoEmployeeModel> GetAllEmployees();

        int GetNextEmployeeId();

        void AddEmployee(DemoEmployeeModel model);

        void UpdateEmployee(DemoEmployeeModel model);

        void DeleteEmployee(int employeeId);
        #endregion

        #region Techincal Skills
        DataTable GetAllSkills();

        void InsertTechnicalSkills(int employeeId, List<string> skills);

        #endregion
    }

    public class DemoEmpRepository : IDemoEmpRepository
    {
        public List<DemoJobTitleModel> GetAllJobTitles()
        {
            var retVal = new List<DemoJobTitleModel>();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM DEMO_JOB_TITLE ORDER BY JOB_TITLE");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoJobTitleModel { JobTitle = row["JOB_TITLE"].ToString() });
            }
            return retVal;
        }

        #region Departments
        public List<DemoDepartmentModel> GetAllDepartments()
        {
            var retVal = new List<DemoDepartmentModel>();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM DEMO_DEPARTMENT");
            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoDepartmentModel
                {
                    DepartmentId = Convert.ToInt32(row["DEPARTMENT_ID"]),
                    DepartmentName = row["DEPARTMENT_NAME"].ToString()
                });
            }

            return retVal;
        }

        public int GetDepartmentId(string departmentName)
        {
            return Convert.ToInt32(DAL.Seraph.ExecuteScalar(
                $"SELECT DEPARTMENT_ID FROM DEMO_DEPARTMENT WHERE DEPARTMENT_NAME = '{departmentName}'"));
        }
        public void AddDepartment(DemoDepartmentModel model)
        {
            var sql = $@"INSERT INTO DEMO_DEPARTMENT(DEPARTMENT_ID, DEPARTMENT_NAME)
                        VALUES((SELECT MAX(DEPARTMENT_ID) + 1 FROM DEMO_DEPARTMENT), '{model.DepartmentName}')";
            DAL.Seraph.ExecuteNonQuery(sql);
        }

        public void UpdateDepartment(DemoDepartmentModel model)
        {
            DAL.Seraph.ExecuteNonQuery($@"UPDATE DEMO_DEPARTMENT SET DEPARTMENT_NAME = '{model.DepartmentName}' WHERE DEPARTMENT_ID = {model.DepartmentId}");
        }

        public void DeleteDepartment(int departmentId)
        {
            DAL.Seraph.ExecuteNonQuery($"DELETE FROM DEMO_DEPARTMENT WHERE DEPARTMENT_ID = {departmentId}");
        }
        #endregion

        #region Employees
        public List<DemoEmployeeModel> GetAllEmployees()
        {
            var retVal = new List<DemoEmployeeModel>();
            var dt = DAL.Seraph.ExecuteQuery(@"
                    SELECT A.*, B.DEPARTMENT_NAME
                    FROM DEMO_EMPLOYEE A
                        INNER JOIN DEMO_DEPARTMENT B
                            ON A.DEPARTMENT_ID = B.DEPARTMENT_ID 
                    ORDER BY A.ID");

            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new DemoEmployeeModel
                {
                    Id = Convert.ToInt32(row["ID"]),
                    FirstName = row["FIRST_NAME"].ToString(),
                    LastName = row["LAST_NAME"].ToString(),
                    Salary = Convert.ToInt32(row["SALARY"]),
                    DepartmentId = Convert.ToInt32(row["DEPARTMENT_ID"]),
                    DepartmentName = row["DEPARTMENT_NAME"].ToString(),
                    Rating = Convert.ToInt32(row["RATING"]),
                    JobTitle = row["JOB_TITLE"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString() == "1",
                    StartDate = Convert.ToDateTime(row["START_DATE"])
                });
            }
            return retVal;
        }

        public int GetNextEmployeeId()
        {
            return Convert.ToInt32(DAL.Seraph.ExecuteScalar("SELECT DEMO_EMPLOYEE_SEQ.NEXTVAL FROM DUAL"));
        }

        public void AddEmployee(DemoEmployeeModel model)
        {
            var sql = $@"INSERT INTO DEMO_EMPLOYEE(ID, FIRST_NAME, LAST_NAME, SALARY, JOB_TITLE, DEPARTMENT_ID, RATING, IS_ACTIVE, START_DATE)
                        VALUES({model.Id}, 
                              '{HelperTools.FormatSqlString(model.FirstName)}',
                              '{HelperTools.FormatSqlString(model.LastName)}',
                               {model.Salary},
                               '{model.JobTitle}',
                               {model.DepartmentId},
                               {model.Rating}, 
                               {(model.IsActive ? 1 : 0)}, 
                                TO_DATE('{model.StartDate:MM/dd/yyyy}', 'MM/DD/YYYY'))";
            DAL.Seraph.ExecuteNonQuery(sql);
        }

        public void UpdateEmployee(DemoEmployeeModel model)
        {
            var sql = $@"UPDATE DEMO_EMPLOYEE
                        SET FIRST_NAME = '{HelperTools.FormatSqlString(model.FirstName)}',
                            LAST_NAME = '{HelperTools.FormatSqlString(model.LastName)}',
                            SALARY = {model.Salary}, 
                            DEPARTMENT_ID = {model.DepartmentId},
                            JOB_TITLE = '{model.JobTitle}',
                            RATING = {model.Rating},
                            START_DATE = TO_DATE('{model.StartDate:MM/dd/yyyy}', 'MM/DD/YYYY'),
                            IS_ACTIVE = {(model.IsActive ? 1 : 0)}
                        WHERE ID = {model.Id}";
            DAL.Seraph.ExecuteNonQuery(sql);
        }

        public void DeleteEmployee(int employeeId)
        {
            var anonymousBlock = new StringBuilder();
            anonymousBlock.AppendLine("DECLARE");
            anonymousBlock.AppendLine($"V_EMP_ID NUMBER := {employeeId};");
            anonymousBlock.AppendLine("BEGIN");
            anonymousBlock.AppendLine("DELETE FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = V_EMP_ID;");
            anonymousBlock.AppendLine("DELETE FROM DEMO_EMPLOYEE WHERE ID = V_EMP_ID;");
            anonymousBlock.AppendLine("END;");
            DAL.Seraph.ExecuteNonQuery(anonymousBlock.ToString());
        }

        #endregion

        #region Techincal Skills
        public DataTable GetAllSkills()
        {
            return DAL.Seraph.ExecuteQuery("SELECT * FROM DEMO_TECHNICAL_SKILL");
        }

        public void InsertTechnicalSkills(int employeeId, List<string> skills)
        {
            var anonymousBlock = new StringBuilder();
            anonymousBlock.AppendLine("DECLARE");
            anonymousBlock.AppendLine($"V_EMP_ID NUMBER := {employeeId};");
            anonymousBlock.AppendLine("BEGIN");
            anonymousBlock.AppendLine("DELETE FROM DEMO_TECHNICAL_SKILL WHERE EMPLOYEE_ID = V_EMP_ID;");

            foreach (var item in skills)
            {
                anonymousBlock.AppendLine($"INSERT INTO DEMO_TECHNICAL_SKILL VALUES(V_EMP_ID, '{item}');");
            }

            anonymousBlock.AppendLine("END;");
            DAL.Seraph.ExecuteNonQuery(anonymousBlock.ToString());
        }

        #endregion
    }

    public class DemoEmployeeModel : ValidationPropertyChangedBase, IEditableObject
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                if (_id == value) return;

                SetPropertyValue(ref _id, value);
                ValidateProperty(value);
            }
        }

        private string _firstName;

        [Required(ErrorMessage = "First Name is required!")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "First Name Should be minimum 2 characters and a maximum of 30 characters!")]
        [DataType(DataType.Text)]
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName == value) return;

                SetPropertyValue(ref _firstName, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private string _lastName;

        [Required(ErrorMessage = "Last Name is required!")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name Should be minimum 2 characters and a maximum of 30 characters!")]
        [DataType(DataType.Text)]
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName == value) return;

                SetPropertyValue(ref _lastName, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private int _salary;

        [Required(ErrorMessage = "Salary is required!")]
        [Range(100, 300000, ErrorMessage = "Salary must be between $100 and $300,000")]
        public int Salary
        {
            get => _salary;
            set
            {
                if (_salary == value) return;

                SetPropertyValue(ref _salary, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private string _jobTitle;

        [Required(ErrorMessage = "Job-Title is required!")]
        [DataType(DataType.Text)]
        public string JobTitle
        {
            get => _jobTitle;
            set
            {
                if (_jobTitle == value) return;

                SetPropertyValue(ref _jobTitle, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private int _departmentId;

        [Required(ErrorMessage = "Department is required!")]
        [Range(100, 200, ErrorMessage = "Invalid Department!")]
        public int DepartmentId
        {
            get => _departmentId;
            set
            {
                if (_departmentId == value) return;

                SetPropertyValue(ref _departmentId, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private string _departmentName;

        public string DepartmentName
        {
            get => _departmentName;
            set => SetPropertyValue(ref _departmentName, value);
        }

        private int _rating;

        [Required(ErrorMessage = "Rating is required!")]
        [Range(1, 10, ErrorMessage = "Invalid Rating, Rating must be between 1 and 10.")]
        public int Rating
        {
            get => _rating;
            set
            {
                if (_rating == value) return;

                SetPropertyValue(ref _rating, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                SetPropertyValue(ref _isActive, value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private DateTime _startDate;

        [Required(ErrorMessage = "Start-Date is required!")]
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate == value) return;

                SetPropertyValue(ref _startDate, value);
                ValidateProperty(value);
                CallNotifyPropertyChangesForErrors();
            }
        }

        private void CallNotifyPropertyChangesForErrors()
        {
            NotifyPropertyChanged("HasErrors"); //Call NotifyPropertyChanged on "HasErrors" for displaying/hiding tooltip
            NotifyPropertyChanged("AllErrors"); //Call NotifyPropertyChanged on "AllErrors" to update errors in tooltip
        }

        public int StartDecade => StartDate.Year / 10 * 10;

        private string _skillList;
        public string SkillList
        {
            get => _skillList;
            set
            {
                if (_skillList == value) return;

                SetPropertyValue(ref _skillList, value);
            }
        }

        public string FullName => $"{LastName}, {FirstName}";

        public override string ToString()
        {
            return $"{FirstName}, {LastName}";
        }

        public string ToString(Func<DemoEmployeeModel, string> formatPerson)
        {
            return formatPerson.Invoke(this);
        }

        #region IEditableObject implementation

        private DemoEmployeeModel _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (!_inEdit)
            {
                _backupCopy = MemberwiseClone() as DemoEmployeeModel;
                _inEdit = true;
                IsDirty = true;
            }
        }

        public void CancelEdit()
        {
            if (_inEdit)
            {
                Id = _backupCopy.Id;
                FirstName = _backupCopy.FirstName;
                LastName = _backupCopy.LastName;
                Salary = _backupCopy.Salary;
                JobTitle = _backupCopy.JobTitle;
                DepartmentId = _backupCopy.DepartmentId;
                DepartmentName = _backupCopy.DepartmentName;
                Rating = _backupCopy.Rating;
                StartDate = _backupCopy.StartDate;
                IsActive = _backupCopy.IsActive;
                _backupCopy = null;
                _inEdit = false;
            }
        }

        public void EndEdit()
        {
            if (_inEdit)
            {
                _backupCopy = null;
                _inEdit = false;
            }
        }

        #endregion IEditableObject implementation
    }

    public class DemoJobTitleModel : ValidationPropertyChangedBase, IEditableObject
    {
        private string _jobTitle;

        public string JobTitle
        {
            get => _jobTitle;
            set
            {
                if (_jobTitle == value) return;

                SetPropertyValue(ref _jobTitle, value);
                ValidateProperty(value);
            }
        }

        #region IEditableObject implementation

        private DemoJobTitleModel _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (!_inEdit)
            {
                _backupCopy = MemberwiseClone() as DemoJobTitleModel;
                _inEdit = true;
                IsDirty = true;
            }
        }

        public void CancelEdit()
        {
            if (_inEdit)
            {
                JobTitle = _backupCopy.JobTitle;
                _inEdit = false;
                //IsDirty = false; //(Not needed here!)
            }
        }

        public void EndEdit()
        {
            if (_inEdit)
            {
                _backupCopy = null;
                _inEdit = false;
                //IsDirty = false; //(Not needed here!)
            }
        }

        #endregion IEditableObject implementation

    }

    public class DemoDepartmentModel : ValidationPropertyChangedBase, IEditableObject
    {
        private int _departmentId;

        public int DepartmentId
        {
            get => _departmentId;
            set
            {
                if (_departmentId == value) return;

                SetPropertyValue(ref _departmentId, value);
                ValidateProperty(value);
            }
        }

        private string _departmentName;

        [Required(ErrorMessage = "Department is required!")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Department Name Should be minimum 2 characters and a maximum of 30 characters!")]
        [DataType(DataType.Text)]
        public string DepartmentName
        {
            get => _departmentName;
            set
            {
                if (_departmentName == value) return;

                SetPropertyValue(ref _departmentName, value);
                ValidateProperty(value);
            }
        }

        #region IEditableObject implementation

        private DemoDepartmentModel _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (!_inEdit)
            {
                _backupCopy = MemberwiseClone() as DemoDepartmentModel;
                _inEdit = true;
                IsDirty = true;
            }
        }

        public void CancelEdit()
        {
            if (_inEdit)
            {
                DepartmentId = _backupCopy.DepartmentId;
                DepartmentName = _backupCopy.DepartmentName;
                _inEdit = false;
                //IsDirty = false; //(Not needed here!)
            }
        }

        public void EndEdit()
        {
            if (_inEdit)
            {
                _backupCopy = null;
                _inEdit = false;
                //IsDirty = false; //(Not needed here!)
            }
        }

        #endregion IEditableObject implementation
    }
}
