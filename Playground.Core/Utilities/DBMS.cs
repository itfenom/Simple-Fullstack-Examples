using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;

namespace Playground.Core.Utilities
{
    public class Dbms
    {
        private SqlConnection _mOConnection;
        private string _mSConnectionString;

        public Dbms()
        {
            _mOConnection = null;
        }

        public Dbms(string sConnectionString)
        {
            _mSConnectionString = sConnectionString;
            _mOConnection = null;
        }

        public void CloseConnection()
        {
            CloseConnection(_mOConnection);
        }

        public void CloseConnection(SqlConnection oConnection)
        {
            try
            {
                // ReSharper disable once UseNullPropagation
                if (oConnection != null)
                {
                    oConnection.Close();
                }
            }
            catch (SqlException exception1)
            {
                SqlException exception = exception1;
                throw new Exception(GetSqlErr(exception.Errors));
            }
        }

        public SqlConnection GetConnection()
        {
            SqlConnection oConnection = _mOConnection;
            if (IsConnectionOpen)
            {
                return oConnection;
            }
            // ReSharper disable once UseNullPropagation
            if (_mOConnection != null)
            {
                _mOConnection.Close();
            }
            _mOConnection = OpenConnection();
            return _mOConnection;
        }

        public static object GetDbValue(object oField, object oDefaultVal)
        {
            object objectValue = RuntimeHelpers.GetObjectValue(oDefaultVal);
            if (!Convert.IsDBNull(RuntimeHelpers.GetObjectValue(oField)))
            {
                objectValue = RuntimeHelpers.GetObjectValue(oField);
            }
            return objectValue;
        }

        public SqlConnection GetNewConnection()
        {
            return OpenConnection();
        }

        public static string GetSqlErr(SqlErrorCollection oErrors)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SQL Exception:{0}", "\r\n");
            int num2 = oErrors.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (i > 0)
                {
                    builder.Append("\r\n");
                }
                builder.AppendFormat("{0}-> {1}", "\t", oErrors[i].Message);
            }
            return builder.ToString();
        }

        private SqlConnection OpenConnection()
        {
            SqlConnection connection;
            try
            {
                connection = new SqlConnection(_mSConnectionString);
                connection.Open();
            }
            catch (SqlException exception1)
            {
                SqlException exception = exception1;
                throw new Exception(GetSqlErr(exception.Errors));
            }
            return connection;
        }

        public string ConnectionString
        {
            get => _mSConnectionString;
            set => _mSConnectionString = value;
        }

        public bool IsConnectionOpen => (_mOConnection != null) && (_mOConnection.State == ConnectionState.Open);
    }
}
