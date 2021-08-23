using Dapper;
using Oracle.ManagedDataAccess.Client;
using Playground.Mvc.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace Playground.Mvc.Models
{
    public class XyzEmployeeRepository : DapperBaseRepository
    {
        protected override string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SeraphOracle"].ToString();
        }

        public XyzEmployeePagingModel GetEmployeeFilteredList(JqGridRequest request)
        {
            var pageStart = request.PerPage * (request.Page - 1);
            var pageEnd = pageStart + request.PerPage;
            var sortOrder = string.Empty;

            if(!string.IsNullOrEmpty(request.SortByProperty))
            {
                sortOrder = request.SortOrder == SortOrder.Ascending ? $" ORDER BY {request.SortByProperty} ASC" : $" ORDER BY {request.SortByProperty} DESC";
            }

            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();
                var sql = $@"SELECT * FROM 
                    (
                    SELECT ROWNUM RNUM, A.*
                    FROM ({SelectColumnList<XyzEmployee>(request.SearchPropertiesAndTerms)}) A
                    WHERE ROWNUM <= {pageEnd}
                    )
                    WHERE RNUM > {pageStart}
                    {sortOrder}";

                var countQuery = SelectCount<XyzEmployee>();

                var items = dbConn.Query<XyzEmployee>(sql);
                var count = Convert.ToInt32(dbConn.ExecuteScalar(countQuery));
                var totalPages = Math.Ceiling((double)count / request.PerPage);

                var retVal = new XyzEmployeePagingModel
                {
                    ItemCount = count,
                    CurrentPage = request.Page
                };
                retVal.TotalPages = Convert.ToInt32(totalPages);
                retVal.EmployeeList = items;

                return retVal;
            }
        }

        public IEnumerable<object> GetDistinctColumnValues(string columnName)
        {
            var sql = SelectDistinctColumnValues<XyzEmployee>(columnName);
            var items = Select<string>(sql);
            var itemsList = new List<string>(items);
            itemsList.Sort();
            return itemsList;
        }

        public ManageXyzEmployeeModel GetEmployees()
        {
            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                var sql = "SELECT * FROM XYZ_EMPLOYEE";
                var employees = dbConn.Query<XyzEmployee>(sql);

                sql = @"SELECT ID as Id, 
                        EMPLOYEE_ID AS EmployeeId,  
                        Company_Name as CompanyName, 
                        TITLE, 
                        DISPLAY_SEQUENCE AS DisplaySequence, 
                        SALARY, 
                        SKILLS, 
                        HOBBIES, 
                        HIRE_DATE AS HireDate, 
                        STATUS
                        FROM XYZ_EMPLOYEE_WORK_HISTORY";
                var empWorkHistory = dbConn.Query<XyzEmployeeWorkHistory>(sql);

                var retVal = new ManageXyzEmployeeModel();
                retVal.Employees = new List<XyzEmployee>();
                foreach (var emp in employees)
                {
                    var employee = new XyzEmployee();
                    employee.Id = emp.Id;
                    employee.Name = emp.Name;
                    employee.Company = emp.Company;
                    employee.Email = emp.Email;
                    employee.Gender = emp.Gender;

                    var empWorkHist = empWorkHistory.Where(x => x.EmployeeId == emp.Id).ToList();
                    if (empWorkHist != null)
                    {
                        employee.WorkHistory = empWorkHist;
                    }
                    else
                    {
                        employee.WorkHistory = new List<XyzEmployeeWorkHistory>();
                    }

                    retVal.Employees.Add(employee);
                }

                return retVal;
            }
        }

        public Result InsertNewEmployee(string name, string company, string email, string gender, string salary, string title)
        {
            var result = new Result();
            result.IsSucceed = true;
            result.Message = string.Empty;

            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();
                using (var trans = dbConn.BeginTransaction())
                {
                    try
                    {
                        //insert into XYZ_EMPLOYEE
                        var empId = dbConn.ExecuteScalar<int>("SELECT XYZ_EMPLOYEE_SEQ.NEXTVAL FROM DUAL", null, trans);

                        var sql = $@"Insert into XYZ_EMPLOYEE (ID,NAME,COMPANY,EMAIL,GENDER) 
                        values ({empId},'{name}','{company}','{email}','{gender}')";

                        dbConn.Execute(sql, null, trans);

                        //Now, insert into XYZ_EMPLOYEE_WORK_HISTORY
                        sql = $@"INSERT INTO XYZ_EMPLOYEE_WORK_HISTORY (ID,EMPLOYEE_ID,COMPANY_NAME,TITLE,DISPLAY_SEQUENCE,SALARY,STATUS) 
                             VALUES (NULL,{empId},'{company}','{title}',{1},{salary},'Active')";

                        dbConn.Execute(sql, null, trans);

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        result.IsSucceed = false;
                        result.Message = e.Message;
                    }
                }
            }

            return result;
        }

        public Result InsertWorkHistory(string empId, string company, string title, string salary, string skills, string hobbies, string status, string hireDate)
        {
            var result = new Result();
            result.IsSucceed = true;
            result.Message = string.Empty;

            var skillsVal = string.IsNullOrEmpty(skills) ? "NULL" : $"'{skills}'";
            var hobbiesVal = string.IsNullOrEmpty(hobbies) ? "NULL" : $"'{hobbies}'";
            var hireDateVal = $"TO_DATE('{hireDate}', 'MM/DD/YYYY')";

            try
            {
                using (var dbConn = new OracleConnection(GetConnectionString()))
                {
                    dbConn.Open();
                    var sql = $@"INSERT INTO XYZ_EMPLOYEE_WORK_HISTORY (ID, EMPLOYEE_ID, COMPANY_NAME, TITLE, DISPLAY_SEQUENCE, SALARY, SKILLS, HOBBIES, STATUS, HIRE_DATE) 
                          VALUES (NULL, {empId}, '{company}', '{title}',
                          (SELECT NVL(MAX(DISPLAY_SEQUENCE) + 1, 1) AS DISP_SEQ FROM XYZ_EMPLOYEE_WORK_HISTORY WHERE EMPLOYEE_ID = {empId}),
                          {salary}, {skillsVal}, {hobbiesVal},'{status}', {hireDateVal})";
                    dbConn.Execute(sql);
                }
            }
            catch (Exception e)
            {
                result.IsSucceed = false;
                result.Message = e.Message;
            }

            return result;
        }

        public DataTable GetWorkHistoryDataTable(string employeeId, string workHistoryId)
        {
            var sql = $@"
            SELECT A.* , 
                   B.FILE_NAME, 
                   B.FILE_TYPE
            FROM  XYZ_EMPLOYEE_WORK_HISTORY A
                LEFT JOIN XYZ_EMPLOYEE_FILE B
                    ON A.ID = B.EMPLOYEE_WORK_HISTORY_ID
            WHERE A.ID = {workHistoryId}
            AND A.EMPLOYEE_ID = {employeeId}";

            var ds = GetDataSet(sql);

            return ds.Tables[0];
        }

        public Result UpdateWorkHistory(XyzEmployeeUpdateModel model)
        {
            var result = new Result
            {
                IsSucceed = true,
                Message = string.Empty
            };

            var skillsVal = string.IsNullOrEmpty(model.Skills) ? "NULL" : $"'{model.Skills}'";
            var hobbiesVal = string.IsNullOrEmpty(model.Hobbies) ? "NULL" : $"'{model.Hobbies}'";
            var hireDateVal = $"TO_DATE('{model.HireDate}', 'MM/DD/YYYY')";

            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();
                using (var trans = dbConn.BeginTransaction())
                {
                    try
                    {
                        var sql = $@"UPDATE XYZ_EMPLOYEE_WORK_HISTORY 
                          SET COMPANY_NAME = '{model.Company}', 
                               TITLE = '{model.Title}', 
                               SALARY = {model.Salary}, 
                               SKILLS = {skillsVal}, 
                               HOBBIES = {hobbiesVal},
                               STATUS = '{model.Status}', 
                               HIRE_DATE = {hireDateVal} 
                           WHERE ID = {model.WorkHistoryId}
                           AND EMPLOYEE_ID = {model.EmployeeId}";

                        dbConn.Execute(sql, null, trans);

                        if (!string.IsNullOrEmpty(model.FileName))
                        {
                            var fi = new FileInfo(model.FileName);
                            var fileType = Helpers.MimeTypeHelper.GetMimeType(model.SelectedDocumentationFileData, fi.FullName);

                            sql = $"DELETE FROM XYZ_EMPLOYEE_FILE WHERE EMPLOYEE_WORK_HISTORY_ID = {model.WorkHistoryId}";
                            dbConn.Execute(sql, null, trans);

                            var blobParameter = new OracleParameter
                            {
                                ParameterName = "SelectedDocumentationFileData",
                                OracleDbType = OracleDbType.Blob,
                                Direction = ParameterDirection.Input,
                                Value = model.SelectedDocumentationFileData
                            };

                            sql = $@"INSERT INTO XYZ_EMPLOYEE_FILE (EMPLOYEE_WORK_HISTORY_ID, FILE_NAME, FILE_TYPE, FILE_DATA)
                                    VALUES({model.WorkHistoryId}, '{fi.Name}', '{fileType}', :SelectedDocumentationFileData)";

                            dbConn.Execute(sql, model, trans);
                        }

                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        result.IsSucceed = false;
                        result.Message = e.Message;
                    }
                }
            }

            return result;
        }

        public byte[] GetFileData(string workHistoryId, out string fileType)
        {
            byte[] retVal = null;
            string fType = string.Empty;

            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = dbConn;
                    cmd.CommandText = $"SELECT FILE_DATA, FILE_TYPE FROM XYZ_EMPLOYEE_FILE WHERE EMPLOYEE_WORK_HISTORY_ID = {workHistoryId}";
                    cmd.CommandType = CommandType.Text;
                    dbConn.Open();

                    var dr = cmd.ExecuteReader();
                    if(dr.Read())
                    {
                        retVal = dr.GetOracleBlob(0).Value;
                        fType = dr.GetString(1);
                    }
                }
            }

            fileType = fType;
            return retVal; 
        }
    }

    [DatabaseSchema("Seraph")]
    [DatabaseTable("XYZ_EMPLOYEE")]
    public class XyzEmployee
    {
        public int Id { get; set; }

        public string Name { get; set; }
       
        public string Company { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        [DatabaseExclude]
        public bool IsModified { get; set; }

        [DatabaseExclude]
        public bool IsDeleted { get; set; }

        [DatabaseExclude]
        public List<XyzEmployeeWorkHistory> WorkHistory { get; set; }

        public void ReplaceNullsWithEmptyStrings()
        {
            foreach (var property in typeof(XyzEmployee).GetProperties())
            {
                if(property.PropertyType == typeof(string))
                {
                    var val = property.GetValue(this);
                    if(val == null)
                    {
                        property.SetValue(this, string.Empty);
                    }
                }
            }
        }
    }

    [DatabaseSchema("Seraph")]
    [DatabaseTable("XYZ_EMPLOYEE_WORK_HISTORY")]
    public class XyzEmployeeWorkHistory
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string CompanyName { get; set; }

        public string Title { get; set; }

        public DateTime HireDate { get; set; }

        public string Status { get; set; }

        public int DisplaySequence { get; set; }

        public decimal Salary { get; set; }

        public string Skills { get; set; }

        public string Hobbies { get; set; }

        [DatabaseExclude]
        public bool IsModified { get; set; }

        [DatabaseExclude]
        public bool IsDeleted { get; set; }
    }

    public class ManageXyzEmployeeModel
    {
        public List<XyzEmployee> Employees { get; set; }
    }

    public class XyzEmployeePagingModel
    {
        public IEnumerable<XyzEmployee> EmployeeList { get; set; }

        public int CurrentPage { get; set; }

        public int ItemCount { get; set; }

        public int TotalPages { get; set; }
    }

    public class Result
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }

    public class XyzEmployeeUpdateModel
    {
        public string EmployeeId { get; set; }
        public string WorkHistoryId { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Salary { get; set; }
        public string Skills { get; set; }
        public string Hobbies { get; set; }
        public string HireDate { get; set; }
        public byte[] SelectedDocumentationFileData { get; set; }
        public string FileName { get; set; }
    }
}