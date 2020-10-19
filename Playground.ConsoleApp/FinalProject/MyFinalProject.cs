using System;

namespace Playground.ConsoleApp.FinalProject
{
    public class MyFinalProject
    {
        public void Start()
        {
            AccountManager accountManager = new AccountManager();

            accountManager.LoadData();

            string inputData = "continue";
            int accountPosition;
            int accountTypeChoice = 0;
            int accountActionChoice;

            while ((accountTypeChoice != 4) && (inputData != "exit"))
            {
                Console.WriteLine(" *** Welcome to ATM *** ");
                Console.Write("To logon, Enter your Account Number, exit to quit. ");
                inputData = Console.ReadLine();

                // ReSharper disable once RedundantAssignment
                if ((accountPosition = accountManager.FindAccountNumber(inputData)) != -1)
                {
                    accountTypeChoice = accountManager.AccountTypeMenu();
                    if (accountTypeChoice == 3)
                    {
                        Console.Write("Enter Administrator Password:");
                        string adminPassword = Console.ReadLine();
                        if (adminPassword == "let me in")
                        {
                            Console.WriteLine("\n *************** ACCOUNT SUMMARY ***************  \n");
                            accountManager.ExecuteAccountSummary();
                            Console.WriteLine("\n ***************   END SUMMARY   *************** \n");
                        }
                    }
                    else if (accountTypeChoice != 4)
                    {
                        accountActionChoice = accountManager.AccountActionMenu();
                        accountPosition = accountManager.FindAccountNumber(inputData, accountTypeChoice);
                        accountManager.ExecuteAccountActions(accountTypeChoice, accountActionChoice, accountPosition, inputData);
                    }
                    else accountTypeChoice = 4;
                }
            }
        }
    }
}