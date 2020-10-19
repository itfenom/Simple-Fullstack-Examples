using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Autofac;
using Playground.ConsoleApp.AutoFacEx;
using Playground.ConsoleApp.AutoMapperEx;
using Playground.ConsoleApp.BankingEx;
using Playground.ConsoleApp.BawDataViewer;
using Playground.ConsoleApp.DelegateEx;
using Playground.ConsoleApp.DotNetInterview;
using Playground.ConsoleApp.FinalProject;
using Playground.ConsoleApp.GenericEx;
using Playground.ConsoleApp.Grades;
using Playground.Core.Utilities;

namespace Playground.ConsoleApp
{
    internal class Program
    {
        private static bool useProdDB = false;

        private static void InitializeSetting()
        {
            Core.CoreConfig.CreateApplicationRelatedFolders("ConsoleApp");

            if (useProdDB)
            {
                Console.WriteLine("***************  USING PRODUCTION DATABASE ********************");
                Core.CoreConfig.UseDevDatabase = () => false;
            }
            else
            {
                Console.WriteLine("***************  USING DEV DATABASE ********************");
                Core.CoreConfig.UseDevDatabase = () => true;
            }
        }

        public static void Main(string[] args)
        {
            InitializeSetting();

            #region oiStructure
            //var oiStructure = new OiBinaryOutput("EG5520", @"C:\Users\kashi\Documents\GitHub\Playground\Playground.ConsoleApp\ConsoleAppResources\OIstructure.mat");
            //var mlStructure = oiStructure.ProcessOiOutput();

            #endregion

            #region Auto-Mapper
            /*
            Console.WriteLine("Auto-Mapper example begin:");
            var autoMapperEx = new AutoMapperExample();
            autoMapperEx.CreateObjectMappingOne();
            autoMapperEx.CreateObjectMappingTwo();
            Console.WriteLine("Auto-Mapper Example end!\n");
            */
            #endregion

            #region AutoFac dependency injector
            /*
            Console.WriteLine("AutoFac dependency injector example begin:");
            var memos = new List<Memo>();
            memos.Add(new Memo { Title = "Release Autofac 1.1", DueAt = DateTime.Now.AddDays(10) });
            memos.Add(new Memo { Title = "Write CodeProject Article", DueAt = DateTime.Now.AddDays(-2) });
            memos.Add(new Memo { Title = "Release Autofac 1.2", DueAt = DateTime.Now.AddDays(-10) });

            var builder = new ContainerBuilder();
            builder.Register(c => new MemoChecker(c.Resolve<List<Memo>>(), c.Resolve<IMemoDueNotifier>()));
            builder.Register(c => new PrintingNotifier(c.Resolve<TextWriter>())).As<IMemoDueNotifier>();

            builder.RegisterInstance(memos).As<List<Memo>>();
            builder.RegisterInstance(Console.Out).As<TextWriter>().ExternallyOwned();

            using (var container = builder.Build())
            {
                container.Resolve<MemoChecker>().CheckNow();
            }
            Console.WriteLine("AutoFac dependency injector example end.\n");
            */
            #endregion

            #region BankingProject
            /*
            Console.WriteLine("Banking Example begin:");
            var bank = new Bank();
            bank.StartProcessing();
            Console.WriteLine("Banking Example end.\n");
            */
            #endregion

            #region FinalProject
            /*
            Console.WriteLine("Final Project Example Begin:");
            var myFinalProject = new MyFinalProject();
            myFinalProject.Start();
            Console.WriteLine("Final Project Example End.\n");
            */
            #endregion

            #region Manage Grades
            /*
            Console.WriteLine("Grade example begin: ");
            var manageGrade = new ManageGrade();
            manageGrade.Start();
            Console.WriteLine("Grade example end.\n");
            */
            #endregion

            #region DotNet Interview Questions
            /*
            Console.WriteLine("Dot-Net Interview Questions begin:");
            InterviewQuestions.ImplementGenerateStrings();
            InterviewQuestions.PrintHighestNumberInArrays();
            var n = 5;
            Console.WriteLine($"\nRecursiveMethod output is {InterviewQuestions.RecursiveMethod(n)}, when n = {n}");
            Console.WriteLine($"\nReversing a string 'I am at Qorvo!' {InterviewQuestions.ReverseString("I am at Qorvo!")}");
            InterviewQuestions.PrintEvenNumbers();
            Console.WriteLine($"\nCalculateAge for 03/26/1986 is: {InterviewQuestions.CalculateAge(new DateTime(1986, 3, 26))}");
            InterviewQuestions.CountCharactersInString("Qorvo");
            InterviewQuestions.GetMaxCountInListUsingLinq();
            InterviewQuestions.FindDuplicates();

            // ReSharper disable once RedundantDelegateCreation
            SampleDelegate delegateObj = new SampleDelegate(InterviewQuestions.CheckEven);
            // ReSharper disable once RedundantDelegateCreation
            delegateObj += new SampleDelegate(InterviewQuestions.SquareNumber);
            delegateObj(25);

            InterviewQuestions.GetTypesOfGenericMethodParameters("This is string", 425);
            InterviewQuestions.PrintEvenNumbersUsingFunc();
            Console.WriteLine("Dot-Net Interview Questions end.\n");
            */
            #endregion

            #region Generic usage
            /*
            Console.WriteLine("Use of Generics begin:");
            var genericList = new MyGenericList<string>();
            genericList.Add("Hello");
            genericList.Add("Weired");
            genericList.Add("How's");
            genericList.Add("going...?");

            // ReSharper disable once UnusedVariable
            var result = genericList.AreEqual("Hi", "Hi");
            Console.WriteLine("Use of Generics end.\n");
            */
            #endregion

            #region Other Misc Examples
            //OtherMisc();
            #endregion

            #region Delegates in Dictionary
            //DictionaryDelegates();
            #endregion

            //End
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static void DictionaryDelegates()
        {

            var reporter = new Reporter();
            var reportDic = new Dictionary<ReportType, Func<int, string>>();

            reportDic.Add(ReportType.Daily, reporter.GetDailyReport);
            reportDic.Add(ReportType.Weekly, reporter.GetWeeklyReport);
            reportDic.Add(ReportType.Monthly, reporter.GetMonthlyReport);
            reportDic.Add(ReportType.Annually, reporter.GetAnnualReport);

            var report = reportDic[ReportType.Daily](60);
            Console.WriteLine(report);

        }

        private static void InsertIntoSqlServer()
        {
            var oConn = new SqlConnection();
            // ReSharper disable once UnusedVariable
            var oCmd = new SqlCommand();
            oConn.ConnectionString = "Data Source=DFWKMUBARAK-L;Initial Catalog=Seraph;Persist Security Info=True;User ID=test;Password=Superman123$";
            oConn.Open();

            //for (int i = 1; i < 6001; i++)
            //{
            //    var empName = "Employee_" + i;
            //    var empEmail = "employee_" + i + "@test.com";
            //    var empSalary = Convert.ToString(50000 + i);
            //    var empHireDate = Convert.ToDateTime("12/12/2004").AddDays(i);

            //    var sql = "INSERT INTO EMPLOYEE(EMP_NAME, EMP_EMAIL, EMP_PHONE, EMP_SALARY, EMP_HIRE_DATE, EMP_GENDER, EMP_PHOTO_ID, EMP_IS_ACTIVE)"
            //         + " VALUES('"
            //         + empName + "', '"
            //         + empEmail + "', '(000)000-0000', "
            //         + empSalary + ", '"
            //         + empHireDate.ToString("MM/dd/yyyy") + "', 'M', NULL, 1)";

            //    oCmd.Connection = oConn;
            //    oCmd.CommandText = sql;
            //    oCmd.CommandType = CommandType.Text;
            //    oCmd.ExecuteNonQuery();
            //}

            oConn.Close();
            oConn.Dispose();
        }

        private static void OtherMisc()
        {
            var tupleExample = Tuple.Create(1, "Billy", "Haynes", 4, 5, 6, 7, Tuple.Create(8, 9, 10));
            var val1 = tupleExample.Item5;
            var val2 = tupleExample.Rest.Item1.Item2;
            var result = val1 + val2;
            Console.WriteLine("\nAdding 5th and 9th element in Tuple: " + result);

            //Machine Name
            Console.WriteLine("\nThis Machine Name is: " + Environment.MachineName);

            //Assembly path:
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(currentAssembly);
            Console.WriteLine("\nCurrent Assembly path is: " + path);

            //IsNumeric
            Console.WriteLine("Enter number to find out if it is numeric:");
            var inputVal = Console.ReadLine();

            if (BooleanOperations.IsNumeric(inputVal))
            {
                Console.WriteLine("\n" + inputVal + " is numeric.");
            }
            else
            {
                Console.WriteLine("\n" + inputVal + " is NOT numeric.");
            }

            //Remove last index in a string
            var beforeRemovingComma = "'" + "Some value" + "'" + ',';
            Console.WriteLine("\nBefore: " + beforeRemovingComma);
            var afterRemovingComma = StringOperations.RemoveLastIndexOfCharacter(beforeRemovingComma, ',');
            Console.WriteLine("After: " + afterRemovingComma);

            //
        }
    }
}
