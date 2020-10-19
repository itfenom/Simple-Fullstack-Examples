using System.Windows.Media;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.DataBinding
{
    public class DataBindingViewModel : PropertyChangedBase
    {
        public override string Title => "DataBinding Example";

        private Brush _contentColor = Brushes.Green;

        public Brush ContentColor
        {
            get => _contentColor;
            set
            {
                if (value != _contentColor)
                {
                    SetPropertyValue(ref _contentColor, value);
                }
            }
        }

        public DelegateCommand<Brush> ChangeColorCommand { get; protected set; }

        public ICommand CloseCommand { get; }

        private string _employeeName;

        public string EmployeeName
        {
            get => _employeeName;
            set
            {
                if (value != _employeeName)
                {
                    SetPropertyValue(ref _employeeName, value);
                    SetEmployeeDetails();
                }
            }
        }

        private string _employeeTitle;

        public string EmployeeTitle
        {
            get => _employeeTitle;
            set
            {
                if (value != _employeeTitle)
                {
                    SetPropertyValue(ref _employeeTitle, value);
                    SetEmployeeDetails();
                }
            }
        }

        private string _employeeGender;

        public string EmployeeGender
        {
            get => _employeeGender;
            set
            {
                if (value != _employeeGender)
                {
                    SetPropertyValue(ref _employeeGender, value);
                }
            }
        }

        private string _employeeDetail;

        public string EmployeeDetail
        {
            get => _employeeDetail;
            set
            {
                SetPropertyValue(ref _employeeDetail, value);
            }
        }

        private void SetEmployeeDetails()
        {
            EmployeeDetail = $"{EmployeeName} - {EmployeeTitle}";
        }

        private void ChangeColor(Brush color)
        {
            ContentColor = color;
        }

        private bool ChangeColor_CanExecute(object arg)
        {
            if (arg != _contentColor)
            {
                return true;
            }

            return false;
        }

        private void Close()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }

        public DataBindingViewModel()
        {
            EmployeeName = "Kashif Mubarak";
            EmployeeTitle = "Software Engineer";
            EmployeeGender = "Male";

            EmployeeDetail = $"{EmployeeName} - {EmployeeTitle}";

            ChangeColorCommand = new DelegateCommand<Brush>(ChangeColor, ChangeColor_CanExecute);
            CloseCommand = new DelegateCommand(() => Close());
        }
    }
}