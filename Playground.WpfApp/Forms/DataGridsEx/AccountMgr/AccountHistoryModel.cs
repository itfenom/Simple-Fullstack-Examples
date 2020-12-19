using System;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public class AccountHistoryModel : PropertyChangedBase
    {
        private string _transactionType;

        public string TransactionType
        {
            get => _transactionType;
            set => SetPropertyValue(ref _transactionType, value);
        }

        private string _timeStamp;

        public string TimeStamp
        {
            get => _timeStamp;
            set => SetPropertyValue(ref _timeStamp, value);
        }

        private string _category;

        public string Category
        {
            get => _category;
            set => SetPropertyValue(ref _category, value);
        }

        private string _account;

        public string Account
        {
            get => _account;
            set => SetPropertyValue(ref _account, value);
        }

        private string _loginId;

        public string LoginId
        {
            get => _loginId;
            set => SetPropertyValue(ref _loginId, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetPropertyValue(ref _password, value);
        }

        private string _notes;

        public string Notes
        {
            get => _notes;
            set => SetPropertyValue(ref _notes, value);
        }

        private string _dateCreated;

        public string DateCreated
        {
            get => _dateCreated;
            set => SetPropertyValue(ref _dateCreated, value);
        }

        private string _dateModified;

        public string DateModified
        {
            get => _dateModified;
            set => SetPropertyValue(ref _dateModified, value);
        }

    }
}
