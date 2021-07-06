using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

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


        public void UpdateWorkHistory()
        {

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

    #region Base Dapper Repository class
    public abstract class DapperBaseRepository
    {
        protected abstract string GetConnectionString();

        protected string GetPropertyName<T, U>(Expression<Func<T, U>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression) body = ((LambdaExpression)body).Body;

            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((PropertyInfo)((MemberExpression)body).Member).Name;

                case ExpressionType.Convert:
                    return ((PropertyInfo)((MemberExpression)((UnaryExpression)body).Operand).Member).Name;

                default:
                    throw new InvalidOperationException();
            }
        }

        protected string PropertyInPascalCase<T>(Expression<Func<T, object>> e)
        {
            var propertyName = GetPropertyName(e);
            return ConvertFromPascalCase(propertyName);
        }

        protected string ConvertFromPascalCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        protected string ConvertToCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        protected string GetInsertStatement<T>(T dbObj)
        {
            var schema = typeof(T)
                .GetCustomAttributes(typeof(DatabaseSchemaAttribute), false)
                .Cast<DatabaseSchemaAttribute>()
                .FirstOrDefault();

            var table = typeof(T)
                .GetCustomAttributes(typeof(DatabaseTableAttribute), false)
                .Cast<DatabaseTableAttribute>()
                .FirstOrDefault();

            string target = string.Empty;
            if (schema != null)
            {
                if (table != null)
                {
                    target = $"{schema.Schema}.{table.Table}";
                }
                else
                {
                    target = $"{schema.Schema}.{ConvertFromPascalCase(typeof(T).Name)}";
                }
            }
            else
            {
                if (table != null)
                {
                    target = $"{table.Table}";
                }
                else
                {
                    target = ConvertFromPascalCase(typeof(T).Name);
                }
            }


            string selectedColumns = $"INSERT INTO {target} (";
            var props = typeof(T).GetProperties().ToList();
            foreach (var prop in props)
            {
                if (null != prop.GetCustomAttribute<DatabaseExcludeAttribute>())
                {
                    continue;
                }

                var column = prop.GetCustomAttribute<DatabaseColumnAttribute>();
                if (column == null && !prop.Name.Contains("_"))
                {
                    selectedColumns = selectedColumns + ConvertFromPascalCase(prop.Name) + ",";
                }
                else if (column != null)
                {
                    selectedColumns = selectedColumns + column.ColumnName + ",";
                }
            }

            selectedColumns = selectedColumns.TrimEnd(',') + ") VALUES (";

            foreach (var prop in props)
            {
                if (null != prop.GetCustomAttribute<DatabaseExcludeAttribute>())
                {
                    continue;
                }

                selectedColumns += $":{prop.Name},";
            }

            selectedColumns = selectedColumns.TrimEnd(',') + ")";
            return selectedColumns;
        }

        protected string SelectColumnList<T>(List<KeyValuePair<string, string>> searchPropertiesAndTerms)
        {
            var schema = typeof(T)
                .GetCustomAttributes(typeof(DatabaseSchemaAttribute), false)
                .Cast<DatabaseSchemaAttribute>()
                .FirstOrDefault();

            var table = typeof(T)
                .GetCustomAttributes(typeof(DatabaseTableAttribute), false)
                .Cast<DatabaseTableAttribute>()
                .FirstOrDefault();

            var search = string.Empty;
            if (true == searchPropertiesAndTerms?.Any())
            {
                foreach (var kvp in searchPropertiesAndTerms)
                {
                    var prop = typeof(T).GetProperty(kvp.Key);

                    var isString = prop.PropertyType == typeof(string);
                    if (isString)
                    {
                        if (search == string.Empty)
                        {
                            search += $" WHERE UPPER(inner_s_a.{kvp.Key}) LIKE '%{kvp.Value?.ToUpper()}%'";
                        }
                        else
                        {
                            search += $" AND UPPER(inner_s_a.{kvp.Key}) LIKE '%{kvp.Value?.ToUpper()}%'";
                        }
                    }
                    else
                    {
                        if (search == string.Empty)
                        {
                            search += $" WHERE inner_s_a.{kvp.Key} = {kvp.Value}";
                        }
                        else
                        {
                            search += $" AND inner_s_a.{kvp.Key} = {kvp.Value}";
                        }
                    }
                }
            }

            string selectedColumns = "SELECT inner_s_a.* FROM (Select ";
            foreach (var prop in typeof(T).GetProperties())
            {
                if (null != prop.GetCustomAttribute<DatabaseExcludeAttribute>())
                {
                    continue;
                }

                var column = prop.GetCustomAttribute<DatabaseColumnAttribute>();
                if (column == null && !prop.Name.Contains("_"))
                {
                    selectedColumns = selectedColumns + ConvertFromPascalCase(prop.Name) + " AS " + prop.Name + ",";
                }
                else if (column != null)
                {
                    selectedColumns = selectedColumns + column.ColumnName + " AS " + prop.Name + ",";
                }
            }

            selectedColumns = selectedColumns.TrimEnd(',');

            if (schema != null)
            {
                if (table != null)
                {
                    return selectedColumns + $" From {schema.Schema}.{table.Table}) inner_s_a {search}";
                }

                return selectedColumns + $" From {schema.Schema}.{ConvertFromPascalCase(typeof(T).Name)}) inner_s_a {search}";
            }
            else
            {
                if (table != null)
                {
                    return selectedColumns + $" From {table.Table}) inner_s_a {search}";
                }

                return selectedColumns + " From " + ConvertFromPascalCase(typeof(T).Name) + ") inner_s_a " + search;
            }
        }

        protected string SelectCount<T>()
        {
            var schema = typeof(T)
                .GetCustomAttributes(typeof(DatabaseSchemaAttribute), false)
                .Cast<DatabaseSchemaAttribute>()
                .FirstOrDefault();

            var table = typeof(T)
                .GetCustomAttributes(typeof(DatabaseTableAttribute), false)
                .Cast<DatabaseTableAttribute>()
                .FirstOrDefault();

            string selectedColumns = "Select COUNT(*)";

            if (schema != null)
            {
                if (table != null)
                {
                    return selectedColumns + $" From {schema.Schema}.{table.Table}";
                }

                return selectedColumns + $" From {schema.Schema}.{ConvertFromPascalCase(typeof(T).Name)}";
            }
            else
            {
                if (table != null)
                {
                    return selectedColumns + $" From {table.Table}";
                }

                return selectedColumns + " From " + ConvertFromPascalCase(typeof(T).Name);
            }
        }

        protected string SelectDistinctColumnValues<T>(string columnName)
        {
            var schema = typeof(T)
                .GetCustomAttributes(typeof(DatabaseSchemaAttribute), false)
                .Cast<DatabaseSchemaAttribute>()
                .FirstOrDefault();

            var table = typeof(T)
                .GetCustomAttributes(typeof(DatabaseTableAttribute), false)
                .Cast<DatabaseTableAttribute>()
                .FirstOrDefault();

            string selectedColumns = $"Select DISTINCT {ConvertFromPascalCase(columnName)}";
            string where = $" Where {ConvertFromPascalCase(columnName)} is not null";

            if (schema != null)
            {
                if (table != null)
                {
                    return selectedColumns + $" From {schema.Schema}.{table.Table} {where}";
                }

                return selectedColumns + $" From {schema.Schema}.{ConvertFromPascalCase(typeof(T).Name)} {where}";
            }
            else
            {
                if (table != null)
                {
                    return selectedColumns + $" From {table.Table} {where}";
                }

                return selectedColumns + " From " + ConvertFromPascalCase(typeof(T).Name) + where;
            }
        }

        protected IEnumerable<T> Select<T>(string strQuery)
        {
            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();

                var items = dbConn.Query<T>(strQuery);
                return items;
            }
        }

        protected int Insert<T>(T dbObj)
        {
            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();

                var strInsert = GetInsertStatement(dbObj);

                int result = dbConn.Execute(strInsert, dbObj);
                return result;
            }
        }

        protected int Update(string sql)
        {
            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();

                int result = dbConn.Execute(sql);
                return result;
            }
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(GetConnectionString());
        }

        public DataSet GetDataSet(string sql)
        {
            using (var dbConn = new OracleConnection(GetConnectionString()))
            {
                dbConn.Open();
                using (var reader = dbConn.ExecuteReader(sql))
                {
                    return ConvertDataReaderToDataSet(reader);
                }
            }
        }

        public DataSet ConvertDataReaderToDataSet(IDataReader data)
        {
            DataSet ds = new DataSet();
            int i = 0;
            while (!data.IsClosed)
            {
                ds.Tables.Add("Table" + (i + 1));
                ds.EnforceConstraints = false;
                ds.Tables[i].Load(data);
                i++;
            }
            return ds;
        }
    }

    public class DatabaseSchemaAttribute : Attribute
    {
        public string Schema { get; }

        public DatabaseSchemaAttribute(string schema)
        {
            Schema = schema;
        }
    }

    public class DatabaseTableAttribute : Attribute
    {
        public string Table { get; }

        public DatabaseTableAttribute(string table)
        {
            Table = table;
        }
    }

    public class DatabaseColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public DatabaseColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    public class DatabaseExcludeAttribute : Attribute
    {

    }
    #endregion
}