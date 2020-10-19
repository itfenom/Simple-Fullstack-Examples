using System;
using System.IO;

namespace Playground.ConsoleApp.FinalProject
{
    public class AccountManager
    {
        private readonly Account[] _account;
        // ReSharper disable once InconsistentNaming
        private const int CHECKING_ACCOUNT = 1;
        // ReSharper disable once InconsistentNaming
        private const int SAVINGS_ACCOUNT = 2;

        public AccountManager()
        {
            _account = new Account[10];
        }

        public void LoadData()
        {
            try
            {
                var fileName = "initialData.txt";
                var assemblyWithPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var fi = new FileInfo(assemblyWithPath);
                // ReSharper disable once AssignNullToNotNullAttribute
                var path = Path.Combine(fi.DirectoryName, fileName);

                var reader = new StreamReader(path);
                string currentLine;
                string[] row;
                int currPos = 0;
                CheckingAccount temp1;
                SavingsAccount temp2;
                while ((currentLine = reader.ReadLine()) != null)
                {
                    row = currentLine.Split(',');
                    temp1 = new CheckingAccount(row[0], Double.Parse(row[1]));
                    temp2 = new SavingsAccount(row[0], Double.Parse(row[2]));
                    _account[currPos] = temp1;
                    currPos++;
                    _account[currPos] = temp2;
                    currPos++;
                }
                //close the file
                reader.Close();
            }// end try
            catch (Exception e)
            {
                Console.WriteLine("Error!, File not found");
                Console.WriteLine(e.Message);
            }// end catch
        }

        public int FindAccountNumber(string searchKey)
        {
            Account a;
            for (int pos = 0; pos < _account.Length; pos++)
            {
                if (_account[pos] != null)
                {
                    a = _account[pos];
                    if (searchKey == a.AccountNumber)
                        return pos;
                }
            } 

            return -1;
        }

        public int FindAccountNumber(string searchKey, int accountType)
        {
            Account a;
            for (int pos = 0; pos < _account.Length; pos++)
            {
                if (_account[pos] != null)
                {
                    a = _account[pos];
                    if (searchKey == a.AccountNumber)
                    {
                        if ((accountType == 1) && (a is CheckingAccount))
                            return pos;
                        if ((accountType == 2) && (a is SavingsAccount))
                            return pos;
                    }
                }
            } 

            return -1;
        }

        public void ExecuteAccountSummary()
        {
            Account curr;

            double checkingBalance = 0;
            double savingBalance = 0;
            double totalAmount = 0;

            int totalAccounts = 0;
            int totalChecking = 0;
            int totalSavings = 0;

            for (int pos = 0; pos < _account.Length; pos++)
            {
                curr = _account[pos];
                if (curr != null)
                {
                    if (curr is CheckingAccount)
                    {
                        checkingBalance += curr.AccountBalance;
                        totalChecking++;
                    }
                    else if (curr is SavingsAccount)
                    {
                        savingBalance += curr.AccountBalance;
                        totalSavings++;
                    }
                    totalAmount += curr.AccountBalance;
                    totalAccounts++;
                }  
            }  

            Console.WriteLine("Total Number of Accounts: {0}  : Total Amount            : {1} ", totalAccounts, totalAmount);
            Console.WriteLine("Total Checking Accounts : {0}  : Total Amount in Checking: {1} ", totalChecking, checkingBalance);
            Console.WriteLine("Total Savings Accounts  : {0}  : Total Amount in Savings : {1} ", totalSavings, savingBalance);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteViewBalance(Account a, int accountType)
        {
            Console.WriteLine(a.ToString());
            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteDeposit(Account a, double amount, int accountType)
        {
            a.Deposit(amount);
            Console.WriteLine(a.ToString());
            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteWithdraw(Account a, double amount, int accountType)
        {
            a.Withdraw(amount);
            Console.WriteLine(a.ToString());
            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ExecuteTransfer(Account from, Account to, double amount, int accountType)
        {
            from.Transfer(to, amount);
            Console.WriteLine(from.ToString());
            Console.WriteLine(to.ToString());
            return true;
        }

        public bool ExecuteAccountActions(int accountType, int accountActions, int accountPosition, string inputData)
        {
            int currPosition;
            Account curr;

            currPosition = accountPosition;
            if (currPosition >= 0)
            {
                curr = _account[currPosition];
                switch (accountActions)
                {
                    case 1:
                        ExecuteViewBalance(curr, accountType);
                        break;

                    case 2:
                        Console.Write("Enter Deposit Amount : ");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        double depositAmount = Double.Parse(Console.ReadLine());
                        ExecuteDeposit(curr, depositAmount, accountType);
                        break;

                    case 3:
                        Console.Write("Enter Withdrawal Amount : ");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        double withdrawAmount = Double.Parse(Console.ReadLine());
                        ExecuteWithdraw(curr, withdrawAmount, accountType);
                        break;

                    case 4:

                        Console.Write("Enter Amount to transfer : ");
                        // ReSharper disable once AssignNullToNotNullAttribute
                        double transferAmount = Double.Parse(Console.ReadLine());

                        Console.WriteLine("1. To Transfer from Checking to Savings:");
                        Console.WriteLine("2. To Transfer from Savings to Checking:");
                        int choice = Convert.ToInt16(Console.ReadLine());

                        int checkingAccountPosition = FindAccountNumber(inputData, CHECKING_ACCOUNT);
                        int savingsAccountPosition = FindAccountNumber(inputData, SAVINGS_ACCOUNT);

                        Account from;
                        Account to;

                        if (choice == 1)
                        {
                            from = _account[(checkingAccountPosition)];
                            to = _account[(savingsAccountPosition)];
                        }
                        else
                        {
                            from = _account[(savingsAccountPosition)];
                            to = _account[(checkingAccountPosition)];
                        }

                        ExecuteTransfer(from, to, transferAmount, accountType);
                        break;

                    default:
                        AccountTypeMenu();
                        break;
                }
            }
            else
                return false;
            return true;
        }

        public int AccountTypeMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Checking Account.");
            Console.WriteLine("2. Savings Account.");
            Console.WriteLine("3. Admin Menu.");
            Console.WriteLine("4. EXIT.");
            Console.Write("Enter your Accoount Type: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            return choice;
        }

        public int AccountActionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. View Balance.");
            Console.WriteLine("2. Deposit.");
            Console.WriteLine("3. Withdraw.");
            Console.WriteLine("4. Transfer funds.");
            Console.WriteLine("5. Return to Previous Menu. ");
            Console.Write("Enter your Selection:  ");
            int choice = Convert.ToInt32(Console.ReadLine());
            return choice;
        }
    }
}