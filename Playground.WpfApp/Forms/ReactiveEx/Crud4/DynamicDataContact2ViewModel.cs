using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using Playground.WpfApp.Behaviors;
using Playground.WpfApp.Forms.ReactiveEx.Crud3;
using ReactiveUI;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Playground.WpfApp.Forms.ReactiveEx.Crud4
{
 /*
 * This is to test DynamicData using ReactiveUI
 * Using ObservableCollectionExtended<T> of DynamicData, which is single threaded.
 */

    public class DynamicDataContact2ViewModel : ValidatableBindableBase, ICloseWindow
    {
        public override string Title => "CRUD4 - ReactiveUI with ObservableCollectionExtended<T>";

        private List<Contact> _deletedContacts;

        private readonly SourceList<Contact> _contacts = new SourceList<Contact>();
        public ObservableCollectionExtended<Contact> Contacts { get; } = new ObservableCollectionExtended<Contact>();

        readonly ObservableAsPropertyHelper<bool> _databasesValid;
        public bool DatabasesValid
        {
            get
            {
                if (_databasesValid == null)
                    return false;
                return _databasesValid.Value;
            }
        }

        private Contact _selectedContact;

        public Contact SelectedContact
        {
            get => _selectedContact;
            set => this.RaiseAndSetIfChanged(ref _selectedContact, value);
        }

        //-----------------
        //Constructor
        public DynamicDataContact2ViewModel()
        {
            _deletedContacts = new List<Contact>();
            _contacts.AddRange(GetAllContacts());

            //Reset filter variables
            _nameFilter = string.Empty;
            _emailFilter = string.Empty;

            //using reactive ui operator to respond to any change 
            var multipleFilters = this.WhenAnyValue(
                    x => x.NameFilter,
                    x => x.EmailFilter)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Do(s =>
                {
                    Console.WriteLine($@"\r\nSearching for: {s}");
                }).Select(
                    searchTerm =>
                    {

                        var filters = BuildGroupFilter();

                        return row => filters.All(filter => filter(row));

                        bool Searcher(Contact item) => item.Name.ToLower().Contains(searchTerm.Item1.ToLower()) ||
                                                                     item.Email.ToLower().Contains(searchTerm.Item2.ToLower());

#pragma warning disable 162
                        return (Func<Contact, bool>)Searcher;
#pragma warning restore 162
                    });


            
            //Connect to make ObservableChangeSet
            this._contacts.Connect()
                .Bind(this.Contacts)
                .Filter(multipleFilters)
                .DisposeMany()
                .WhenAnyPropertyChanged(nameof(Contact.Name), nameof(Contact.Email), nameof(Contact.Phone))
                .Subscribe(myObject =>
                {
                    // Do stuff with myObject
                    Console.WriteLine($"{myObject.Name} has changed!");
                });


            //setup change tracking
            var isValid = Contacts
              .ToObservableChangeSet()
              // Subscribe only to IsValid property changes.
              .AutoRefresh(database => database.HasErrors)
              // Materialize the collection.
              .ToCollection()
              // Determine if all forms are valid.
              .Select(x => x.All(y => !y.HasErrors));

            _databasesValid = isValid
              .ObserveOn(RxApp.MainThreadScheduler)
              .ToProperty(this, x => x.DatabasesValid);

            //Add new Command
            AddNewContactCommand = ReactiveCommand.Create(() =>
            {
                _contacts.Add(new Contact { Name = string.Empty, Phone = string.Empty, Email = string.Empty, EditState = EditState.New });
            }).DisposeWith(Disposables.Value);

            //Delete Command
            var isSelected = this.WhenAnyValue(x => x.SelectedContact, (Contact c) => c != null);
            DeleteContactCommand = ReactiveCommand.Create(
                () =>
                {
                    var result = MessageBox.Show($@"Are you sure, you want to delete {SelectedContact.Name} ?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SelectedContact.EditState = EditState.Deleted;
                        SelectedContact.EndEdit();
                        _deletedContacts.Add(SelectedContact);
                        _contacts.Remove(SelectedContact);
                        SelectedContact = null;
                    }
                }, isSelected).DisposeWith(Disposables.Value);

            //Save Command
            var canExecuteSave = this.WhenAnyValue(
                x => x.SelectedContact,
                x => x.SelectedContact.Name,
                x => x.SelectedContact.Email,
                x => x.SelectedContact.Phone,
                x => x.HasErrors,
                x => x.AllErrors,
                x => x.Contacts,
                (s, n, e, p, err, errCount, cts) =>
                {
                    return DatabasesValid == true;
                });

            SaveCommand = ReactiveCommand.Create(() => Save(), canExecuteSave).DisposeWith(Disposables.Value);

            //Cancel/Close window Command
            CancelCommand = ReactiveCommand.Create(() =>
            {
                Close?.Invoke();
            });
        }

        private List<Contact> GetAllContacts()
        {
            var retVal = new List<Contact>
            {
                new Contact {Name = "Kashif", Phone = "972-207-2406", Email = "Kashif@test.com"},
                new Contact {Name = "James", Phone = "972-207-2407", Email = "James@test.com"},
                new Contact {Name = "Carlene", Phone = "972-207-2408", Email = "Carlene@test.com"}
            };

            return retVal;
        }

        public bool ValidateEmail(string email)
        {
            var emailRegExp = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            return Regex.IsMatch(email, emailRegExp);

        }

        public ReactiveCommand<Unit, Unit> AddNewContactCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteContactCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private void Save()
        {
            //Validate before saving!

            //Edit/Insert
            foreach (var item in _contacts.Items)
            {
                if (item.IsEditing || item.EditState == EditState.Changed)
                {
                    //Update
                    item.EditState = EditState.NotChanged;
                    item.EndEdit();
                }

                if (item.EditState == EditState.New)
                {
                    //Insert
                    item.EditState = EditState.NotChanged;
                    item.EndEdit();
                }
            }

            //Delete
            if (_deletedContacts.Count > 0)
            {
                foreach (var item in _deletedContacts)
                {
                    //Delete    
                }

                _deletedContacts.Clear();
            }

            SelectedContact = null;
        }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        #region Filtering
        private string _nameFilter;

        public string NameFilter
        {
            get => _nameFilter;
            set => this.RaiseAndSetIfChanged(ref _nameFilter, value);
        }

        private string _emailFilter;

        public string EmailFilter
        {
            get => _emailFilter;
            set => this.RaiseAndSetIfChanged(ref _emailFilter, value);
        }

        private IEnumerable<Predicate<Contact>> BuildGroupFilter()
        {
            if (!string.IsNullOrEmpty(NameFilter))
            {
                yield return rowView => rowView.Name.ToLower().Contains(NameFilter.ToLower());
            }

            if (!string.IsNullOrEmpty(EmailFilter))
            {
                yield return rowView => rowView.Email.ToLower().Contains(EmailFilter.ToLower());
            }
        }

        private Func<Contact, bool> BuildFilters(string nameFilter, string emailFilter)
        {
            if (string.IsNullOrEmpty(nameFilter) && string.IsNullOrEmpty(emailFilter)) return c => true;

            return c => c.Name.ToLower().Contains(nameFilter.ToLower())
                                 || c.Email.ToLower().Contains(emailFilter.ToLower());
        }

        #endregion

        #region Closing
        public Action Close { get; set; }

        public bool HasUnsavedChanges()
        {
            if (_deletedContacts.Count > 0) return true;

            var isNewOrChangedObjects = Contacts
                .Where(x => x.IsEditing || x.EditState == EditState.New || x.EditState == EditState.Changed).ToList();

            if (isNewOrChangedObjects.Count > 0) return true;

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
            _selectedContact = null;
        }
        #endregion
    }
}
