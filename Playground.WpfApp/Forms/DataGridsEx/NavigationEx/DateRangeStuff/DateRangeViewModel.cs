using System;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx.DateRangeStuff
{
    public class DateRangeViewModel : PropertyChangedBase
    {
        private DateTime? _endDate;
        private DateRange? _range;
        private DateTime? _startDate;
        private IDateTimeProvider _dateTimeProvider;
        public bool UserCancelled;

        public DateRangeViewModel(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            //StartDate = DateTime.Today;
            //EndDate = DateTime.Today;

            ApplyCommand = new DelegateCommand(() => Apply(), () => CanApply);
            CancelCommand = new DelegateCommand(() => Cancel());
            ClearCommand = new DelegateCommand(() => Clear());
            SelectLastSevenDaysCommand = new DelegateCommand(() => SelectLastSevenDays());
            SelectThisMonthCommand = new DelegateCommand(() => SelectThisMonth());
            SelectLastThirtyDaysCommand = new DelegateCommand(() => SelectLastThirtyDays());
            SelectLastTwentyYearsCommand = new DelegateCommand(() => SelectLastTwentyYears());

            PropertyChanged += DateRangeViewModel_PropertyChanged;
        }

        private void DateRangeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                ApplyCommand.RaiseCanExecuteChanged();
            }
        }

        #region Commands

        public ICommand SelectLastSevenDaysCommand { get; }

        private void SelectLastSevenDays()
        {
            var today = _dateTimeProvider.Today();
            StartDate = today.AddDays(-7);
            EndDate = today;
        }

        public ICommand SelectLastThirtyDaysCommand { get; }

        private void SelectLastThirtyDays()
        {
            var today = _dateTimeProvider.Today();
            StartDate = today.AddDays(-30);
            EndDate = today;
        }

        public ICommand SelectThisMonthCommand { get; }

        private void SelectThisMonth()
        {
            var today = _dateTimeProvider.Today();
            StartDate = new DateTime(today.Year, today.Month, 1);
            EndDate = StartDate.Value.AddMonths(1).AddTicks(-1);
        }

        public ICommand SelectLastTwentyYearsCommand { get; }

        private void SelectLastTwentyYears()
        {
            var today = _dateTimeProvider.Today();
            StartDate = new DateTime(today.AddYears(-20).Year, today.Month, 1);
            EndDate = today;
        }

        public DelegateCommand ApplyCommand { get; }

        private bool CanApply
        {
            get
            {
                if (StartDate == null || EndDate == null) return false;
                // ReSharper disable once ArrangeRedundantParentheses
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                return (StartDate.HasValue && EndDate.HasValue && StartDate.Value <= EndDate.Value);
            }
        }

        private void Apply()
        {
            Range = new DateRange(StartDate.Value, EndDate.Value);
            CloseWindow();
        }

        public ICommand CancelCommand { get; }

        private void Cancel()
        {
            if (Range == null)
            {
                StartDate = null;
                EndDate = null;
            }
            else
            {
                StartDate = Range.Value.StartDate;
                EndDate = Range.Value.EndDate;
            }

            UserCancelled = true;
            CloseWindow();
        }

        public ICommand ClearCommand { get; }

        private void Clear()
        {
            Range = null;
        }

        private void CloseWindow()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }
        #endregion

        #region Properties
        public DateTime? StartDate
        {
            get => _startDate;
            set => SetPropertyValue(ref _startDate, value);
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set => SetPropertyValue(ref _endDate, value);
        }

        public DateRange? Range
        {
            get => _range;
            set => SetPropertyValue(ref _range, value);
        }
        #endregion
    }
}
