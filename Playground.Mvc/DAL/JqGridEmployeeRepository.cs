using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SortOrder = Playground.Mvc.Models.SortOrder;

namespace Playground.Mvc.DAL
{
    public interface IJqGridEmployeeRepository
    {
        string GetSqlServerConnectionString();

        IEnumerable<string> GetDistinctColumnValues(string columnName);

        Tuple<List<EmployeeModel>, int> GetDataForJqGrid(JqGridRequest request);
    }

    public class JqGridEmployeeRepository : IJqGridEmployeeRepository
    {
        public string GetSqlServerConnectionString()
        {
            return "Server=DESKTOP-05EK4V3;Database=Seraph;User Id=letmein;Password=letmein;";
        }

        public IEnumerable<string> GetDistinctColumnValues(string columnName)
        {
            var retVal = new List<string>();
            var sql = $"SELECT DISTINCT {columnName} FROM EMPLOYEE WHERE {columnName} IS NOT NULL";
            using (var connection = new SqlConnection(GetSqlServerConnectionString()))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        retVal.Add(rdr[columnName].ToString());
                    }
                }
            }

            retVal.Sort();

            return retVal;
        }

        public Tuple<List<EmployeeModel>, int> GetDataForJqGrid(JqGridRequest request)
        {
            var pageStart = request.PerPage * (request.Page - 1);
            var pageEnd = pageStart + request.PerPage;
            var sortOrder = string.Empty;
            if (!string.IsNullOrEmpty(request.SortByProperty))
            {
                sortOrder = request.SortOrder == SortOrder.Ascending ? $" ORDER BY {request.SortByProperty} ASC" : $" ORDER BY {request.SortByProperty} DESC";
            }

            var items = new List<EmployeeModel>();
            var count = 0;

            using (var connection = new SqlConnection(GetSqlServerConnectionString()))
            {
                var sql = $@"
                DECLARE @PageNumber AS INT, @RowspPage AS INT
                SET @PageNumber = {request.Page}
                SET @RowspPage = {request.PerPage}
                SELECT a.*
                FROM EMPLOYEE a
                {GetFilterWhereClause<EmployeeModel>(request.SearchPropertiesAndTerms)}
                {sortOrder}
                OFFSET ((@PageNumber - 1) * @RowspPage) ROWS
                FETCH NEXT @RowspPage ROWS ONLY";

                connection.Open();

                var cmd = new SqlCommand("SELECT COUNT(*) FROM EMPLOYEE", connection);
                count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();

                using (cmd = new SqlCommand(sql, connection))
                {
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        items.Add(new EmployeeModel
                        {
                            EMP_ID = Convert.ToInt32(rdr["EMP_ID"]),
                            EMP_NAME = rdr["EMP_NAME"].ToString(),
                            EMP_EMAIL = rdr["EMP_EMAIL"].ToString(),
                            EMP_PHONE = rdr["EMP_PHONE"].ToString(),
                            EMP_SALARY = rdr["EMP_SALARY"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["EMP_SALARY"]),
                            EMP_HIRE_DATE = rdr["EMP_HIRE_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["EMP_HIRE_DATE"]),
                            EMP_GENDER = rdr["EMP_GENDER"] == DBNull.Value ? null : rdr["EMP_GENDER"].ToString(),
                            EMP_IS_ACTIVE = rdr["EMP_IS_ACTIVE"] == DBNull.Value ? false : (bool)rdr["EMP_IS_ACTIVE"]
                        });
                    }
                }
            }

            return Tuple.Create(items, count);

            #region Paging in Oracle
            //    using (var dbConn = new OracleConnection(GetConnectionString()))
            //{
            //    dbConn.Open();
            //    var strQuery = $@"select * 
            //                      from 
            //                    ( select rownum rnum, a.*
            //                        from ({SelectColumnList<Employee>(request.SearchPropertiesAndTerms)}) a
            //                       where rownum <= {pageEnd} 
            //                    )
            //                    where rnum > {pageStart}
            //                    {sortOrder}";

            //    var strCountQuery = SelectCount<EmployeeSummaryModel>();

            //    var items = dbConn.Query<EmployeeSummaryModel>(strQuery);
            //    var count = dbConn.ExecuteScalar(strCountQuery);
            //    return Tuple.Create(items, Convert.ToInt32(count));
            #endregion
        }

        private string GetFilterWhereClause<T>(List<KeyValuePair<string, string>> searchPropertiesAndTerms)
        {
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
                            search += $" WHERE UPPER({kvp.Key}) LIKE '%{kvp.Value?.ToUpper()}%'";
                        }
                        else
                        {
                            search += $" AND UPPER({kvp.Key}) LIKE '%{kvp.Value?.ToUpper()}%'";
                        }
                    }
                    else
                    {
                        var isNullableBool = prop.PropertyType == typeof(Nullable<bool>);
                        if (isNullableBool)
                        {
                            if (search == string.Empty)
                            {
                                search += $" WHERE {kvp.Key} = '{kvp.Value}'";
                            }
                            else
                            {
                                search += $" AND {kvp.Key} = '{kvp.Value}'";
                            }
                        }
                        else
                        {
                            if (search == string.Empty)
                            {
                                search += $" WHERE {kvp.Key} = {kvp.Value}";
                            }
                            else
                            {
                                search += $" AND {kvp.Key} = {kvp.Value}";
                            }
                        }
                    }
                }
            }

            return search;
        }
    }
}