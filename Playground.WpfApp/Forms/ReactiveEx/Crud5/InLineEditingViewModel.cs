using Playground.WpfApp.Behaviors;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud5
{
    public class InLineEditingViewModel : EditableBindableBase, ICloseWindow
    {
        public override string Title => "Inline Editing in DataGrid";

        private ObservableCollection<InLineEditingModel> _allRecords;

        [CollectionItemsValid]
        public ObservableCollection<InLineEditingModel> AllRecords
        {
            get => _allRecords;
            set => this.RaiseAndSetIfChanged(ref _allRecords, value);
        }

        private InLineEditingModel _selectedRecord;

        public InLineEditingModel SelectedRecord
        {
            get => _selectedRecord;
            set => this.RaiseAndSetIfChanged(ref _selectedRecord, value);
        }

        public ICollectionView AllRecordsView { get; set; }

        public override bool IsChanged => base.IsChanged || AllRecords.OfType<EditableBindableBase>().Any(grp => grp.IsChanged);

        public InLineEditingViewModel()
        {
            _allRecords = new ObservableCollection<InLineEditingModel>();
            _allRecords.Add(new InLineEditingModel { Name = "Employee1", Age = 25 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee2", Age = 26 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee3", Age = 27 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee4", Age = 28 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee5", Age = 29 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee6", Age = 30 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee7", Age = 35 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee8", Age = 45 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee9", Age = 55 });
            _allRecords.Add(new InLineEditingModel { Name = "Employee10", Age = 65 });
            this.RaisePropertyChanged(nameof(AllRecords));

            AllRecordsView = CollectionViewSource.GetDefaultView(_allRecords);
            this.RaisePropertyChanged(nameof(AllRecordsView));

            //Delete Command
            var canDelete = this.WhenAnyValue(
                x => x.SelectedRecord,
                (InLineEditingModel m) =>
                {
                    return m != null;
                });
            DeleteCommand = ReactiveCommand.Create(() => 
            {
                if(MessageBox.Show($"Are you sure, you want to delete {SelectedRecord.Name} ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _allRecords.Remove(SelectedRecord);
                    this.RaisePropertyChanged(nameof(AllRecords));

                    _selectedRecord = null;
                    this.RaisePropertyChanged(nameof(SelectedRecord));

                    this.RaisePropertyChanged(nameof(AllRecordsView));
                }

            }, canDelete).DisposeWith(Disposables.Value);

            //Add new 
            AddNewCommand = ReactiveCommand.Create(() => 
            {
                var newItem = new InLineEditingModel
                {
                    Name = string.Empty,
                    Age = 0,
                    EditState = EditState.New
                };
                AllRecords.Add(newItem);
                ValidateObject();
                
                _selectedRecord = newItem;
                this.RaisePropertyChanged(nameof(SelectedRecord));

                AllRecordsView.MoveCurrentTo(newItem);
                this.RaisePropertyChanged(nameof(AllRecordsView));

            }).DisposeWith(Disposables.Value);

            //Save
            var canSave = this.WhenAnyValue(
                x => x.AllErrors,
                x => x.HasErrors,
                x => x.SelectedRecord,
                x => x.SelectedRecord.Name,
                x => x.SelectedRecord.Age,
                x => x.IsChanged,
                (allErr, hasErr, seletedRec, name, age, isChanged)
                =>
                {
                    var isAnyHasError = _allRecords.Any(x => x.HasErrors);
                 
                    if(isAnyHasError)
                    {
                        return false;
                    }

                    return !hasErr && allErr.Count() == 0 && HasUnsavedChanges();
                });
            SaveCommand = ReactiveCommand.Create(() => 
            {
                var isAnyHasError = _allRecords.Any(x => x.HasErrors);

                if (isAnyHasError)
                {
                    MessageBox.Show("Pleae fix validation error(s) and try again.", "Save", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var recordsToSave = _allRecords.Where(x => x.IsChanged || x.EditState == EditState.New).ToList();
                foreach (var item in recordsToSave)
                {
                    if(item.EditState == EditState.New)
                    {
                        //Save new record!
                    }
                    else if(item.IsChanged)
                    {
                        //Update the record
                    }

                    item.AcceptChanges();
                    item.EditState = EditState.NotChanged;
                }
                this.EditState = EditState.NotChanged;
                SelectedRecord = null;
            }, canSave).DisposeWith(Disposables.Value);

            //Close this window
            CancelCommand = ReactiveCommand.Create(() =>
            {
                Close?.Invoke();
            });
        }

        #region Commands
        public ICommand DeleteCommand { get; }

        public ICommand AddNewCommand { get; }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
        #endregion

        #region Filtering
        private string _nameFilter;

        public string NameFilter
        {
            get => _nameFilter;
            set => this.RaiseAndSetIfChanged(ref _nameFilter, value);
        }

        private string _ageFilter;
        public string AgeFilter
        {
            get => _ageFilter;
            set => this.RaiseAndSetIfChanged(ref _ageFilter, value);
        }

        #endregion

        #region Closing
        public Action Close { get; set; }

        public bool HasUnsavedChanges()
        {
            if (AllRecords == null) return false;

            var newOrModifiedCount = AllRecords
                .Where(x => x.IsChanged || x.EditState == EditState.New).ToList().Count;

            if (newOrModifiedCount > 0) return true;

            return false;
        }

        public bool CanClose()
        {
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show("Unsaved changes found.\nDiscard changes and close?", "Confirm Close",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        public void DisposeResources()
        {
            _allRecords = null;
            AllRecordsView = null;
            _selectedRecord = null;
        }
        #endregion
    }

    public class InLineEditingModel : EditableBindableBase
    {
        private string _name;

        [Required(ErrorMessage = "Name is required!")]
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private int _age;

        [Range(1, 150, ErrorMessage = "Age must be between 1 - 150")]
        public int Age
        {
            get => _age;
            set => this.RaiseAndSetIfChanged(ref _age, value);
        }
    }
}
