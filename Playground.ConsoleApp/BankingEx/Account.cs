namespace Playground.ConsoleApp.BankingEx
{
    internal class Account
    {
        private string _accountNumber;
        private double _accountBalance;

        public Account(string accNum, double balance)
        {
            _accountNumber = accNum;
            _accountBalance = balance;
        } 

        public string AccountNumber
        {
            get => _accountNumber;
            set => _accountNumber = value;
        }  

        public double AccountBalance
        {
            get => _accountBalance;
            set => _accountBalance = value;
        }  

        public bool Deposit(double amount)
        {
            _accountBalance += amount;
            return true;
        }

        public bool Withdraw(double amount)
        {
            double newBalance = _accountBalance - amount;
            if (newBalance >= 0)
            {
                _accountBalance = newBalance;
            }
            else
                return false;

            return true;
        }

        public bool Transfer(Account toAccount, double transferAmount)
        {
            if (Withdraw(transferAmount))
            {
                toAccount.Deposit(transferAmount);
            }
            else
                return false;

            return true;
        } 

        public new string ToString()
        {
            return ("\tAccount Number : " + _accountNumber + "\tAccount Balance : " + _accountBalance);
        }
    }
}