using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.ItemsControlEx.Ex1
{
    public class ItemsControlViewModel : PropertyChangedBase
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        public override string Title => "Items Control Example!";

        private bool _isBusy;

        public new bool IsBusy
        {
            get => _isBusy;
            set => SetPropertyValue(ref _isBusy, value);
        }

        private List<ItemsControlCountry> _countryList;

        public ItemsControlViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            OkCommand = new DelegateCommand(() => OnOk(), () => CanExecuteOkCommand);
            CreateDropDownsCommand = new DelegateCommand(() => OnCreateDropDowns());
            LoadData();

            PropertyChanged += ItemsControlViewModel_PropertyChanged;
        }

        private void ItemsControlViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _okCommand.RaiseCanExecuteChanged();
            }
        }

        private void LoadData()
        {
            _countryList = new List<ItemsControlCountry>();
            _countryList.Add(new ItemsControlCountry { CountryId = 1, CountryName = "USA" });
            _countryList.Add(new ItemsControlCountry { CountryId = 2, CountryName = "Canada" });
            _countryList.Add(new ItemsControlCountry { CountryId = 3, CountryName = "Mexico" });
            _countryList.Add(new ItemsControlCountry { CountryId = 4, CountryName = "Brazil" });
        }

        private int _numberOfDropDowns;

        public int NumberOfDropDowns
        {
            get => _numberOfDropDowns;
            set => SetPropertyValue(ref _numberOfDropDowns, value);
        }

        private ObservableCollection<ItemsControlCountriesList> _comboBoxList;

        public ObservableCollection<ItemsControlCountriesList> ComboBoxList
        {
            get => _comboBoxList;
            set => SetPropertyValue(ref _comboBoxList, value);
        }

        public ICommand CreateDropDownsCommand { get; }

        private void OnCreateDropDowns()
        {
            if (_numberOfDropDowns == 0)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Create Drop Downs", "Please specify number of drop - down(s) and try again!");
                return;
            }

            int index = 1;
            int counter = NumberOfDropDowns;
            var items = new List<ItemsControlCountriesList>();

            while (counter > 0)
            {
                items.Add(new ItemsControlCountriesList { DropDownNumberLabel = $"Select Country {index}: ", Countries = _countryList });
                counter--;
                index++;
            }

            _comboBoxList = new ObservableCollection<ItemsControlCountriesList>(items);
            NotifyPropertyChanged("ComboBoxList");
        }

        private DelegateCommand _okCommand;

        public DelegateCommand OkCommand
        {
            get => _okCommand;
            set => SetPropertyValue(ref _okCommand, value);
        }

        private bool CanExecuteOkCommand => (_comboBoxList != null && _comboBoxList.Count > 0);

        private void OnOk()
        {
            var sb = new StringBuilder();
            var userSelection = new List<int>();
            int counter = 0;

            foreach (var item in _comboBoxList)
            {
                if (item.SelectedItemValue == 0)
                {
                    counter++;
                }

                userSelection.Add(item.SelectedItemValue);
            }

            if (counter == userSelection.Count)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Create Drop Downs", "* No column selected from the drop-down!\n\nPlease make column selection and try again.");
                return;
            }

            counter = 0;

            //UIHelper.SetBusyState();
            IsBusy = true;
            Thread.Sleep(3000);

            foreach (var item in _comboBoxList)
            {
                var countryId = (from c in _countryList where c.CountryId == item.SelectedItemValue select c.CountryId).FirstOrDefault();
                var countryName = (from c in _countryList where c.CountryId == item.SelectedItemValue select c.CountryName).FirstOrDefault();

                sb.AppendLine($"Selection: {(counter + 1)}\nCountry Id: {countryId}\tCountry Name: {countryName}\n\n");

                counter++;
            }

            IsBusy = false;
            //UIHelper.RemoveBusyState();
            _dialogCoordinator.ShowMessageAsync(this, "Create Drop Downs", sb.ToString());
        }

        public ICommand CloseCommand
        {
            get { return new DelegateCommand(() => Close()); }
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
    }

    public class ItemsControlCountriesList
    {
        public int SelectedItemValue { get; set; }
        public string DropDownNumberLabel { get; set; }
        public List<ItemsControlCountry> Countries { get; set; }
    }

    public class ItemsControlCountry
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}