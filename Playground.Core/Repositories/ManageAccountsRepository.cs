using System;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Playground.Core.AdoNet;

namespace Playground.Core.Repositories
{
    public class ManageAccountsRepository
    {
        public static int InsertNewCategory(string categoryName)
        {
            var retVal = -999;
            var categoryNameParam = DAL.CreateOracleParameter("P_CATEGORY_NAME", ParameterDirection.Input, OracleDbType.Varchar2, categoryName.Length, categoryName);
            var categoryIdParam = DAL.CreateOracleParameter("P_CATEGORY_ID", ParameterDirection.Output, OracleDbType.Decimal, null);
            var output = DAL.Seraph.ExecuteStoredProcedureWithOutParameters("PKG_ACCT_MGR.PROC_INSERT_CATEGORY", categoryNameParam, categoryIdParam);

            if (output.Any())
            {
                return Convert.ToInt32(output["P_CATEGORY_ID"]);
            }

            return retVal;
        }

        public static void DeleteCategory(int categoryId)
        {
            var categoryIdParam = DAL.CreateOracleParameter("P_CATEGORY_ID", ParameterDirection.Input, OracleDbType.Decimal, categoryId);
            DAL.Seraph.ExecuteStoredProcedureNonQuery("PKG_ACCT_MGR.PROC_DELETE_CATEGORY", categoryIdParam);
        }

        public static void UpdateCategory(string categoryName, int categoryId)
        {
            var categoryNameParam = DAL.CreateOracleParameter("P_CATEGORY_NAME", ParameterDirection.Input, OracleDbType.Varchar2, categoryName.Length, categoryName);
            var categoryIdParam = DAL.CreateOracleParameter("P_CATEGORY_ID", ParameterDirection.Input, OracleDbType.Decimal, categoryId);
            DAL.Seraph.ExecuteStoredProcedureWithOutParameters("PKG_ACCT_MGR.PROC_UPDATE_CATEGORY", categoryNameParam, categoryIdParam);
        }

        public static int InsertNewAccount(string name, string login, string pass, string notes, int categoryId)
        {
            var retVal = -999;

            var acctNameParam = DAL.CreateOracleParameter("P_ACCT_NAME", ParameterDirection.Input, OracleDbType.Varchar2, name.Length, name);
            var acctLoginParam = DAL.CreateOracleParameter("P_ACCT_LOGIN", ParameterDirection.Input, OracleDbType.Varchar2, login.Length, login);
            var acctPassParam = DAL.CreateOracleParameter("P_ACCT_PASS", ParameterDirection.Input, OracleDbType.Varchar2, pass.Length, pass);
            var notesParam = DAL.CreateOracleParameter("P_ACCT_NOTES", ParameterDirection.Input, OracleDbType.Varchar2, notes.Length, notes);
            var categoryIdParam = DAL.CreateOracleParameter("P_CATEGORY_ID", ParameterDirection.Input, OracleDbType.Decimal, categoryId);
            var outParam = DAL.CreateOracleParameter("OUT_ID", ParameterDirection.Output, OracleDbType.Decimal, null);

            var output = DAL.Seraph.ExecuteStoredProcedureWithOutParameters("PKG_ACCT_MGR.PROC_INSERT_ACCT", 
                                                            acctNameParam, acctLoginParam, acctPassParam, notesParam, categoryIdParam, outParam);

            if (output.Any())
            {
                return Convert.ToInt32(output["OUT_ID"]);
            }

            return retVal;
        }

        public static void UpdateExistingAccount(int id, string acctName, string loginId, string pass, string notes, int categoryId)
        {
            var sql = "UPDATE ACCT_MGR " + 
                      "SET ACCT_NAME = '" + acctName + "', ACCT_LOGIN_ID = '" + loginId + "', ACCT_PASSWORD = '" + pass 
                        + "', ACCT_NOTES = '" + notes + "', DATE_MODIFIED = SYSTIMESTAMP, CATEGORY_ID = " + categoryId
                        + " WHERE ID = " + id;
            DAL.Seraph.ExecuteNonQuery(sql);
        }

        public static void DeleteAccount(int accountId)
        {
            var acctIdParam = DAL.CreateOracleParameter("P_ACCOUNT_ID", ParameterDirection.Input, OracleDbType.Decimal, accountId);
            DAL.Seraph.ExecuteStoredProcedureNonQuery("PKG_ACCT_MGR.PROC_DELETE_ACCOUNT", acctIdParam);
        }
    }
}
