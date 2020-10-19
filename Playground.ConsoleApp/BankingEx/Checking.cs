namespace Playground.ConsoleApp.BankingEx
{
    internal class Checking : Account
    {
        // ReSharper disable once InconsistentNaming
        private const string account_type = "C";

        public Checking(string accNum, double bal)
            : base(accNum, bal)
        {
        } 

        public string AccountType => account_type;

        public new string ToString()
        {
            return (base.ToString() + "\tAccount Type : " + account_type);
        }
    }
}