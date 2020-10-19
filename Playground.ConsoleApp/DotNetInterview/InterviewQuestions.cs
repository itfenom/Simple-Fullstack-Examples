using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Playground.ConsoleApp.DotNetInterview
{
    public static class InterviewQuestions
    {
        public static void CheckEven(int num)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (num % 2 == 0)
                Console.WriteLine("\nThis number is an even number");
            else
                Console.WriteLine("\nThis number is an odd number");
        }

        public static void SquareNumber(int num)
        {
            Console.WriteLine("\nSquare of this number is: {0}", num * num);
        }

        public static void PrintHighestNumberInArrays()
        {
            int[] intArrays = new[] { 2, 5, 15, 89, 3, 22, 69 };
            var highestNumber = intArrays[0];

            for (int i = 0; i < intArrays.Length; i++)
            {
                if (intArrays[i] > highestNumber)
                {
                    highestNumber = intArrays[i];
                }
            }

            Console.WriteLine($"\nHighest number in arrays {string.Join(",", intArrays)} is: {highestNumber}");
        }

        public static int RecursiveMethod(int n)
        {
            if (n == 0) return 1;

            return n * RecursiveMethod(n - 1);
        }

        public static string ReverseString(string input)
        {
            var sb = new StringBuilder();
            char[] cArray = input.ToCharArray();
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                sb.Append(cArray[i]);
            }

            return sb.ToString();
        }

        public static void PrintEvenNumbers()
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= 20; i++)
            {
                if (i % 2 == 0)
                    sb.Append(" " + i.ToString() + " ");
            }

            //Enumerable.Range(1, 21).Where(n => n % 2 == 0).ToList().ForEach(Console.WriteLine);

            Console.WriteLine("\nEven # up to 20 is: " + sb + "\n");
        }

        public static int CalculateAge(DateTime birthDate)
        {
            var now = new DateTime(2016, 5, 25);
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }

        public static void CountCharactersInString(string inputVal)
        {
            var sb = new StringBuilder();
            var backupVal = inputVal;
            var length = inputVal.Length;
            while (length != 0)
            {
                var character = inputVal[0];
                var count = CountChars(inputVal, character);
                inputVal = inputVal.Replace(character.ToString(), "");
                sb.AppendLine(character + ": appeared " + count.ToString() + " times.");
                length = length - count;
            }

            Console.WriteLine($"\nCharacter count in: {backupVal}\n{sb}\n");
        }

        private static int CountChars(string s, char c)
        {
            int counter = 0;
            foreach (char chr in s)
            {
                if (chr == c)
                {
                    counter++;
                }
            }
            return counter;
        }

        public static void GetMaxCountInListUsingLinq()
        {
            var list1 = new List<int>
            {
                1, 2, 4, 50, 500, 1000
            };

            var list2 = new List<int>
            {
                4, 5, 6
            };

            var list3 = new List<int>
            {
                99, 34,5, 6, 9
            };

            var maxCount = (new List<List<int>> { list1, list2, list3 }).OrderByDescending(x => x.Count()).First().Count;

            Console.WriteLine("\nMax count of the 3 List<int> is: " + maxCount.ToString() + "\n");
        }

        // ReSharper disable once InconsistentNaming
        public static void GetTypesOfGenericMethodParameters<T, X>(T param1, X param2) where T : class where X : struct
        {
            Console.WriteLine($"\nType of param1 is: {param1.GetType()}. Type of param2 is: {param2.GetType()}");
        }

        public static T GetDefault<T>()
        {
            /*
             int defx = this.GetDefault<int>(); //will assign 0
            char defy = this.GetDefault<char>(); // will assign null('\0')
            object defz = this.GetDefault<object>(); // will assign null
             */
            return default(T);
        }

        public static void PrintEvenNumbersUsingFunc()
        {
            Console.WriteLine("\nPrinting Even numbers using Func<int, bool>()\n");

            // ReSharper disable once UnusedVariable
            Func<int, bool> isEven = delegate(int x)
            {
                return x % 2 == 0;
            };

            Func<int, bool> isEvenNumer = x => (x % 2 == 0);

            var output = new List<int>();

            for (int i = 0; i <= 20; i++)
            {
                if (isEvenNumer(i))
                {
                    output.Add(i);
                }
            }

            Console.WriteLine($"\n{string.Join(",", output)}\n");
        }

        public static void ImplementGenerateStrings()
        {
            var today = DateTime.Today;
            var seq = GenerateStrings(() =>
            {
                today += TimeSpan.FromDays(7);
                return today;
            });

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in seq.OrderBy(x => x.Date).Take(10).Skip(3))
            {
                Console.WriteLine(item.ToString(CultureInfo.InvariantCulture));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in seq.Where(x => x.Date == DateTime.Today.AddDays(42)))
            {
                Console.WriteLine(item.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static IEnumerable<T> GenerateStrings<T>(Func<T> itemGenerator)
        {
            int i = 0;
            while (i++ < 10)
            {
                yield return itemGenerator();
            }
        }

        public static void FindDuplicates()
        {
            var list = new List<MyDuplicateFindingClass>
            {
                 new MyDuplicateFindingClass{ Id = 1, CallLabel = "Func1", DisplayOrder = 10, IsSelected = true},
                 new MyDuplicateFindingClass{ Id = 2, CallLabel = "Func1", DisplayOrder = 10, IsSelected = true},
                 new MyDuplicateFindingClass{ Id = 3, CallLabel = "Func2", DisplayOrder = 20, IsSelected = true},
                 new MyDuplicateFindingClass{ Id = 4, CallLabel = "Func2", DisplayOrder = 20, IsSelected = true}
            };

            // ReSharper disable once IdentifierTypo
            // ReSharper disable once UnusedVariable
            var duplicateOnDispOrder = list.GroupBy(i => new { i.DisplayOrder, i.CallLabel, i.IsSelected })
                  .Where(g => g.Count() > 1)
                  .Select(g => g.Key).ToList();

            // ReSharper disable once UnusedVariable
            var duplicateOnDisplayOrderAndCallLabel = list.GroupBy(i => new { i.DisplayOrder, i.CallLabel })
                              .Where(g => g.Count() > 2)
                              .Select(g => g.Key).ToList();

            var theList = new List<string>() { "Alpha", "Alpha", "Beta", "Gamma", "Delta" };

            theList.GroupBy(txt => txt)
                    .Where(grouping => grouping.Count() > 1)
                    .ToList()
                    .ForEach(groupItem => Console.WriteLine("{0} duplicated {1} times with these values: {2}",
                                                             groupItem.Key,
                                                             groupItem.Count(),
                                                             string.Join(", ", groupItem.ToArray())));
        }

        private class MyDuplicateFindingClass
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            public string CallLabel { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsSelected { get; set; }
        }
    }

    public class MyClass : IDisposable
    {
        private readonly DataTable _myTable;
        private bool _disposed;

        public MyClass()
        {
            _myTable = new DataTable();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _myTable.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

#region Theory: InterviewQuestions

/*
 * Database Questions:
 * Q1 - Display data as a comma separated list.
    SELECT LISTAGG(A.COUNTRY_NAME, ', ') WITHIN GROUP(ORDER BY A.COUNTRY_ID) AS COUNTRY_LIST
    FROM COUNTRIES A
    WHERE ROWNUM <= 6;
    -----------------------------
 * Q2 - Get 3rd highest salary
    SELECT * FROM ( SELECT e.*, ROW_NUMBER() OVER (ORDER BY sal DESC) rn FROM emp e ) WHERE rn = 3;
    -----------------------------
 * Q3- Compare two tables

    with table1(column_a, column_b) as (
    select 1, 2 from dual union all
    select 2, 2 from dual union all
    select 3, null from dual union all
    select null, 4 from dual union all
    select null, null from dual
    )
    SELECT
    COLUMN_A,
    COLUMN_B,
    CASE
    WHEN COLUMN_A <> COLUMN_B THEN 'Not OK'
    WHEN column_a is null and column_b is null then 'both NULL'
    WHEN column_a is null then 'A null'
    WHEN column_b is null then 'B null'
    ELSE 'OK' END AS Status
    FROM Table1
    -----------------------------
    .NET:

    Q. Difference between ToString()/Convert.ToString()?

    string s;
    object o = null;
    s = o.ToString();
    //returns a null reference exception for s.

    string s;
    object o = null;
    s = Convert.ToString(o);
    //returns an empty string for s and does not throw an exception.
    -----------------------------
    Q. Difference between Document.Ready()/window.onload()
    The document.ready() event occurs when all HTML documents have been loaded,
    but window.onload() occurs when all content (including images) has been loaded.
    So, generally the document.ready() event fires first.

    -----------------------------
    Q. How would you call a combo box (of asp.net control) changed event from jQuery?
    __doPostBack('ddlEmployeeNames', <parameters>)

    -----------------------------
    Q. Why to use Html.Partial in MVC?
    This method is used to render the specified partial view as an HTML string.
    This method does not depend on any action methods.
    We can use this like --> @Html.Partial(“TestPartialView”)

    --------------------------------------------------
    Q. What is Html.RenderPartial?
    Result of the method — “RenderPartial” is directly written to the HTML response.
    This method does not return anything (void).
    This method also does not depend on action method.

    --------------------------------------------------
    Q. MVC action filter names/Order:
    The filter order would be like:
    Authorization filters
    Action filters
    Response filters
    Exception filters

    --------------------------------------------------
    Q. Describe boxing and unboxing?
    Boxing is an implicit conversion of a value type to the type object or to any interface type implemented by the value type.
    Boxing a value type creates an object instance containing the value and stores it on the heap.
    Example:
      int x = 101;
      object o = x;  // boxing value of x into object o

      o = 999;
      x = (int)o;    // unboxing value of o into integer x

     --------------------------------------------------
     Q. Write a generic method with 2 parameters (of different types) that writes the type of the parameters to the console.
     -- GetTypesOfGenericMethodParameters()

    --------------------------------------------------
    Q. What are constraints in C#? Where do we use? Order of constraints?
    Constraints are to specify which type of placeholder type with the generic class is allowed.
    Constraints can be applied using the where keyword. A generic class or method can have muliple constriants.
    Order of constraints are class or struct, interfaces and new() comes at the end.
    --------------------------------------------------
    Q. what is enum?
    enum is a collection of constants.

    --------------------------------------------------
    Q. what is "Default" keyword?
    This keyword returns the default value of type parameter. Every reference and value type has a default value.
    This value is returned by the default(Type) expression.
    Default is most useful for writing generic classes.

    --------------------------------------------------
    Q. Difference between Class and Struct?
    In C#, a structure is a value type data type.
    It helps you to make a single variable hold related data of various data types.
    The struct keyword is used for creating a structure. Structures are used to represent a record.
    The struct statement defines a new data type, with more than one member for your program.

    Classes are reference types and structs are value types.
    Structures do not support inheritance.
    Structures cannot have default constructor.

    ----------------------------------------------------
    Q.Can a method return multiple values? how?

    -----------------------------------------------------
    Q.Difference between var & dynamic? Example?

 */

#endregion Theory: InterviewQuestions