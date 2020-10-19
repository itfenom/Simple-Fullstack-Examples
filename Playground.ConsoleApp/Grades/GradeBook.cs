using System;
using System.Collections.Generic;

namespace Playground.ConsoleApp.Grades
{
    public delegate void NameChangedDelegate(object sender, NameChangedEventArgs args);

    public class GradeBook : GradeTracker
    {
        // ReSharper disable once InconsistentNaming
        protected List<float> _grades;

        public GradeBook()
        {
            _grades = new List<float>();
        }

        public override void AddGrade(float grade)
        {
            _grades.Add(grade);
        }

        public override GradeStatistics ComputeStatistics()
        {
            var stats = new GradeStatistics();
            float sum = 0;

            foreach (float grade in _grades)
            {
                sum += grade;

                stats.HighestGrade = Math.Max(grade, stats.HighestGrade);
                stats.LowestGrade = Math.Min(grade, stats.LowestGrade);
            }

            stats.AverageGrade = (sum / _grades.Count);

            return stats;
        }
    }
}