using System;
using System.Collections.Generic;
using System.Data;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.WpfApp.Forms.DataGridsEx.AccountMgr;

namespace Playground.WpfApp.Repositories
{
    public interface IAccountRepository
    {
        List<CategoryModel> GetAllCategories();

        List<AccountModel> GetAccountsByCategoryId(int categoryId);

        void InsertNewAccountCategory(string newCategoryName);

        void UpdateAccountCategory(int categoryId, string newCategoryName);

        void DeleteAccountCategory(int categoryId);

        bool CategoryHasChildren(int categoryId);

        AccountModel GetAccountByAccountId(int accountId);

        void InsertNewAccount(AccountModel newAcctModel);

        void DeleteAccount(int accountId);

        void UpdateAccount(AccountModel updatedAcctModel);

        string GetPassword(int accountId);
    }

    public class AccountRepository : IAccountRepository
    {
        private string _sql = string.Empty;

        public bool CategoryHasChildren(int categoryId)
        {
            bool retVal = false;

            object obj = DAL.Seraph.ExecuteScalar("SELECT COUNT(*) FROM ACCT_MGR WHERE CATEGORY_ID = " + categoryId);

            if (Convert.ToInt32(obj) > 0)
            {
                retVal = true;
            }

            return retVal;
        }

        public void DeleteAccount(int accountId)
        {
            DAL.Seraph.ExecuteNonQuery("DELETE FROM ACCT_MGR WHERE ID = " + accountId);
        }

        public void DeleteAccountCategory(int categoryId)
        {
            DAL.Seraph.ExecuteNonQuery("DELETE FROM ACCT_CATEGORY WHERE CATEGORY_ID = " + categoryId);
        }

        public AccountModel GetAccountByAccountId(int accountId)
        {
            var retVal = new AccountModel();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM ACCT_MGR WHERE ID = " + accountId);

            if (dt.Rows.Count == 0) return null;

            retVal.AccountId = Convert.ToInt32(dt.Rows[0]["ID"]);
            retVal.AccountName = dt.Rows[0]["ACCT_NAME"].ToString();
            retVal.AccountLoginId = dt.Rows[0]["ACCT_LOGIN_ID"].ToString();
            retVal.AccountPassword = dt.Rows[0]["ACCT_PASSWORD"].ToString();
            retVal.Notes = dt.Rows[0]["ACCT_NOTES"].ToString();
            retVal.CategoryId = Convert.ToInt32(dt.Rows[0]["CATEGORY_ID"]);

            if (dt.Rows[0]["DATE_CREATED"] != null && dt.Rows[0]["DATE_CREATED"] != DBNull.Value)
            {
                retVal.DateCreated = Convert.ToDateTime(dt.Rows[0]["DATE_CREATED"]);
            }

            if (dt.Rows[0]["DATE_MODIFIED"] != null && dt.Rows[0]["DATE_MODIFIED"] != DBNull.Value)
            {
                retVal.DateModified = Convert.ToDateTime(dt.Rows[0]["DATE_MODIFIED"]);
            }

            retVal.IsPasswordEncrypted = dt.Rows[0]["IS_PASSWORD_ENCRYPTED"].ToString();

            return retVal;
        }

        public List<AccountModel> GetAccountsByCategoryId(int categoryId)
        {
            var retVal = new List<AccountModel>();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM ACCT_MGR WHERE CATEGORY_ID = " + categoryId);

            if (dt.Rows.Count == 0) return null;

            foreach (DataRow row in dt.Rows)
            {
                var model = new AccountModel();

                model.AccountId = Convert.ToInt32(row["ID"]);
                model.AccountName = row["ACCT_NAME"].ToString();
                model.AccountLoginId = row["ACCT_LOGIN_ID"].ToString();
                model.AccountPassword = row["ACCT_PASSWORD"].ToString();
                model.Notes = row["ACCT_NOTES"].ToString();
                model.CategoryId = Convert.ToInt32(row["CATEGORY_ID"]);

                if (row["DATE_CREATED"] != null && row["DATE_CREATED"] != DBNull.Value)
                {
                    model.DateCreated = Convert.ToDateTime(row["DATE_CREATED"]);
                }

                if (row["DATE_MODIFIED"] != null && row["DATE_MODIFIED"] != DBNull.Value)
                {
                    model.DateModified = Convert.ToDateTime(row["DATE_MODIFIED"]);
                }

                model.IsPasswordEncrypted = row["IS_PASSWORD_ENCRYPTED"].ToString();

                retVal.Add(model);
            }

            return retVal;
        }

        public List<CategoryModel> GetAllCategories()
        {
            var retVal = new List<CategoryModel>();
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM ACCT_CATEGORY");

            if (dt.Rows.Count == 0) return null;

            foreach (DataRow row in dt.Rows)
            {
                retVal.Add(new CategoryModel
                {
                    CategoryId = Convert.ToInt32(row["CATEGORY_ID"]),
                    CategoryName = row["CATEGORY_NAME"].ToString()
                });
            }

            return retVal;
        }

        public string GetPassword(int accountId)
        {
            return DAL.Seraph.ExecuteScalar("SELECT ACCT_PASSWORD FROM ACCT_MGR WHERE ID = " + accountId).ToString();
        }

        public void InsertNewAccount(AccountModel newAcctModel)
        {
            _sql = "INSERT INTO ACCT_MGR(ID, ACCT_NAME, ACCT_LOGIN_ID, ACCT_PASSWORD, ACCT_NOTES, DATE_CREATED, DATE_MODIFIED, CATEGORY_ID, IS_PASSWORD_ENCRYPTED) VALUES("
                 + "ACCT_SEQ.NEXTVAL, '"
                 + newAcctModel.AccountName + "', '"
                 + newAcctModel.AccountLoginId + "', '"
                 + newAcctModel.AccountPassword + "', '"
                 + HelperTools.FormatSqlString(newAcctModel.Notes) + "', SYSDATE, NULL, "
                 + newAcctModel.CategoryId + ", '"
                 + newAcctModel.IsPasswordEncrypted + "')";
            DAL.Seraph.ExecuteNonQuery(_sql);
        }

        public void InsertNewAccountCategory(string newCategoryName)
        {
            _sql = "INSERT INTO ACCT_CATEGORY(CATEGORY_ID, CATEGORY_NAME) VALUES("
                 + "(SELECT ACCT_SEQ.NEXTVAL FROM DUAL), '" + newCategoryName + "')";

            DAL.Seraph.ExecuteNonQuery(_sql);
        }

        public void UpdateAccount(AccountModel updatedAcctModel)
        {
            _sql = "UPDATE ACCT_MGR SET ACCT_NAME = '" + updatedAcctModel.AccountName
                 + "', ACCT_LOGIN_ID = '" + updatedAcctModel.AccountName
                 + "', ACCT_PASSWORD = '" + updatedAcctModel.AccountPassword
                 + "', ACCT_NOTES = '" + HelperTools.FormatSqlString(updatedAcctModel.Notes)
                 + "', DATE_MODIFIED = SYSDATE, CATEGORY_ID = " + updatedAcctModel.CategoryId
                 + ", IS_PASSWORD_ENCRYPTED = '" + updatedAcctModel.IsPasswordEncrypted
                 + "' WHERE ID = " + updatedAcctModel.AccountId;

            DAL.Seraph.ExecuteNonQuery(_sql);
        }

        public void UpdateAccountCategory(int categoryId, string newCategoryName)
        {
            _sql = "UPDATE ACCT_CATEGORY SET CATEGORY_NAME = '" + newCategoryName
                 + "' WHERE CATEGORY_ID = " + categoryId;

            DAL.Seraph.ExecuteNonQuery(_sql);
        }
    }
}