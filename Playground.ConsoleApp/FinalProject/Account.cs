using System;

namespace Playground.ConsoleApp.FinalProject
{
    public class Account
    {
        private double _accountBalance;
        private string _accountNumber;

        public Account() 
        {
        }

        public Account(string acctNum, double balance)
        {
            _accountBalance = balance;
            _accountNumber = acctNum;
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

        public double Deposit(double amt)
        {
            _accountBalance += amt;
            return _accountBalance;
        } // end method deposit

        public double Withdraw(double amt)
        {
            double balance = _accountBalance - amt;
            if (balance >= 0)
            {
                _accountBalance = balance;
            }
            else
            {
                Console.WriteLine("Withdrawal failed!!!");
                Console.WriteLine("You do not have sufficient funds in your account.");
                Console.WriteLine("No change was made to the account!!! \n");
            }

            return 1;
        } 

        public double Transfer(Account toAccount, double transferAmt)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Withdraw(transferAmt) == 1)
            {
                toAccount.Deposit(transferAmt);
            }
            else
                return -1;
            return 1;
        } 

        public new string ToString()
        {
            return ("Account Number: " + _accountNumber + "\tAccount Balance: " + _accountBalance + "\n");
        }
    }
}