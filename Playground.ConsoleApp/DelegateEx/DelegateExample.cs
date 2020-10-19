using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Playground.ConsoleApp.DelegateEx
{
    public delegate bool ValidateNumberDelegate(int number);
    public delegate void SampleDelegate(int num);
    public class DelegateExample
    {
        private readonly DelegateExampleOtherClass _otherClass = new DelegateExampleOtherClass();
        private Func<PersonDelegate, string> _personFormatter;

        public void ExecuteCallBackVoidDelegate()
        {
            Console.WriteLine("\nDisplaying numbers using CallBack.");
            _otherClass.LongRunningMethod(CallBack);
        }

        public void ExecuteGenericFunc()
        {
            Console.WriteLine("\n*** Use of Generic Func<string, string>()");
            Console.WriteLine("Enter a Characters  to convert to upper-case:");
            string inputVal = Console.ReadLine();

            // ReSharper disable once ConvertToLocalFunction
            Func<string, string> convertToUpper = s => s.ToUpper();

            Console.WriteLine(convertToUpper(inputVal) + "\n");
        }

        public void ExecuteDisplayLastNamesInPersonList()
        {
            Console.WriteLine("\nLast names (upperCase) in person list are:\n");
            _personFormatter = p => p.LastName.ToUpper();

            var persons = GetPersonList();
            foreach (var person in persons)
            {
                Console.WriteLine(person.ToString(_personFormatter));
            }
        }

        public void ExecuteCarEvents()
        {
            Car c1 = new Car("SlugBug", 100, 10);

            c1.RegisterWithCarEngine(OnCarEngineEvent);

            Console.WriteLine("\n**** Speeding up *****");
            for (int i = 0; i < 6; i++)
            {
                c1.Accelerate(20);
            }
        }

        public void ExecutePrintNumbersViaDelegate()
        {
            Console.WriteLine("\nPrinting numbers using Yield return");
            int[] numbers = new[] { 3, 5, 1, 2, 7, 8, 13, 11, 45, 67, 89, 19 };

            //var _result = RunNumbers(_numbers, p => p < 5);
            //Console.WriteLine("\nNumbers less than 5:\n");

            //ar _result = RunNumbers(_numbers, p => p > 10);
            //Console.WriteLine("\nNumbers greater than 10:\n");

            var result = RunNumbers(numbers, p => p > 13);
            Console.WriteLine("\nNumbers greater than 13:\n");

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        public void ExecuteDisplayEvenNumbersViaDelegate()
        {
            Console.WriteLine("\nDisplaying Even Numbers:\n");
            var looper = new Looper(1, 10, p => (p % 2 == 0));
            looper.DisplayNumbers();
        }


        public void ExecuteDisplayOddNumbersViaDelegate()
        {
            Console.WriteLine("\nDisplaying Odd Numbers:\n");
            var looper = new Looper(1, 10, p => (p % 2 == 1));
            looper.DisplayNumbers();
        }

        private IEnumerable<int> RunNumbers(IEnumerable<int> numbers, Func<int, bool> outputNumber)
        {
            foreach (int number in numbers)
            {
                if (outputNumber(number))
                    yield return number;
            }
        }

        private void OnCarEngineEvent(string msg)
        {
            Console.WriteLine("\n***** Message from Car object *****");
            Console.WriteLine("=> {0}", msg);
            Console.WriteLine("*************************************\n");
        }

        private void CallBack(int number)
        {
            Console.WriteLine("Now processing: " + number);
        }

        private List<PersonDelegate> GetPersonList()
        {
            var retVal = new List<PersonDelegate> {
                new PersonDelegate{ FirstName = "John", LastName = "Deo"},
                new PersonDelegate{ FirstName = "Jean", LastName = "Doe"},
                new PersonDelegate{ FirstName = "Foo", LastName = "Bar"},
                new PersonDelegate{ FirstName = "This", LastName = "That"},
            };

            return retVal;
        }
    }

    public class DelegateExampleOtherClass
    {
        public void LongRunningMethod(Action<int> number)
        {
            for (int i = 1; i < 5; i++)
            {
                Thread.Sleep(1000);
                number(i);
            }
        }
    }

    public class Car
    {
        public int MaxCarSpeed { get; set; }
        public int CurrentSpeed { get; set; }
        public string PetName { get; set; }

        private bool _carIsDead;

        public Car()
        {
            MaxCarSpeed = 100;
        }

        // ReSharper disable once IdentifierTypo
        public Car(string name, int maxSpeed, int currSpeed)
        {
            PetName = name;
            CurrentSpeed = currSpeed;
            MaxCarSpeed = maxSpeed;
        }

        //Define a delegate
        public delegate void CarEngineHandler(string msgForCaller);

        //declare member variable
        // ReSharper disable once InconsistentNaming
        private CarEngineHandler listOfHandlers;

        //Method to call
        public void RegisterWithCarEngine(CarEngineHandler methodToCall)
        {
            listOfHandlers = methodToCall;
        }

        public void Accelerate(int delta)
        {
            if (_carIsDead)
            {
                listOfHandlers?.Invoke("Sorry, this can is dead...");
            }
            else
            {
                CurrentSpeed += delta;

                // ReSharper disable once UseNullPropagation
                if (10 == (MaxCarSpeed - CurrentSpeed) && listOfHandlers != null)
                {
                    listOfHandlers("Careful buddy!, Gonna blow!");
                }
            }

            if (CurrentSpeed >= MaxCarSpeed)
                _carIsDead = true;
            else
                Console.WriteLine("Current Speed = {0}", CurrentSpeed);
        }
    }

    internal class PersonDelegate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName}, {LastName}";
        }

        public string ToString(Func<PersonDelegate, string> personFormatter)
        {
            if (personFormatter != null)
                return personFormatter.Invoke(this);
            return ToString();
        }
    }

    // ReSharper disable once IdentifierTypo
    internal class Looper
    {
        private readonly int _start;
        private readonly int _end;
        private readonly ValidateNumberDelegate _validateNumber;

        // ReSharper disable once IdentifierTypo
        public Looper(int start, int end, ValidateNumberDelegate validateNumber)
        {
            _start = start;
            _end = end;
            _validateNumber = validateNumber;
        }

        public void DisplayNumbers()
        {
            for (int i = _start; i <= _end; i++)
            {
                var isValid = _validateNumber?.Invoke(i);
                if (isValid == true)
                {
                    Console.WriteLine(i);
                }
            }
        }
    }

    internal class MyDelegate
    {
        public class PersonObj
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        Func<PersonObj, bool> filterDelegate;
        
        private List<PersonObj> GetPersons()
        {
            var p = new List<PersonObj>();

            p.Add(new PersonObj { Name = "John", Age = 12});
            p.Add(new PersonObj { Name = "Jane", Age = 40 });
            p.Add(new PersonObj { Name = "Jake", Age = 25 });
            p.Add(new PersonObj { Name = "Jessie", Age = 50 });
            p.Add(new PersonObj { Name = "Joe", Age = 17 });

            return p;
        }

        public void DisplayPerson(string title, List<PersonObj> people, Func<PersonObj, bool> filter)
        {
            foreach (var item in people)
            {
                if(filter(item))
                {
                    Console.WriteLine($"{item.Name}, {item.Age} years old.");
                }
            }
        }


        //Filters to pass into above method.
        static bool IsChild(PersonObj p) { return p.Age < 18; }
        static bool IsAdult(PersonObj p) { return p.Age >= 18; }
        static bool IsSenior(PersonObj p) { return p.Age >= 65; }

        //Usage:
        //DisplayPerson("Children:", GetPersons(), IsChild);
        //DisplayPerson("Adult:", GetPersons(), IsAdult);
        //DisplayPerson("Senior:", GetPersons(), IsSenior);
    }
}
