using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Playground.Core.AdoNet;

namespace Playground.Core.Repositories
{
    public class ManageUsersRepository
    {
        public static Dictionary<string, string> InsertNewUser(string loginName, string password, string dispName, string email, int roleId)
        {
            var loginNameParam = DAL.CreateOracleParameter("P_LOGIN_NAME", ParameterDirection.Input, OracleDbType.Varchar2, loginName.Length, loginName);
            var passwordParam = DAL.CreateOracleParameter("P_LOGIN_PASSWORD", ParameterDirection.Input, OracleDbType.Varchar2, password.Length, password);
            var displayNameParam = DAL.CreateOracleParameter("P_DISPLAY_NAME", ParameterDirection.Input, OracleDbType.Varchar2, dispName.Length, dispName);
            var emailParam = DAL.CreateOracleParameter("P_EMAIL", ParameterDirection.Input, OracleDbType.Varchar2, email.Length, email);
            var roleIdParam = DAL.CreateOracleParameter("P_ROLE_ID", ParameterDirection.Input, OracleDbType.Decimal, roleId);
            var outIdParam = DAL.CreateOracleParameter("P_ID_OUT", ParameterDirection.Output, OracleDbType.Decimal, null);

            return DAL.Seraph.ExecuteStoredProcedureWithOutParameters("PKG_APP_USERS.ADD_NEW_APP_USER", loginNameParam, passwordParam, displayNameParam, emailParam, roleIdParam, outIdParam);

        }
    }

}
