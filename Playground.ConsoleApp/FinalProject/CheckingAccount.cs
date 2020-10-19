namespace Playground.ConsoleApp.FinalProject
{
    public class CheckingAccount : Account
    {
        // ReSharper disable once InconsistentNaming
        private const string accountType = "Checking";

        public CheckingAccount(string accountNumber, double balance)
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