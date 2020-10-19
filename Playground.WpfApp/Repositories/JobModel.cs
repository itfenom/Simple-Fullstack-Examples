using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Repositories
{
    public class JobModel : PropertyChangedBase
    {
        private string _jobId;

        public string JobId
        {
            get => _jobId;
            set => SetPropertyValue(ref _jobId, value);
        }

        private string _jobTitle;

        public string JobTitle
        {
            get => _jobTitle;
            set => SetPropertyValue(ref _jobTitle, value);
        }

        private string _minSalary;

        public string MinSalary
        {
            get => _minSalary;
            set => SetPropertyValue(ref _minSalary, value);
        }

        private string _maxSalary;

        public string MaxSalary
        {
            get => _maxSalary;
            set => SetPropertyValue(ref _maxSalary, value);
        }
    }
}
