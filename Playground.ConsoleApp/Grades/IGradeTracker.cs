namespace Playground.ConsoleApp.Grades
{
    public interface IGradeTracker
    {
        GradeStatistics ComputeStatistics();

        void AddGrade(float grade);
    }
}