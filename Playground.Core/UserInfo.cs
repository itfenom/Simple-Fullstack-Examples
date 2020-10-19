using System;
using Playground.Core.AdoNet;

namespace Playground.Core
{
    public class UserInfo
    {
        public decimal UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public decimal RoleId { get; set; }
        public string Password { get; set; }

        public string RoleName => GetRoleName(RoleId);

        public static string GetRoleName(decimal roleId)
        {
            return DAL.Seraph.ExecuteScalar("SELECT ROLE_NAME FROM APP_USER_ROLE WHERE ROLE_ID = " + roleId).ToString();
        }

        public static UserInfo GetUserInfo(string userName, string password)
        {
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM APP_USER WHERE LOGIN_NAME = '" + userName + "' AND LOGIN_PASSWORD = '" + password + "'");

            if (dt.Rows.Count == 0) return null;

            return new UserInfo(Convert.ToInt32(dt.Rows[0]["ID"]), dt.Rows[0]["LOGIN_NAME"].ToString(), dt.Rows[0]["LOGIN_PASSWORD"].ToString(), dt.Rows[0]["DISPLAY_NAME"].ToString(), Convert.ToInt32(dt.Rows[0]["ROLE_ID"]));
        }

        public static UserInfo ActiveUser { get; set; } = null;

        //public static UserInfo GetGenericeUserInfo(string userName)
        //{
        //    return new UserInfo(102, "Guest", "Guest", "Guest", 2);
        //}

        public UserInfo(int userId, string userName, string password, string displayName, int roleId)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
            RoleId = roleId;
            DisplayName = displayName;
        }
    }
}
