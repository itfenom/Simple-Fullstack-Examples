using System;
using System.IO;

namespace Playground.ConsoleApp.BankingEx
{
    public class Bank
    {
        // ReSharper disable once InconsistentNaming
        private const int _MAX_CUSTOMERS = 100;
        // ReSharper disable once InconsistentNaming
        private const int _MAX_ACCOUNTS = 2 * _MAX_CUSTOMERS;

        // ReSharper disable once InconsistentNaming
        private const int _ACCOUNT_TYPE_CHECKING = 1;
        // ReSharper disable once InconsistentNaming
        private const int _ACCOUNT_TYPE_SAVING = 2;

        // ReSharper disable once NotAccessedField.Local
        private string _bankName;
        // ReSharper disable once NotAccessedField.Local
        private ATM _bankAtm;
        private Customer[] _customers;
        private Account[] _accounts;

        public Bank()
        {
            _bankName = "Bank of Anonymous";
            _customers = new Customer[_MAX_CUSTOMERS];
            _accounts = new Account[_MAX_ACCOUNTS];

            string _fileNameAccount = "InitialAccountInformation.txt";
            string _fileNameCustomer = "InitialCustomerInformation.txt";
            var assemblyWithPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var fi = new FileInfo(assemblyWithPath);

            // ReSharper disable once AssignNullToNotNullAttribute
            string accountPath = Path.Combine(fi.DirectoryName, _fileNameAccount);
            string customerPath = Path.Combine(fi.DirectoryName, _fileNameCustomer);

            LoadAccountData(accountPath);
            //Console.Read();
            LoadCustomerData(customerPath);
            //Console.Read();

            _bankAtm = new ATM();
        }  

        public Bank(string bankName, string accountsPath, string customerPath)
        {
            _bankName = bankName;
            _customers = new Customer[_MAX_CUSTOMERS];
            _accounts = new Account[_MAX_ACCOUNTS];

            LoadAccountData(accountsPath);
            //Console.Read();
            LoadCustomerData(customerPath);
            //Console.Read();

            _bankAtm = new ATM();
        } 

        private void LoadCustomerData(string filePath)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int FIRST_NAME = 0;
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int LAST_NAME = 1;
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int ACCOUNT = 1;
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int NAME = 0;

                var reader = new StreamReader(filePath);
                string currentLine;
                string[] row;
                string[] name;
                int currPos = 0;
                Customer temp;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //displays the contents of the file till it reaches the null or end of file.
                    //Console.WriteLine("{0}", currentLine);
                    row = currentLine.Split(',');
                    name = row[NAME].Split(' ');

                    temp = new Customer(name[FIRST_NAME], name[LAST_NAME], row[ACCOUNT]);
                    // Console.WriteLine("Loading : ACCOUNT NUMBER : {0} ---> {1} {2}", row[_ACCOUNT],name[_FIRST_NAME], name[_LAST_NAME]);

                    _customers[currPos] = temp;
                    currPos++;
                }

                reader.Close();
            }  
            catch (Exception e)
            {
                Console.WriteLine("Error! File not found.  [" + filePath + "]");
                Console.WriteLine(e.Message);
            } 
        } 

        private void LoadAccountData(string filePath)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int ACCOUNT_NUMBER = 0;
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int SAVINGS_BALANCE = 2;
                // ReSharper disable once UnusedVariable
                // ReSharper disable once InconsistentNaming
                const int CHECKING_BALANCE = 1;

                var reader = new StreamReader(filePath);
                string currentLine;
                string[] row;
                int currPos = 0;
                Checking temp;
                Savings temp2;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //displays the contents of the file till it reaches the null or end of file.

                    row = currentLine.Split(',');

                    temp = new Checking(row[ACCOUNT_NUMBER], Double.Parse(row[CHECKING_BALANCE]));
                    //Console.WriteLine("Loading : ACCOUNT NUMBER : {0} CHECKING BALANCE : {1}", row[_ACCOUNT_NUMBER], row[_CHECKING_BALANCE]);

                    temp2 = new Savings(row[ACCOUNT_NUMBER], Double.Parse(row[SAVINGS_BALANCE]));
                    // Console.WriteLine("Loading : ACCOUNT NUMBER : {0} SAVINGS BALANCE : {1}", row[_ACCOUNT_NUMBER], row[_SAVINGS_BALANCE]);

                    _accounts[currPos] = temp;
                    currPos++;
                    _accounts[currPos] = temp2;
                    currPos++;
                }

                reader.Close();
            }  
            catch (Exception e)
            {
                Console.WriteLine("Error! File not found. [" + filePath + "]");
                Console.WriteLine(e.Message);
            } 
        } 

        public int FindAccountNumber(string searchKey)
        {
            Account a;
            for (int pos = 0; pos < _accounts.Length; pos++)
            {
                if (_accounts[pos] != null)
                {
                    a = _accounts[pos];
                    if (searchKey == a.AccountNumber)
                        return pos;
                }
            } 

            return -1;
        }

        public int FindCustomer(string searchKey)
        {
            Customer a;
            for (int pos = 0; pos < _customers.Length; pos++)
            {
                if (_customers[pos] != null)
                {
                    a = _customers[pos];
                    if (searchKey == a.AccntNum)
                        return pos;
                }
            } 

            return -1;
        }

        public int FindAccountNumber(string searchKey, int accountType)
        {
            Account a;
            for (int pos = 0; pos < _accounts.Length; pos++)
            {
                if (_accounts[pos] != null)
                {
                    a = _accounts[pos];
                    if (searchKey == a.AccountNumber)
                    {
                        if ((accountType == _ACCOUNT_TYPE_CHECKING) && (a is Checking))
                            return pos;
                        if ((accountType == _ACCOUNT_TYPE_SAVING) && (a is Savings))
                            return pos;
                    }
                }
            }

            return -1;
        }

        public void Summarize()
        {
            Account curr;
            double totalAmount = 0, totalCheckingBal = 0, totalSavingsBal = 0;
            int totalAccounts = 0, totalChecking = 0, totalSavings = 0;

            for (int pos = 0; pos < _MAX_ACCOUNTS; pos++)
            {
                curr = _accounts[pos];
                if (curr != null)
                {
                    if (curr is Checking)
                    {
                        totalCheckingBal += curr.AccountBalance;
                        totalChecking++;
                    }
                    if (curr is Savings)
                    {
                        totalSavingsBal += curr.AccountBalance;
                        totalSavings++;
                    }
                    totalAmount += curr.AccountBalance;
                    totalAccounts++;
                } 
            }  

            Console.WriteLine(" Total Accounts : {0}  Total Balance : {1} ", totalAccounts, totalAmount);
            Console.WriteLine(" Total Checking Accounts : {0}  Total Checking Balance : {1} ", totalCheckingBal, totalChecking);
            Console.WriteLine(" Total Savings Accounts : {0}  Total Savings Balance : {1} ", totalSavingsBal, totalSavings);
        }

        public void PrintRecords()
        {
            Account curr;
            Customer c;
            string output = "";

            for (int pos = 0; pos < _MAX_ACCOUNTS; pos++)
            {
                curr = _accounts[pos];
                if (curr != null)
                {
                    if (curr is Checking)
                        output = ((Checking)curr).ToString();
                    if (curr is Savings)
                        output = ((Savings)curr).ToString();
                    //Console.WriteLine(" HERE " + pos);
                    c = _customers[FindCustomer(curr.AccountNumber)];
                    output += "\n\t\t" + c.ToString() + "\n";

                    Console.WriteLine(output);
                    output = "";
                } 
            }  
        }

        private static int Menu1()
        {
            Console.WriteLine(" 1. Checking Account ");
            Console.WriteLine(" 2. Savings Account ");
            Console.WriteLine(" 3. Exit ");
            // ReSharper disable once AssignNullToNotNullAttribute
            return (Int16.Parse(Console.ReadLine()));
        }

        private static int Menu2()
        {
            Console.WriteLine(" 1. Deposit ");
            Console.WriteLine(" 2. Balance ");
            Console.WriteLine(" 3. Withdraw ");
            Console.WriteLine(" 4. Transfer ");
            Console.WriteLine(" 5. Previous Screen ");
            // ReSharper disable once AssignNullToNotNullAttribute
            return (Int16.Parse(Console.ReadLine()));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteDeposit(Account a, double depositAmount, int accountType)
        {
            return (a.Deposit(depositAmount));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteTransfer(Account fromAccount, Account toAccount, double transferAmount, int accountType)
        {
            return (fromAccount.Transfer(toAccount, transferAmount));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteAction(int accountType, int accountAction, string currAccountNumber)
        {
            // ReSharper disable once InconsistentNaming
            const int _ACTION_DEPOSIT = 1;
            // ReSharper disable once InconsistentNaming
            const int _ACTION_DISPLAY = 2;
            // ReSharper disable once IdentifierTypo
            // ReSharper disable once InconsistentNaming
            const int _ACTION_TRANFSER = 4;

            int currPosition;
            Account curr;

            currPosition = FindAccountNumber(currAccountNumber, accountType);
            if (currPosition >= 0)
            {
                curr = _accounts[currPosition];
                switch (accountAction)
                {
                    case _ACTION_DEPOSIT:
                        Console.Write("Enter Deposit Amount : ");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        double depositAmount = Double.Parse(Console.ReadLine());
                        ExecuteDeposit(curr, depositAmount, accountType);
                        break;

                    case _ACTION_DISPLAY:

                        PrintRecords();
                        Summarize();
                        break;

                    case _ACTION_TRANFSER:

                        Console.Write("Enter Transfer Amount : ");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        double transferAmount = Double.Parse(Console.ReadLine());

                        Console.WriteLine("1. Transfer From Checking to Savings");
                        Console.WriteLine("2. Transfer From Savings to Checking");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        int choice = Int16.Parse(Console.ReadLine());

                        int checkingAccountPosition = FindAccountNumber(currAccountNumber, _ACCOUNT_TYPE_CHECKING);
                        int savingsAccountPosition = FindAccountNumber(currAccountNumber, _ACCOUNT_TYPE_SAVING);

                        Account from, to;

                        if (choice == 1)
                        {
                            from = _accounts[checkingAccountPosition];
                            to = _accounts[savingsAccountPosition];
                        }
                        else
                        {
                            to = _accounts[checkingAccountPosition];
                            from = _accounts[savingsAccountPosition];
                        } 

                        ExecuteTransfer(from, to, transferAmount, accountType);

                        break;
                }
            }
            else
                return false;

            return true;
        }

        public void StartProcessing()
        {
            // ReSharper disable once InconsistentNaming
            const string STOP = "QUIT";

            string currentAccountNumber = "";

            while (currentAccountNumber != STOP)
            {
                Console.WriteLine("Enter Account Number : ");
                currentAccountNumber = Console.ReadLine();

                int accountType;
                if ((FindAccountNumber(currentAccountNumber) >= 0) && ((accountType = Menu1()) != 3))
                {
                    var accountAction = Menu2();
                    ExecuteAction(accountType, accountAction, currentAccountNumber);
                }
                else
                {
                    currentAccountNumber = STOP;
                }
            } 
        }
    }
}