﻿using Dapper;
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
    }

    public class XyzEmployeePagingModel
    {
        public IEnumerable<XyzEmployee> EmployeeList { get; set; }

        public int CurrentPage { get; set; }

        public int ItemCount { get; set; }

        public int TotalPages { get; set; }
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