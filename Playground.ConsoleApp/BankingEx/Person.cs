namespace Playground.ConsoleApp.BankingEx
{
    internal class Person
    {
        private string _firstName;
        private string _lastName;
        //private string _socialSecurity;
        //private int _age;

        public Person(string fName, string lName)
        {
            _firstName = fName;
            _lastName = lName;
        }

        public string FirstName
        {
            get => _firstName;
            set => _firstName = value;
        }  

        public string LastName
        {
            get => _lastName;
            set => _lastName = value;
        }  

        public override string ToString()
        {
            return (_lastName + ",\t" + _firstName);
        }
    }
}