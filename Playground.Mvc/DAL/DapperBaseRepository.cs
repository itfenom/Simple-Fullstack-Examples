using Dapper;
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
        private static HashSet<Type> mappedTypes = new HashSet<Type>();

        protected abstract string GetConnectionString();

        #region Object-Relational Mapping

        /// <summary>
        /// Gets the SCHEMA.TABLE_NAME that maps to a given type, for use in SQL queries.
        /// </summary>
        public static string GetSchemaAndTableName<T>()
        {
            //Get the schema and table attributes attached to the type, if they exist.
            DatabaseSchemaAttribute schemaAttribute = typeof(T).GetCustomAttribute<DatabaseSchemaAttribute>(false);
            DatabaseTableAttribute tableAttribute = typeof(T).GetCustomAttribute<DatabaseTableAttribute>(false);

            string tableName;
            if (tableAttribute != null)
            {
                //If there was a DatabaseTableAttribute attached to the type, use the explicitly specified table name.
                tableName = tableAttribute.TableName;
            }
            else
            {
                //Otherwise, the table name is implied by the name of the type/class.
                string typeName = typeof(T).Name;

                if (typeName.Contains('_'))
                {
                    //If the type name includes an underscore, assume it's already in snake_case.
                    tableName = typeName;
                }
                else
                {
                    //Otherwise, assume we need to convert the type name from PascalCase to snake_case.
                    tableName = ConvertPascalCaseToSnakeCase(typeName);
                }
            }

            string schemaAndTableName;
            if (schemaAttribute != null)
            {
                //If there was a DatabaseSchemaAttribute attached to the type, use the explicitly specified schema name.
                schemaAndTableName = $"{schemaAttribute.SchemaName}.{tableName}";
            }
            else
            {
                //Otherwise, don't specify the schema/owner for the table.
                schemaAndTableName = tableName;
            }

            return schemaAndTableName.ToLower();
        }

        /// <summary>
        /// Gets the column name that maps to a given property, for use in SQL queries.
        /// </summary>
        public static string GetColumnName(PropertyInfo property)
        {
            //Get the column attribute attached to the property, if it exists.
            DatabaseColumnAttribute columnAttribute = property.GetCustomAttribute<DatabaseColumnAttribute>();

            if (columnAttribute != null)
            {
                //If there was a DatabaseColumnAttribute attached to the property, use the explicitly specified column name.
                return columnAttribute.ColumnName.ToLower();
            }
            else
            {
                //Otherwise, the column name is implied by the name of the property.
                string propertyName = property.Name;

                if (propertyName.Contains('_'))
                {
                    //If the property name includes an underscore, assume it's already in snake_case.
                    return propertyName.ToLower();
                }
                else
                {
                    //Otherwise, assume we need to convert the property name from PascalCase to snake_case.
                    return ConvertPascalCaseToSnakeCase(propertyName);
                }
            }
        }

        /// <summary>
        /// Tries several different methods to map a column name to a property.
        /// </summary>
        public static PropertyInfo GetPropertyFromColumnName(Type type, string columnName)
        {
            PropertyInfo property;

            List<PropertyInfo> nonExcludedProperties = type.GetProperties().Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null).ToList();

            //First, try to find a property with a DatabaseColumnAttribute on it that matches the column name. 

            property = nonExcludedProperties.Where(p => columnName.Equals(p.GetCustomAttribute<DatabaseColumnAttribute>()?.ColumnName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (property != null) return property;

            //Second, try to find a property with a name that matches the column name.

            property = nonExcludedProperties.Where(p => columnName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (property != null) return property;

            //Third, try taking out the underscores and seeing if anything matches.

            if (columnName.Contains('_'))
            {
                string columnNameWithoutUnderscores = columnName.Replace("_", "");
                property = nonExcludedProperties.Where(p => columnNameWithoutUnderscores.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (property != null) return property;
            }

            //Finally, if none of that stuff worked... I guess we give up.

            return null;
        }

        /// <summary>
        /// Uses an expression to get a property of a type.  Possibly useful in the future for Linq-to-SQL stuff?
        /// </summary>
        public static PropertyInfo GetPropertyFromExpression<T, U>(Expression<Func<T, U>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            MemberExpression memberExpression;
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    memberExpression = (MemberExpression)body;
                    break;

                case ExpressionType.Convert:
                    memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return (PropertyInfo)memberExpression.Member;
        }

        /// <summary>
        /// Registers a class with Dapper.  Tells Dapper how to map between class properties and database columns.
        /// </summary>
        /// <remarks>
        /// This is what makes Dapper actually use our custom DatabaseColumnAttribute class.
        /// </remarks>
        public static void RegisterTypeMapping<T>()
        {
            Type type = typeof(T);
            bool isClassOrStruct = type.IsClass || !(type.IsPrimitive || type == typeof(decimal) || type == typeof(DateTime) || type.IsEnum);
            if (isClassOrStruct && !mappedTypes.Contains(type))
            {
                SqlMapper.SetTypeMap(type, new CustomPropertyTypeMap(type, GetPropertyFromColumnName));
                mappedTypes.Add(type);
            }
        }

        #endregion Object-Relational Mapping

        #region String Case Converters

        /// <summary>
        /// Converts the given string from PascalCase to snake_case.
        /// </summary>
        public static string ConvertPascalCaseToSnakeCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        /// <summary>
        /// Converts the given string from snake_case to PascalCase.
        /// </summary>
        public static string ConvertSnakeCaseToPascalCase(string str)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            bool previousWasUnderscore = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == '_')
                {
                    //Don't add underscores to the Pascal case string.

                    previousWasUnderscore = true;
                }
                else
                {
                    if (i == 0 || previousWasUnderscore)
                    {
                        //If it's the first character in the string, or it was preceded by an underscore, then capitalize it.
                        c = char.ToUpper(c);
                    }
                    else
                    {
                        //Otherwise, make it lowercase.
                        c = char.ToLower(c);
                    }
                    stringBuilder.Append(c);

                    previousWasUnderscore = false;
                }
            }

            return stringBuilder.ToString();
        }

        #endregion String Case Converters

        #region Data Type Converters

        public static bool ConvertYOrNStringToBool(string yOrNString)
        {
            if (string.Equals(yOrNString, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (string.Equals(yOrNString, "N", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool? ConvertYOrNStringToNullableBool(string yOrNString)
        {
            if (string.Equals(yOrNString, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (string.Equals(yOrNString, "N", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public static string ConvertBoolToYOrNString(bool nullableBool)
        {
            if (nullableBool == true)
            {
                return "Y";
            }
            else
            {
                return "N";
            }
        }

        public static string ConvertNullableBoolToYOrNString(bool? nullableBool)
        {
            if (nullableBool == true)
            {
                return "Y";
            }
            else if (nullableBool == false)
            {
                return "N";
            }
            else
            {
                return null;
            }
        }

        #endregion Data Type Converters

        #region SQL Query Builders

        /// <summary>
        /// Generates a SQL statement to INSERT the given object into whichever table its type maps to.
        /// </summary>
        public static string BuildInsertSql<T>(T obj)
        {
            IEnumerable<PropertyInfo> includedProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null);

            return $@"
                INSERT INTO {GetSchemaAndTableName<T>()}
                (
                    {string.Join($@",
                    ", includedProperties.Select(p => GetColumnName(p)).ToArray())}
                )
                VALUES
                (
                    {string.Join($@",
                    ", includedProperties.Select(p => $":{p.Name}").ToArray())}
                )";
        }

        /// <summary>
        /// Generates a SQL statement to UPDATE the database record corresponding to the given object.
        /// </summary>
        public static string BuildUpdateSql<T>(T obj)
        {
            IEnumerable<PropertyInfo> primaryKeyProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null && p.GetCustomAttribute<DatabasePrimaryKeyAttribute>() != null);
            IEnumerable<PropertyInfo> nonPrimaryKeyProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null && p.GetCustomAttribute<DatabasePrimaryKeyAttribute>() == null);

            if (!primaryKeyProperties.Any())
            {
                throw new ArgumentException($"The type {typeof(T).FullName} must have at least one (non-excluded) property with a {typeof(DatabasePrimaryKeyAttribute).Name} on it.");
            }
            if (!nonPrimaryKeyProperties.Any())
            {
                throw new ArgumentException($"The type {typeof(T).FullName} must have at least one (non-excluded) property without a {typeof(DatabasePrimaryKeyAttribute).Name} on it.");
            }

            return $@"
                UPDATE
                    {GetSchemaAndTableName<T>()}
                SET
                    {string.Join($@",
                    ", nonPrimaryKeyProperties.Select(p => $"{GetColumnName(p)} = :{p.Name}").ToArray())}
                WHERE
                    {string.Join($@"
                    AND ", primaryKeyProperties.Select(p => $"{GetColumnName(p)} = :{p.Name}").ToArray())}";
        }

        /// <summary>
        /// Generates a SQL statement to DELETE the database record corresponding to the given object.
        /// </summary>
        public static string BuildDeleteSql<T>(T obj)
        {
            IEnumerable<PropertyInfo> primaryKeyProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null && p.GetCustomAttribute<DatabasePrimaryKeyAttribute>() != null);

            if (!primaryKeyProperties.Any())
            {
                throw new ArgumentException($"The type {typeof(T).FullName} must have at least one (non-excluded) property with a {typeof(DatabasePrimaryKeyAttribute).Name} on it.");
            }

            return $@"
                DELETE FROM
                    {GetSchemaAndTableName<T>()}
                WHERE
                    {string.Join($@"
                    AND ", primaryKeyProperties.Select(p => $"{GetColumnName(p)} = :{p.Name}").ToArray())}";
        }

        public static string BuildSelectColumnListSql<T>(List<KeyValuePair<string, string>> searchPropertiesAndTerms)
        {
            string sql = $@"
                SELECT
                    inner_s_a.*
                FROM
                    (
                        SELECT
                            {string.Join($@",
                            ", typeof(T).GetProperties() //Write column names for each property in type T.
                                .Where(p => p.GetCustomAttribute<DatabaseExcludeAttribute>() == null)
                                .Select(p => $"{GetColumnName(p)} AS {p.Name}"))}
                        FROM
                            {GetSchemaAndTableName<T>()}
                    ) inner_s_a";

            if (searchPropertiesAndTerms != null && searchPropertiesAndTerms.Count > 0)
            {
                //If any search terms were supplied, write a WHERE clause with conditions for each one
                sql += $@"
                WHERE
                    {string.Join($@"
                    AND ", searchPropertiesAndTerms.Select(s => BuildCaseInsensitiveColumnFilter<T>("inner_s_a", s))
                    .Where(s => s != null).ToArray())}";
            }

            return sql;
        }

        public static string BuildCaseInsensitiveColumnFilter<T>(string tableName, KeyValuePair<string, string> filterTerm)
        {
            PropertyInfo property = typeof(T).GetProperty(filterTerm.Key);

            if (property == null) return null;

            if (property.PropertyType == typeof(string))
            {
                return $"UPPER({tableName}.{filterTerm.Key}) LIKE '%{filterTerm.Value?.ToUpper()}%'";
            }
            else
            {
                return $"{tableName}.{filterTerm.Key} = {filterTerm.Value}";
            }
        }

        /// <summary>
        /// Generates a SQL query to SELECT COUNT(*) FROM whichever table the given type maps to.
        /// </summary>
        public static string BuildSelectCountSql<T>()
        {
            return $"SELECT COUNT(*) FROM {GetSchemaAndTableName<T>()}";
        }

        public static string BuildSelectDistinctColumnValuesSql<T>(string columnName)
        {
            string columnNameSnakeCase = ConvertPascalCaseToSnakeCase(columnName);

            return $@"
                SELECT DISTINCT
                    {columnNameSnakeCase}
                FROM
                    {GetSchemaAndTableName<T>()}
                WHERE
                    {columnNameSnakeCase} IS NOT NULL";
        }

        #endregion SQL Query Builders

        #region Database Interactions

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(GetConnectionString());
        }

        /// <summary>
        /// Initializes an OracleConnection and OracleTransaction.  Intended for use at the beginning of methods where connection and transaction are optional parameters.
        /// </summary>
        /// <returns>
        /// True if the connection and transaction were initialized within this method.  False if the connection was already not null to begin with.
        /// </returns>
        public bool InitializeConnectionAndTransaction(ref OracleConnection connection, ref OracleTransaction transaction)
        {
            bool disposalNeeded;
            if (connection == null)
            {
                if (transaction != null) { throw new ArgumentException(@"The argument ""connection"" cannot be null if the argument ""transaction"" is not null."); }

                connection = CreateConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                disposalNeeded = true;
            }
            else
            {
                disposalNeeded = false;
            }
            return disposalNeeded;
        }

        /// <summary>
        /// Disposes of an OracleConnection and OracleTransaction.  Intended for use at the end of methods where connection and transaction are optional parameters.
        /// </summary>
        public void DisposeConnectionAndTransactionIfNeeded(bool disposalNeeded, OracleConnection connection, OracleTransaction transaction)
        {
            if (disposalNeeded)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Commit();
                    }
                    catch (InvalidOperationException)
                    {
                        //Ignore it.  It just means the transaction was rolled back already.
                    }
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query and returns a "dynamic" object for each row returned.  The property names will be the column names in call caps.
        /// </summary>
        public IEnumerable<dynamic> Select(string sql, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            IEnumerable<dynamic> result = connection.Query(sql, null, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Executes the query and returns an object of type T for each row returned.
        /// </summary>
        public IEnumerable<T> Select<T>(string sql, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            RegisterTypeMapping<T>();

            IEnumerable<T> result = connection.Query<T>(sql, null, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Builds an INSERT statement for the given object and executes it.  Returns the number of rows affected.
        /// </summary>
        public int Insert<T>(T obj, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            string insertSql = BuildInsertSql(obj);

            int result = connection.Execute(insertSql, obj, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Builds an UPDATE statement for the given object and executes it.  Returns the number of rows affected.
        /// </summary>
        public int Update<T>(T obj, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            string updateSql = BuildUpdateSql(obj);

            int result = connection.Execute(updateSql, obj, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Builds a DELETE statement for the given object and executes it.  Returns the number of rows affected.
        /// </summary>
        public int Delete<T>(T obj, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            string deleteSql = BuildDeleteSql(obj);

            int result = connection.Execute(deleteSql, obj, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Executes an SQL statement such as an INSERT, UPDATE, or DELETE, which does not return rows like a SELECT would.
        /// </summary>
        /// <returns>the number of rows affected</returns>
        public int ExecuteNonQuery(string sql, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            int result = connection.Execute(sql, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
        }

        /// <summary>
        /// Executes a query which expects to get back a single scalar value (no more than one row and one column)
        /// </summary>
        /// <returns>the result of the query</returns>
        public T ExecuteScalar<T>(string sql, OracleConnection connection = null, OracleTransaction transaction = null)
        {
            bool disposalNeeded = InitializeConnectionAndTransaction(ref connection, ref transaction);

            RegisterTypeMapping<T>();

            T result = connection.ExecuteScalar<T>(sql, null, transaction);

            DisposeConnectionAndTransactionIfNeeded(disposalNeeded, connection, transaction);

            return result;
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

        #endregion Database Interactions
    }

    public class DatabaseSchemaAttribute : Attribute
    {
        public string SchemaName { get; }

        public DatabaseSchemaAttribute(string schemaName)
        {
            SchemaName = schemaName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseTableAttribute : Attribute
    {
        public string TableName { get; }

        public DatabaseTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DatabaseColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public DatabaseColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseExcludeAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabasePrimaryKeyAttribute : Attribute
    {

    }
}