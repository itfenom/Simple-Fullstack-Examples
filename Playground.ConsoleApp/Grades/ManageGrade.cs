using System;
using System.Globalization;

namespace Playground.ConsoleApp.Grades
{
    public class ManageGrade
    {
        public void Start()
        {
            GradeTracker book = new ThrowAwayGradeBook();

            // ReSharper disable once RedundantDelegateCreation
            book.NameChanged += new NameChangedDelegate(OnNameChanged);

            try
            {
                Console.WriteLine("Please enter a name: ");
                book.Name = Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            book.AddGrade(75);
            book.AddGrade(91);
            book.AddGrade(89.5f);

            var stats = book.ComputeStatistics();

            WriteToConsole(book.Name, stats.HighestGrade, stats.LowestGrade, stats.AverageGrade, stats.LetterGrade, stats.Description);
        }

        private static void OnNameChanged(object sender, NameChangedEventArgs args)
        {
            Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} - Name changed from {args.ExistingName} to {args.NewName}");
        }

        private static void WriteToConsole(string name, double highestGrade, double lowestGrade, double averageGrade, string gradeResult, string description)
        {
            Console.WriteLine($"Name: {name}\nHighest Grade: {highestGrade}\nLowest Grade: {lowestGrade}\nAverage Grade: {averageGrade}\nGradeResult: {gradeResult}\nGrade Description: {description}");
        }
    }
}