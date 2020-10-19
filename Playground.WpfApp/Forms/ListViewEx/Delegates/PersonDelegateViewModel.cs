using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.ListViewEx.Delegates
{
    public class PersonDelegateViewModel : ValidationPropertyChangedBase
    {
        public override string Title => "Delegate Example with Func<T, TResult> and Action<T>";

        private readonly IDemoEmpRepository _repository;

        private Func<DemoEmployeeModel, string> _formatPersonUsingFunc;

        private Action<List<DemoEmployeeModel>> _formatPersonUsingAction;

        private ObservableCollection<DemoEmployeeModel> _personDelegateList;

        public ObservableCollection<DemoEmployeeModel> PersonDelegateList
        {
            get => _personDelegateList;
            set => SetPropertyValue(ref _personDelegateList, value);
        }

        private ObservableCollection<string> _outputStringList;

        public ObservableCollection<string> OutputStringList
        {
            get => _outputStringList;
            set => SetPropertyValue(ref _outputStringList, value);
        }

        private bool _isExpandedStringHandling;

        public bool IsExpandedStringHandling
        {
            get => _isExpandedStringHandling;
            set
            {
                if (value)
                {
                    IsExpandedActions = false;
                }
                SetPropertyValue(ref _isExpandedStringHandling, value);
            }
        }

        private bool _isExpandedActions;

        public bool IsExpandedActions
        {
            get => _isExpandedActions;
            set
            {
                if (value)
                {
                    IsExpandedStringHandling = false;
                }
                SetPropertyValue(ref _isExpandedActions, value);
            }
        }

        public PersonDelegateViewModel()
        {
            _repository = new DemoEmpRepository();
            var personDelegateList = GetPersonDelegateList();
            _personDelegateList = new ObservableCollection<DemoEmployeeModel>(personDelegateList);
            _outputStringList = new ObservableCollection<string>();
            _isExpandedStringHandling = true;
            _isExpandedActions = false;
            ProcessDataCommand = new DelegateCommand(() => ProcessData());
        }

        private List<DemoEmployeeModel> GetPersonDelegateList()
        {
            return _repository.GetAllEmployees();
        }

        public ICommand ProcessDataCommand { get; }

        private void ProcessData()
        {
            OutputStringList.Clear();
            if (IsExpandedStringHandling)
            {
                AssignFuncDelegates();
                foreach (var item in _personDelegateList)
                {
                    AddToOutputList(item.ToString(_formatPersonUsingFunc));
                }
            }
            else if (IsExpandedActions)
            {
                //formatPersonUsingAction = null;
                AssignActions();
                var persons = GetPersonDelegateList();
                _formatPersonUsingAction.Invoke(persons);
            }
        }

        private void AssignFuncDelegates()
        {
            if (DefaultRdoBtn)
            {
                _formatPersonUsingFunc = p => p.ToString();
            }
            else if (LastNameRdoBtn)
            {
                _formatPersonUsingFunc = p => p.LastName.ToUpper();
            }
            else if (FirstNameRdoBtn)
            {
                _formatPersonUsingFunc = p => p.FirstName.ToLower();
            }
            else if (FullNameRdoBtn)
            {
                _formatPersonUsingFunc = p => $"{p.LastName}, {p.FirstName}";
            }
            else
            {
                _formatPersonUsingFunc = p => p.ToString();
            }
        }

        private void AssignActions()
        {
            if (AverageRatingChkBox)
            {
                _formatPersonUsingAction += p =>
                    AddToOutputList("Average Rating: " + p.Average(x => x.Rating).ToString("#.#"));
            }

            if (EarliestStartDateChkBox)
            {
                _formatPersonUsingAction += p =>
                    AddToOutputList("Earlier start-date: " + p.Min(x => x.StartDate).ToString("d"));
            }

            if (BestCommanderChkBox)
            {
                _formatPersonUsingAction += p => AddToOutputList(p.OrderByDescending(r => r.Rating).First().ToString());
                //MessageBox.Show(p.OrderByDescending(r => r.Rating).First().ToString());
            }

            if (FirstLettersChkBox)
            {
                _formatPersonUsingAction += p =>
                {
                    p.ForEach(c => OutputStringList.Add("First letter in Last-Name: " + c.LastName[0] + "\n"));
                    //p.ForEach(c => Console.Write(c.LastName[0]));
                    //Console.WriteLine();
                };
            }
        }

        private void AddToOutputList(string item)
        {
            OutputStringList.Add(item);
        }

        #region Radio-Buttons

        private bool _defaultRdoBtn;

        public bool DefaultRdoBtn
        {
            get => _defaultRdoBtn;
            set => SetPropertyValue(ref _defaultRdoBtn, value);
        }

        private bool _lastNameRdoBtn;

        public bool LastNameRdoBtn
        {
            get => _lastNameRdoBtn;
            set => SetPropertyValue(ref _lastNameRdoBtn, value);
        }

        private bool _firstNameRdoBtn;

        public bool FirstNameRdoBtn
        {
            get => _firstNameRdoBtn;
            set => SetPropertyValue(ref _firstNameRdoBtn, value);
        }

        private bool _fullNameRdoBtn;

        public bool FullNameRdoBtn
        {
            get => _fullNameRdoBtn;
            set => SetPropertyValue(ref _fullNameRdoBtn, value);
        }

        #endregion Radio-Buttons

        #region Check-Boxes

        private bool _averageRatingChkBox;

        public bool AverageRatingChkBox
        {
            get => _averageRatingChkBox;
            set => SetPropertyValue(ref _averageRatingChkBox, value);
        }

        private bool _earliestStartDateChkBox;

        public bool EarliestStartDateChkBox
        {
            get => _earliestStartDateChkBox;
            set => SetPropertyValue(ref _earliestStartDateChkBox, value);
        }

        private bool _bestCommanderChkBox;

        public bool BestCommanderChkBox
        {
            get => _bestCommanderChkBox;
            set => SetPropertyValue(ref _bestCommanderChkBox, value);
        }

        private bool _firstLettersChkBox;

        public bool FirstLettersChkBox
        {
            get => _firstLettersChkBox;
            set => SetPropertyValue(ref _firstLettersChkBox, value);
        }

        #endregion Check-Boxes

        protected override void DisposeManagedResources()
        {
            _formatPersonUsingFunc = null;
            _formatPersonUsingAction = null;
            _personDelegateList = null;
            _outputStringList = null;
        }
    }
}