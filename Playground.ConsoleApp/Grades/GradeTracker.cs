using System;

namespace Playground.ConsoleApp.Grades
{
    public abstract class GradeTracker : IGradeTracker
    {
        // ReSharper disable once InconsistentNaming
        protected string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }

                if (_name != value)
                {
                    var args = new NameChangedEventArgs {NewName = value, ExistingName = _name};

                    // ReSharper disable once PossibleNullReferenceException
                    NameChanged(this, args);
                }
                _name = value;
            }
        }

        public event NameChangedDelegate NameChanged;

        public abstract GradeStatistics ComputeStatistics();

        public abstract void AddGrade(float grade);
    }
}