namespace Playground.ConsoleApp.BankingEx
{
    internal class Customer : Person
    {
        private string _accountNumber;

        public Customer(string fName, string lName, string acct)
            : base(fName, lName)
        {
            _accountNumber = acct;
        }

        public string AccntNum
        {
            get => _accountNumber;
            set => _accountNumber = value;
        } 

        public new string ToString()
        {
            return (base.ToString() + "\t:\t" + _accountNumber);
        }
    }
}