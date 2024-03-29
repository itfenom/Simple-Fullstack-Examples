﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
// ReSharper disable RedundantDelegateCreation
// ReSharper disable UnusedMethodReturnValue.Local

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewLib
{
    public class MetaAccountMainViewModel : BaseViewModel, IDisposable
    {
        //Fields
        private readonly OneTaskProcessor _procesor;
        private readonly MetaAccountRootViewModel _root;
        private ICommand _searchCommand;
        private bool _disposed;
        private bool _isStringContainedSearchOption;
        private int _countSearchMatches;
        private bool _isProcessing;
        private bool _isLoading;
        private string _statusStringResult;
        private string _searchString;

        //Constructor
        public MetaAccountMainViewModel()
        {
            _procesor = new OneTaskProcessor();
            _root = new MetaAccountRootViewModel();
            _disposed = false;
            _searchString = "FINANCE";
            _isProcessing = _isLoading = false;
            _countSearchMatches = 0;
            _isStringContainedSearchOption = true;
        }

        //Properties
        public bool IsProcessing
        {
            get => _isProcessing;
            protected set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    NotifyPropertyChanged(() => IsProcessing);
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            protected set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    NotifyPropertyChanged(() => SearchString);
                }
            }
        }

        public bool IsStringContainedSearchOption
        {
            get => _isStringContainedSearchOption;
            set
            {
                if (_isStringContainedSearchOption != value)
                {
                    _isStringContainedSearchOption = value;
                    NotifyPropertyChanged(() => IsStringContainedSearchOption);
                }
            }
        }

        public string StatusStringResult
        {
            get => _statusStringResult;
            protected set
            {
                if (_statusStringResult != value)
                {
                    _statusStringResult = value;
                    NotifyPropertyChanged(() => StatusStringResult);
                }
            }
        }

        public int CountSearchMatches
        {
            get => _countSearchMatches;
            protected set
            {
                if (_countSearchMatches != value)
                {
                    _countSearchMatches = value;
                    NotifyPropertyChanged(() => CountSearchMatches);
                }
            }
        }

        public MetaAccountRootViewModel Root => _root;

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand<object>(async (p) =>
                    {
                        string findThis = p as string;

                        if (findThis == null)
                            return;

                        await SearchCommand_ExecutedAsync(findThis);
                    },
                        (p =>
                        {
                            if (Root.BackUpCountryRootsCount == 0 && IsProcessing == false)
                                return false;

                            return true;
                        })
                    );
                }

                return _searchCommand;
            }
        }

        //Methods

        public async Task LoadSampleDataAsync()
        {
            IsProcessing = true;
            IsLoading = true;
            StatusStringResult = "Loading Data... please wait.";
            try
            {
                await Root.LoadData();

                IsLoading = false;
                StatusStringResult = $"Searching... '{SearchString}'";

                await SearchCommand_ExecutedAsync(SearchString);
            }
            finally
            {
                IsLoading = false;
                IsProcessing = false;
            }
        }

        private async Task<int> SearchCommand_ExecutedAsync(string findThis)
        {
            // Setup search parameters
            SearchParams param = new SearchParams(findThis, (IsStringContainedSearchOption ? SearchMatch.StringIsContained : SearchMatch.StringIsMatched));

            // Make sure the task always processes the last input but is not started twice
            try
            {
                IsProcessing = true;

                var tokenSource = new CancellationTokenSource();
                Func<int> a = new Func<int>(() => Root.DoSearch(param, tokenSource.Token));
                var t = await _procesor.ExecuteOneTask(a, tokenSource);

                this.StatusStringResult = findThis;
                CountSearchMatches = t;

                return CountSearchMatches;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            finally
            {
                IsProcessing = false;
            }

            return -1;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _procesor.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
