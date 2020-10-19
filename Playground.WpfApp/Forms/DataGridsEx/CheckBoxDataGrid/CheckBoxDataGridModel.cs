using System;
using System.ComponentModel;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.CheckBoxDataGrid
{
    public class CheckBoxDataGridModel : ValidationPropertyChangedBase, IEditableObject
    {
        private string _employeeName;

        public string EmployeeName
        {
            get => _employeeName;
            set => SetPropertyValue(ref _employeeName, value);
        }

        private bool _canEditEmployee;

        public bool CanEditEmployee
        {
            get => _canEditEmployee;
            set => SetPropertyValue(ref _canEditEmployee, value);
        }

        private bool _isEmployeeChecked;

        public bool IsEmployeeChecked
        {
            get => _isEmployeeChecked;
            set
            {
                SetPropertyValue(ref _isEmployeeChecked, value);
                _onSelectionChanged?.Invoke(value);
            }
        }

        private readonly Action<bool> _onSelectionChanged;

        public CheckBoxDataGridModel()
        {

        }

        //This constructor used in SelectAllDataGrid
        public CheckBoxDataGridModel(string name, bool isChecked, Action<bool> onSelectionChanged)
        {
            _employeeName = name;
            _isEmployeeChecked = isChecked;
            _onSelectionChanged = onSelectionChanged;
        }

        #region IEditableObject implementation

        private bool _inEdit;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
        }

        #endregion IEditableObject implementation
    }

    public class ActionCommand
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
        public string ParameterText { get; set; }
    }
}