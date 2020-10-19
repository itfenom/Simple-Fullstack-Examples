namespace Playground.ConsoleApp.FinalProject
{
    public class SavingsAccount : Account
    {
        // ReSharper disable once InconsistentNaming
        private const string accountType = "Savings";

        public SavingsAccount(string accountNumber, double balance)
            : base(accountNumber, balance)
        {
        } 

        public string AccountType => accountType;

        public new string ToString()
        {
            return (base.ToString() + " Account Type: " + accountType);
        }
    }
}