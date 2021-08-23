﻿using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Playground.Mvc.DAL
{
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

        //protected IEnumerable<T> Select<T>(string strQuery)
        //{
        //    using (var dbConn = new OracleConnection(GetConnectionString()))
        //    {
        //        dbConn.Open();

        //        var items = dbConn.Query<T>(strQuery);
        //        return items;
        //    }
        //}

        protected IEnumerable<T> Select<T>(string query, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool needToDisposeConnection = false;

            if (connection == null)
            {
                if (transaction != null)
                {
                    throw new ArgumentException("You must pass an OracleConnection if you want to use an OracleTransaction.");
                }

                connection = new OracleConnection(GetConnectionString());
                connection.Open();
                needToDisposeConnection = true;
            }

            IEnumerable<T> items = connection.Query<T>(query, null, transaction);

            if (needToDisposeConnection)
            {
                connection.Dispose();
            }

            return items;
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
}