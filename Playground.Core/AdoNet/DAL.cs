using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Playground.Core.AdoNet
{
    // ReSharper disable once InconsistentNaming
    public class DAL
    {
        private Dictionary<Thread, OracleConnection> _connectionDictionary;

        private static Dictionary<Thread, bool> _maintainConnections;

        private readonly string _developmentConnectionString;

        private readonly string _productionConnectionString;

        private static int _connectionsConfused;

        private DAL(string name, string developmentConnectionString, string productionConnectionString)
        {
            _developmentConnectionString = developmentConnectionString;
            _productionConnectionString = productionConnectionString;
            _connectionDictionary = new Dictionary<Thread, OracleConnection>();
            _maintainConnections = new Dictionary<Thread, bool>();

            DalList.Add(name, this);
        }

        public static string ConnectionString(string database, string userId, string password)
        {
            return "Data Source=" + CustomTnsParser.Instance.GetDatabasesDescription(database) + ";user id=" + userId + ";password=" + password + "; Pooling=true; Connection timeout=60;";
        }

        public static Dictionary<string, DAL> DalList = new Dictionary<string, DAL>();

        public static DAL Seraph = new DAL("Seraph", ConnectionString("XE", "seraph", "password"), ConnectionString("XE", "seraph", "password"));
        public static DAL HR = new DAL("HR", ConnectionString("XE", "HR", "password"), ConnectionString("XE", "HR", "password"));


        public static bool MaintainConnections
        {
            get
            {
                if (!_maintainConnections.Keys.Contains(Thread.CurrentThread))
                {
                    _maintainConnections[Thread.CurrentThread] = false;
                }

                return _maintainConnections[Thread.CurrentThread];
            }
            set
            {
                if (!value && _maintainConnections.ContainsKey(Thread.CurrentThread) && _maintainConnections[Thread.CurrentThread])
                {
                    foreach (DAL dal in DalList.Values)
                    {
                        dal.Close();
                    }
                }

                _maintainConnections[Thread.CurrentThread] = value;
            }
        }

        private void Close()
        {
            OracleConnection connection = GetConnection();
            connection.Close();
            _connectionDictionary.Remove(Thread.CurrentThread);
        }

        private OracleConnection GetConnection()
        {
            if (MaintainConnections)
            {
                OracleConnection connection;

                try
                {
                    connection = _connectionDictionary[Thread.CurrentThread];
                    if (connection == null)
                    {
                        connection = _connectionDictionary[Thread.CurrentThread] = new OracleConnection(GetConnectionString());
                    }

                    string userId = GetUserIdFromConnectionString(connection.ConnectionString);
                    string correctUserId = GetUserIdFromConnectionString(GetConnectionString());
                    if (!userId.Equals(correctUserId))
                    {
                        if (_connectionsConfused < 3)
                        {
                            _connectionsConfused++;
                        }

                        connection.Close();
                        _connectionDictionary.Remove(Thread.CurrentThread);
                        connection = _connectionDictionary[Thread.CurrentThread] = new OracleConnection(GetConnectionString());
                    }
                }
                catch (KeyNotFoundException)
                {
                    connection = _connectionDictionary[Thread.CurrentThread] = new OracleConnection(GetConnectionString());
                }

                return connection;
            }
            else
            {
                return null; //don't use this method if maintainConnections is turned off!
            }
        }

        private OracleConnection Open()
        {
            OracleConnection connection = GetConnection();
            switch (connection.State)
            {
                case ConnectionState.Broken:
                    connection.Close();
                    connection.Open();
                    break;

                case ConnectionState.Closed:
                    connection.Open();
                    break;
            }
            return connection;
        }

        public static void StopMaintainingConnectionsInAllThreads()
        {
            foreach (DAL dal in DalList.Values)
            {
                lock (dal)
                {
                    foreach (OracleConnection connection in dal._connectionDictionary.Values)
                    {
                        connection.Close();
                    }
                    dal._connectionDictionary.Clear();
                }
            }
            lock (_maintainConnections)
            {
                _maintainConnections.Clear();
            }
        }

        private string GetConnectionString()
        {
            if (CoreConfig.UseDevDatabase())
            {
                return _developmentConnectionString;
            }

            return _productionConnectionString;
        }

        private static string GetUserIdFromConnectionString(string connectionString)
        {
            int userIdIndex = connectionString.IndexOf("user id", StringComparison.Ordinal);
            int equalsIndex = connectionString.IndexOf('=', userIdIndex);
            int semicolonIndex = connectionString.IndexOf(';', equalsIndex);
            equalsIndex++;
            return connectionString.Substring(equalsIndex, semicolonIndex - equalsIndex).Trim();
        }

        #region Oracle Parameters

        public static OracleParameter CreateOracleParameter(string name, OracleDbType type, object value)
        {
            return CreateOracleParameter(name, ParameterDirection.Input, type, value);
        }

        public static OracleParameter CreateOracleParameter(string name, ParameterDirection direction, OracleDbType type)
        {
            var parameter = new OracleParameter(name, type) { Direction = direction };
            return parameter;
        }

        public static OracleParameter CreateOracleParameter(string name, ParameterDirection direction, OracleDbType type, object value)
        {
            var parameter = new OracleParameter(name, type)
            {
                Value = value ?? DBNull.Value,
                Direction = direction
            };
            return parameter;
        }

        public static OracleParameter CreateOracleParameter(string name, ParameterDirection direction, OracleDbType type, int size, object value)
        {
            var parameter = new OracleParameter(name, type)
            {
                Size = size,
                Value = value ?? DBNull.Value,
                Direction = direction
            };
            return parameter;
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        /**********************************************************************************************************************************************
         *  Some notes on Creating Associative arrays Oracle Parameters:                                                                              *
         * 2 important things to note!!!                                                                                                              *
         *                                                                                                                                            *
         *  1) In case of Output or InputOutput direction, the Size has to be set                                                                     *
         *     to the maximum number of elements you expect to get from the stored procedure (see the CreateAssociativeArray method).                 *
         *     Specify this maximum number in the maxNumberOfElementsInArray.                                                                         *
         *     For example, you want to pass 3 elements to the stored procedure and get 10 back (InputOutput direction).                              *
         *    Set the maxNumberOfElementsInArray to 10, otherwise CreateAssociativeArray will set Size to 3.                                          *
         *                                                                                                                                            *
         *  2) The special processing of array of strings (see the CreateStringAssociativeArray method).                                              *
         *     String is variable-length element type (Varchar2 and so on). So, for strings we need to                                                *
         *     define the ArrayBindSize property. ArrayBindSize is the collection each element of which                                               *
         *     specifies the length of the corresponding element in the Value property. Being longer                                                  *
         *     than specified, the element will be truncated. Pass the maximum allowed length of elements                                             *
         *     in maxLength, otherwise it will be set to 255 (default) or the maximum length found in the Value property.                             *
         *                                                                                                                                            *
         **********************************************************************************************************************************************/

        public static OracleParameter CreateAssociativeArray<TValueType, TOracleType>(string name, List<TValueType> values, ParameterDirection direction, OracleDbType oracleDbType, TOracleType nullValue, int? maxNumberOfElementsInArray)
        {
            bool isArrayEmpty = values == null || values.Count == 0;
            OracleParameter array = new OracleParameter(name, oracleDbType, direction)
            {
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                // ReSharper disable once RedundantExplicitArraySize
                Value = !isArrayEmpty ? values.ToArray() : (object)new TOracleType[1] { nullValue },
                Size = !isArrayEmpty ? values.Count : 1
            };

            // if it's Output/InputOutput parameter, set the maximum possible number of elements.
            if (maxNumberOfElementsInArray != null &&
               (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput))
                array.Size = Math.Max(array.Size, maxNumberOfElementsInArray.Value);

            return array;
        }

        public static OracleParameter CreateInt32AssociativeArray(string name, List<int> values, ParameterDirection direction = ParameterDirection.Input, int? maxNumberOfElementsInArray = null)
        {
            return CreateAssociativeArray(name, values,
                     direction, OracleDbType.Int32, OracleDecimal.Null, maxNumberOfElementsInArray);
        }

        public static OracleParameter CreateDecimalAssociativeArray(string name, List<decimal> values, ParameterDirection direction = ParameterDirection.Input, int? maxNumberOfElementsInArray = null)
        {
            return CreateAssociativeArray(name, values, direction,
                     OracleDbType.Decimal, OracleDecimal.Null, maxNumberOfElementsInArray);
        }

        public static OracleParameter CreateStringAssociativeArray(string name, List<string> values, ParameterDirection direction = ParameterDirection.Input, int? maxNumberOfElementsInArray = null, int maxLength = 255)
        {
            var retVal = CreateAssociativeArray(name, values, direction,
                       OracleDbType.Varchar2, OracleString.Null, maxNumberOfElementsInArray);

            if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
            {
                var curMaxLen = maxLength;
                values?.ForEach(s => { if (curMaxLen < s.Length) curMaxLen = s.Length; });

                retVal.ArrayBindSize = new int[retVal.Size];
                for (var i = 0; i < retVal.Size; i++)
                    retVal.ArrayBindSize[i] = curMaxLen;
            }

            return retVal;
        }
        
        #endregion

        #region Private Methods
        private object ExecuteScalar(OracleConnection connection, string sql)
        {
            using (var cmd = new OracleCommand(sql, connection))
            {
                return cmd.ExecuteScalar();
            }
        }

        private DataTable ExecuteQuery(OracleConnection connection, string sql)
        {
            using (var da = new OracleDataAdapter(sql, connection))
            {
                var dt = new DataTable();
                try
                {
                    da.Fill(dt);
                }
                catch (InvalidCastException invalidCastException)
                {
                    Console.WriteLine(invalidCastException.Message);

                    dt = new DataTable();
                    using (var oCmd = new OracleCommand(sql, connection))
                    {
                        var dr = oCmd.ExecuteReader(CommandBehavior.CloseConnection);
                        var dtSchema = dr.GetSchemaTable();

                        var listCols = new List<DataColumn>();
                        if (dtSchema != null)
                        {
                            foreach (DataRow row in dtSchema.Rows)
                            {
                                string columnName = Convert.ToString(row["ColumnName"]);
                                var colType = (Type)row["DataType"];

                                if (colType == typeof(decimal))
                                {
                                    colType = typeof(float);
                                }

                                var column = new DataColumn(columnName, colType);
                                listCols.Add(column);
                                dt.Columns.Add(column);
                            }

                            //Read from dataReader
                            while (dr.Read())
                            {
                                var dataRow = dt.NewRow();
                                for (int i = 0; i < listCols.Count; i++)
                                {
                                    if (dr.GetFieldType(i)?.FullName == "System.Decimal")
                                    {
                                        var val = dr.GetOracleDecimal(i).ToString();
                                        dataRow[listCols[i]] = val;
                                    }
                                    else
                                    {
                                        dataRow[listCols[i]] = dr[i];
                                    }
                                }
                                dt.Rows.Add(dataRow);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                return dt;
            }
        }

        private int ExecuteNonQuery(OracleConnection connection, string sql)
        {
            using (var cmd = new OracleCommand(sql, connection))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        private int ExecuteNonQueryWithParameters(OracleConnection connection, string sql, params OracleParameter[] parameters)
        {
            using (var cmd = new OracleCommand(sql, connection))
            {
                cmd.Parameters.AddRange(parameters);
                cmd.BindByName = true;
                return cmd.ExecuteNonQuery();
            }
        }

        private byte[] ExecuteNonQueryWithBlobOutputParameter(OracleConnection connection, string sql, string blobParameterName)
        {
            using (var cmd = new OracleCommand(sql, connection))
            {
                cmd.Parameters.Add(CreateOracleParameter(blobParameterName, ParameterDirection.Output, OracleDbType.Blob));
                cmd.BindByName = true;
                cmd.ExecuteNonQuery();

                using (var lob = (OracleBlob)cmd.Parameters[0].Value)
                {
                    return lob.Value;
                }
            }
        }

        private DataTable ExecuteStoredProcedureQuery(OracleConnection connection, string procedureName, params OracleParameter[] parameters)
        {
            using (var cmd = new OracleCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.AddRange(parameters);

                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private object ExecuteStoredProcedureNonQuery(OracleConnection connection, string procedureName, params OracleParameter[] parameters)
        {
            using (var cmd = new OracleCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                cmd.BindByName = true;
                cmd.ExecuteNonQuery();

                // Rather than return the result of ExecuteNonQuery, which will always be -1 for stored
                // procedures, let's return the value of the ReturnValue parameter if there is one:
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.ReturnValue)
                    {
                        return parameters[i].Value;
                    }
                }
                return null;
            }
        }

        private OracleBlob GetBlob(OracleConnection oConn, string sql)
        {
            OracleBlob blob = null;

            try
            {
                using (var oCmd = new OracleCommand())
                {
                    oCmd.Connection = oConn;
                    oCmd.CommandType = CommandType.Text;
                    oCmd.CommandText = sql;
                    oCmd.CommandTimeout = 100 * 60;

                    using (var oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            blob = oReader.GetOracleBlob(0);
                        }
                    }
                }
            }
            catch (OracleException oEx)
            {
                throw (new Exception(oEx.ToString()));
            }

            return blob;
        }

        private bool InsertOracleBlob(OracleConnection oConn, string sql, byte[] fileData)
        {
            try
            {
                using (var oCmd = new OracleCommand())
                {
                    oCmd.Connection = oConn;
                    oCmd.CommandType = CommandType.Text;
                    oCmd.CommandText = sql;

                    oCmd.Parameters.Add("BLOB_PARAM", OracleDbType.Blob).Value = fileData;
                    oCmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception oEx)
            {
                throw (new Exception(oEx.ToString()));
            }
        }
        #endregion

        #region Public Methods
        public OracleBlob GetBlob(string sql)
        {
            using (var oConn = new OracleConnection(GetConnectionString()))
            {
                oConn.Open();
                return GetBlob(oConn, sql);
            }
        }

        public static DataTable GetAssignmentTable(string variableId, string assignmentId)
        {
            string strSql = "select a.variable_id, b.assignment_id, c.lot, d.product, e.state, f.st_rt_san, g.mfd_key "
            + " from bawudv_variable a, bawudv_assignment b, bawudv_grp_lot c, bawudv_grp_prod d, bawudv_grp_state e, "
            + " bawudv_grp_rt_op f, bawudv_grp_func g where b.assignment_id <> " + assignmentId
            + " and a.variable_id = " + variableId + " and b.variable_id = a.variable_id and c.assignment_id = b.assignment_id "
            + " and d.assignment_id = b.assignment_id  and e.assignment_id = b.assignment_id and f.assignment_id = b.assignment_id "
            + " and g.assignment_id = b.assignment_id ";

            return Seraph.ExecuteQuery(strSql);
        }

        public void GetLowestRankInContext(List<string> prodList, List<string> lotList, List<string> funcList, ref int outLowestRank, ref string outMessage)
        {
            var userIdParam = CreateOracleParameter("P_USER_ID_IN", ParameterDirection.Input, OracleDbType.Int32, Convert.ToInt32(UserInfo.ActiveUser.UserId), null);
            var userDefaultRankParam = CreateOracleParameter("P_DEFAULT_RANK", ParameterDirection.Input, OracleDbType.Int32, 100, null);
            var lowestRankParam = CreateOracleParameter("P_LOWEST_RANK_OUT", ParameterDirection.Output, OracleDbType.Int32);
            var outMsgParam = CreateOracleParameter("P_STATUS_OUT", ParameterDirection.Output, OracleDbType.Varchar2, 100, 255);
            var prodListParam = CreateStringAssociativeArray("P_PROD_LIST_IN", prodList);
            var lotListParam = CreateStringAssociativeArray("P_LOT_LIST_IN", lotList);
            var funcListParam = CreateStringAssociativeArray("P_FUNC_LIST_IN", funcList);

            var outParamVal = Seraph.ExecuteStoredProcedureWithOutParameters("BAW_LOWEST_RANK_PKG.GET_LOWEST_RANK_IN_CONTEXT", userIdParam, prodListParam, lotListParam, funcListParam, userDefaultRankParam, lowestRankParam, outMsgParam);

            if (!string.IsNullOrEmpty(outParamVal["P_LOWEST_RANK_OUT"]) && (!string.IsNullOrEmpty(outParamVal["P_STATUS_OUT"])))
            {
                outLowestRank = Convert.ToInt32(outParamVal["P_LOWEST_RANK_OUT"]);
                outMessage = outParamVal["P_STATUS_OUT"];
            }
        }

        public int GeneralTwoStepTransaction(string strSql1, string strSql2)
        {
            int stat;

            lock (this)
            {
                using (var connection = new OracleConnection(GetConnectionString()))
                {
                    OracleTransaction transaction = null;
                    var cmd = connection.CreateCommand();
                    try
                    {
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        transaction = connection.BeginTransaction();
                        cmd.Transaction = transaction;
                        cmd.CommandText = strSql1;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = strSql2;
                        stat = cmd.ExecuteNonQuery();

                        transaction.Commit();
                        Console.WriteLine("Delete and insert succeeded");
                    }
                    catch (OracleException oex2)
                    {
                        stat = -99; // error occurred
                        transaction?.Rollback();
                        Console.WriteLine(oex2.Message);
                        Console.WriteLine("Transaction Failed... rolled-back");
                    }
                }
            }
            return stat;
        }

        public DataTable GetFlowFunctionEditableProducts(int userId, int usersDefaultRank, int groupId, bool useAllProductsFlag)
        {
            var dt = new DataTable();
            var outputParameters = new Dictionary<string, string[]>();

            var userIdParam = CreateOracleParameter("IN_USER_ID_IN", ParameterDirection.Input, OracleDbType.Decimal, userId);
            var usersDefaultRankParam = CreateOracleParameter("IN_DEFAULT_RANK", ParameterDirection.Input, OracleDbType.Decimal, usersDefaultRank);
            var groupIdParam = CreateOracleParameter("IN_GROUP_ID", ParameterDirection.Input, OracleDbType.Decimal, groupId);
            var useAllProductsFlagParam = CreateOracleParameter("IN_USE_ALL_PRODUCTS", ParameterDirection.Input, OracleDbType.Decimal, (useAllProductsFlag ? 1 : 0));
            var outCheckParam = CreateStringAssociativeArray("OUT_CHECK", null, ParameterDirection.Output, 1000);
            var outProductParam = CreateStringAssociativeArray("OUT_PRODUCT", null, ParameterDirection.Output, 1000);
            var outCanEditParam = CreateStringAssociativeArray("OUT_CAN_EDIT", null, ParameterDirection.Output, 1000);

            lock (this)
            {
                using (var connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "BAW_LOWEST_RANK_PKG.GET_EDITABLE_PRODUCTS";

                        cmd.Parameters.Add(userIdParam);
                        cmd.Parameters.Add(usersDefaultRankParam);
                        cmd.Parameters.Add(groupIdParam);
                        cmd.Parameters.Add(useAllProductsFlagParam);
                        cmd.Parameters.Add(outCheckParam);
                        cmd.Parameters.Add(outProductParam);
                        cmd.Parameters.Add(outCanEditParam);

                        cmd.BindByName = true;

                        try
                        {
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < cmd.Parameters.Count; i++)
                            {
                                if (cmd.Parameters[i].Direction == ParameterDirection.Output)
                                {
                                    outputParameters.Add(cmd.Parameters[i].ParameterName, ConvertToStringArray(cmd.Parameters[i].Value as Array));
                                }
                            }

                            if (outputParameters.Count > 0)
                            {
                                dt.Columns.Add(new DataColumn("CHECK", typeof(bool)));
                                dt.Columns.Add(new DataColumn("PRODUCT", typeof(string)));
                                dt.Columns.Add(new DataColumn("CAN_EDIT", typeof(bool)));

                                string[] checks = outputParameters["OUT_CHECK"];
                                string[] products = outputParameters["OUT_PRODUCT"];
                                string[] canEdit = outputParameters["OUT_CAN_EDIT"];
                                int counter = checks.Length;

                                for (int i = 0; i < counter; i++)
                                {
                                    var dataRow = dt.NewRow();
                                    dataRow["CHECK"] = (Convert.ToInt32(checks[i]) != 0);
                                    dataRow["PRODUCT"] = products[i];
                                    dataRow["CAN_EDIT"] = (Convert.ToInt32(canEdit[i]) != 0);
                                    dt.Rows.Add(dataRow);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public object ExecuteScalar(string sql)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteScalar(Open(), sql);
                }
                else
                {
                    using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                    {
                        connection.Open();
                        return ExecuteScalar(connection, sql);
                    }
                }
            }
        }

        public DataTable ExecuteQuery(string sql)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteQuery(Open(), sql);
                }
                else
                {
                    using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                    {
                        connection.Open();
                        return ExecuteQuery(connection, sql);
                    }
                }
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteNonQuery(Open(), sql);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteNonQuery(connection, sql);
                }
            }
        }

        public int ExecuteNonQueryWithParameters(string sql, params OracleParameter[] parameters)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteNonQueryWithParameters(Open(), sql, parameters);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteNonQueryWithParameters(connection, sql, parameters);
                }
            }
        }

        public byte[] ExecuteNonQueryWithBlobOutputParameter(string sql, string blobParameterName)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteNonQueryWithBlobOutputParameter(Open(), sql, blobParameterName);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteNonQueryWithBlobOutputParameter(connection, sql, blobParameterName);
                }
            }
        }

        public DataTable ExecuteStoredProcedureQuery(string procedureName, params OracleParameter[] parameters)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteStoredProcedureQuery(Open(), procedureName, parameters);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteStoredProcedureQuery(connection, procedureName, parameters);
                }
            }
        }

        public object ExecuteStoredProcedureNonQuery(string procedureName, params OracleParameter[] parameters)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteStoredProcedureNonQuery(Open(), procedureName, parameters);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteStoredProcedureNonQuery(connection, procedureName, parameters);
                }
            }
        }

        public Dictionary<string, string> ExecuteStoredProcedureWithOutParameters(string procedureName, params OracleParameter[] parameters)
        {
            lock (this)
            {
                if (MaintainConnections)
                {
                    return ExecuteStoredProcedureWithOutParameters(Open(), procedureName, parameters);
                }

                using (OracleConnection connection = new OracleConnection(GetConnectionString()))
                {
                    connection.Open();
                    return ExecuteStoredProcedureWithOutParameters(connection, procedureName, parameters);
                }
            }
        }

        private Dictionary<string, string> ExecuteStoredProcedureWithOutParameters(OracleConnection connection, string procedureName, params OracleParameter[] parameters)
        {
            using (var cmd = new OracleCommand())
            {
                //for (int i = 0; i < parameters.Length; i++)
                //{
                //    var quote = (parameters[i].OracleDbType == OracleDbType.Varchar2 ? "'" : "");
                //    var ending = (i == parameters.Length - 1 ? ")" : ",");
                //}

                cmd.Connection = connection;
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                cmd.BindByName = true;
                cmd.ExecuteNonQuery();

                var dictionary = new Dictionary<string, string>();

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.Output)
                    {
                        dictionary.Add(parameters[i].ParameterName, parameters[i].Value.ToString());
                    }
                }

                return dictionary;
            }
        }

        public Dictionary<string, string[]> ExecuteStoredProcAssociativeArraysOutput(string procName, params OracleParameter[] parameters)
        {
            Dictionary<string, string[]> retVal = new Dictionary<string, string[]>();
            lock (this)
            {
                if (MaintainConnections)
                {
                    using (var oCmd = new OracleCommand())
                    {
                        oCmd.Connection = Open();
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.CommandText = procName;

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            oCmd.Parameters.Add(parameters[i]);
                        }

                        oCmd.BindByName = true;
                        oCmd.ExecuteNonQuery();

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].Direction == ParameterDirection.Output && parameters[i].Value != DBNull.Value)
                            {
                                retVal.Add(parameters[i].ParameterName, ConvertToStringArray(parameters[i].Value as Array));
                            }
                        }
                    }
                }
                else
                {
                    using (var oConn = new OracleConnection(GetConnectionString()))
                    {
                        oConn.Open();
                        using (var oCmd = new OracleCommand())
                        {
                            oCmd.Connection = oConn;
                            oCmd.CommandType = CommandType.StoredProcedure;
                            oCmd.CommandText = procName;

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                oCmd.Parameters.Add(parameters[i]);
                            }

                            oCmd.BindByName = true;
                            oCmd.ExecuteNonQuery();

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].Direction == ParameterDirection.Output && parameters[i].Value != null)
                                {
                                    retVal.Add(parameters[i].ParameterName, ConvertToStringArray(parameters[i].Value as Array));
                                }
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] ConvertToStringArray(Array values)
        {
            if (values == null)
            {
                return new[] { "null" };
            }

            // create a new string array
            string[] stringArray = new string[values.Length];

            // loop through  System.Array and populate the 1-D String Array
            for (int i = 0; i < values.Length; i++)
            {
                stringArray[i] = values.GetValue(i).ToString();
            }

            return stringArray;
        }

        public bool InsertOracleBlob(string sql, byte[] fileData)
        {
            using (var oConn = new OracleConnection(GetConnectionString()))
            {
                oConn.Open();
                return InsertOracleBlob(oConn, sql, fileData);
            }
        }
        #endregion

        public DataTable RetrieveAppUsersAsAssociativeArrays()
        {
            var dt = new DataTable();
            var namesParam = DAL.CreateStringAssociativeArray("P_NAMES_OUT", null, ParameterDirection.Output, 50);
            var idParam = DAL.CreateInt32AssociativeArray("P_IDS_OUT", null, ParameterDirection.Output, 25);

            var oracleAssociativeArrays = DAL.Seraph.ExecuteStoredProcAssociativeArraysOutput("PKG_APP_USERS.PROC_GET_ASSOCIATIVE_ARRAYS", namesParam, idParam);

            if (oracleAssociativeArrays.Count == 0) return dt;

            string[] ids = oracleAssociativeArrays["P_IDS_OUT"];
            string[] names = oracleAssociativeArrays["P_NAMES_OUT"];
            int counter = 0;

            //Add Columns to the DataTable
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("LOGIN_NAME", typeof(string));

            //Add id rows.
            for (int i = 0; i < ids.Length; i++)
            {
                var _dataRow = dt.NewRow();
                _dataRow["ID"] = Convert.ToInt32(ids[i]);
                dt.Rows.Add(_dataRow);
            }

            //Add names rows!
            foreach (DataRow row in dt.Rows)
            {
                row["LOGIN_NAME"] = names[counter];
                counter++;
            }

            dt.AcceptChanges();
            return dt;
        }

        public string PassAssociativeArraysToOracle(List<string> namesList, List<int> numberList)
        {
            var namesParam = CreateStringAssociativeArray("P_NAMES_IN", namesList, ParameterDirection.Input);
            var numberParam = CreateInt32AssociativeArray("P_NUM_IN", numberList, ParameterDirection.Input);

            var msgParam = new OracleParameter
            {
                ParameterName = "P_MSG_OUT",
                OracleDbType = OracleDbType.Varchar2,
                Direction = ParameterDirection.Output,
                Value = string.Empty,
                Size = 300
            };

            var procedureRetVal = DAL.Seraph.ExecuteStoredProcedureWithOutParameters("PKG_APP_USERS.PROC_PASS_ASSOCIATIVE_ARRAYS", namesParam, numberParam, msgParam);

            if (procedureRetVal.Count > 0)
            {
                return procedureRetVal["P_MSG_OUT"];
            }

            return "";
        }
    }
}
