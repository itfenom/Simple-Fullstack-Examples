using System;
using System.ComponentModel;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.CheckBoxDataGrid
{
    public class SelectRoleDialogViewModel : ValidationPropertyChangedBase
    {
        private bool? _dialogResultDependencyProperty;

        public bool? DialogResultDependencyProperty
        {
            get => _dialogResultDependencyProperty;
            set => SetPropertyValue(ref _dialogResultDependencyProperty, value);
        }

        private DelegateCommand _selectAndCloseCommand;

        public ICommand SelectAndCloseCommand => _selectAndCloseCommand;

        private bool CanClickOk
        {
            get
            {
                bool retVal = false;
                bool adminSelected = Enum.IsDefined(typeof(ChooseRoleEnum), "Admin");
                bool guestSelected = Enum.IsDefined(typeof(ChooseRoleEnum), "Guest");

                if (adminSelected || guestSelected)
                {
                    retVal = true;
                }

                return retVal;
            }
        }

        private void SelectAndClose()
        {
            DialogResultDependencyProperty = true;
        }

        private ChooseRoleEnum _selectedRoleEnumVal;

        public ChooseRoleEnum SelectedRoleEnumVal
        {
            get => _selectedRoleEnumVal;
            set => SetPropertyValue(ref _selectedRoleEnumVal, value);
        }

        private bool _isVisibleAdminOption;

        public bool IsVisibleAdminOption
        {
            get => _isVisibleAdminOption;
            set => SetPropertyValue(ref _isVisibleAdminOption, value);
        }

        private bool _isVisibleGuestOption;

        public bool IsVisibleGuestOption
        {
            get => _isVisibleGuestOption;
            set => SetPropertyValue(ref _isVisibleGuestOption, value);
        }

        public SelectRoleDialogViewModel(bool showAdminOptionFlag, bool showGuestOptionFlag)
        {
            _isVisibleAdminOption = showAdminOptionFlag;
            _isVisibleGuestOption = showGuestOptionFlag;

            PreSelectOneOption();

            NotifyPropertyChanged("IsVisibleAdminOption");
            NotifyPropertyChanged("IsVisibleGuestOption");

            _selectAndCloseCommand = new DelegateCommand(() => SelectAndClose(), () => CanClickOk);

            PropertyChanged += SelectRoleDialogViewModel_PropertyChanged;
        }

        private void SelectRoleDialogViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _selectAndCloseCommand.RaiseCanExecuteChanged();
            }
        }

        private void PreSelectOneOption()
        {
            if (IsVisibleAdminOption && IsVisibleGuestOption || !IsVisibleAdminOption && !IsVisibleGuestOption)
            {
                SelectedRoleEnumVal = ChooseRoleEnum.Admin;
                return;
            }

            if (IsVisibleAdminOption && !IsVisibleGuestOption)
            {
                SelectedRoleEnumVal = ChooseRoleEnum.Admin;
                return;
            }

            if (!IsVisibleAdminOption && IsVisibleGuestOption)
            {
                SelectedRoleEnumVal = ChooseRoleEnum.Guest;
            }
        }
    }
}