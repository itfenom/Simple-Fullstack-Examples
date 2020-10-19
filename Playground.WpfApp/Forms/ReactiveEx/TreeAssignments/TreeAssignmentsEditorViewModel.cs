using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Playground.WpfApp.Mvvm.AttributedValidation;
using Playground.WpfApp.WpfUtilities;
using ReactiveUI;
// ReSharper disable RedundantNameQualifier
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedVariable

namespace Playground.WpfApp.Forms.ReactiveEx.TreeAssignments
{
    public class TreeAssignmentsEditorViewModel : ValidatableBindableBase
    {
        public override string Title => "Tree Assignments";

        public ReadOnlyCollection<string> AllCallLevels { get; } = new List<string>
        {
            "PRODUCT",
            "LOT",
            "STATE",
            "WAFER"
        }.AsReadOnly();

        public List<BawFunction> AllFunctions { get; } = new List<BawFunction>();

        public List<string> AllStates { get; } = new List<string> { "*" };

        private readonly List<BawProduct> _allProducts = new List<BawProduct>();
        private ICollectionView _categories;
        private bool _filterBySelectedProducts;
        private bool _isReadOnly;
        private bool _isStarProduct;
        private string _productFilter;
        private ICollectionView _productsCollection;

        [ValidateObject]
        public TreeAssignmentsModel Model { get; }

        public TreeAssignmentsEditorViewModel()
        {
            Model = new TreeAssignmentsModel
            {
                Lot = "*",
                Wafer = "*"
            };

            Model.ErrorsChanged += Model_ErrorsChanged;

            LoadData();

            _allProducts.AddRange(GetProducts());
            _allProducts.ForEach(prod => prod.WhenAnyValue(p => p.IsChecked).Skip(1).Subscribe(_ =>
            {
                IsStarProduct = false;

                // NOTE: This could be more efficient, but for now (for ease of use) we'll handle it
                //       this way
                Model.Products = _allProducts.Where(product => product.IsChecked).Select(product => product.Product).ToList();

                // notify that properties associated with the products have changed
                this.RaisePropertyChanged(nameof(FilterBySelectedProducts));
                this.RaisePropertyChanged(nameof(AreAllProductsSelected));
                this.RaisePropertyChanged(nameof(SelectedProductCount));
            }).DisposeWith(Disposables.Value));

            // when a filter changes, refresh the products collection
            this.WhenAnyValue(x => x.ProductFilter, x => x.FilterBySelectedProducts)
                .ObserveOnDispatcher()
                .Subscribe(_ => ProductsCollection.Refresh());

            var canExecuteOk = this.WhenAnyValue(
                x => x.HasErrors,
                (hasErr) => { return !hasErr; });

            OkCommand = ReactiveCommand.Create(() => OnOk(), canExecuteOk).DisposeWith(Disposables.Value);

            //Cancel/Close window Command
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });

            Model.BeginEdit();
            ObserveModel(Model);
        }

        private void ObserveModel(TreeAssignmentsModel model)
        {
            // watch for changes to the call-level
            var callLevelChanged = model.WhenAny(x => x.CallLevel, x => new { x.Sender, CallLevel = x.Value }).Skip(1);

            Predicate<string> callLevelIsProduct = callLevel => "PRODUCT".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            Predicate<string> callLevelIsLot = callLevel => "LOT".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            Predicate<string> callLevelIsState = callLevel => "STATE".Equals(callLevel, StringComparison.OrdinalIgnoreCase);
            Predicate<string> callLevelIsWafer = callLevel => "WAFER".Equals(callLevel, StringComparison.OrdinalIgnoreCase);

            callLevelChanged
                .Where(x => callLevelIsProduct(x.CallLevel))
                .Select(x => x.Sender)
                .Subscribe(x =>
                {
                    x.Lot = "*";
                    x.Wafer = "*";
                    x.State = "*";
                })
                .DisposeWith(Disposables.Value);

            callLevelChanged
                .Where(x => callLevelIsLot(x.CallLevel))
                .Select(x => x.Sender)
                .Subscribe(x =>
                {
                    x.Wafer = "*";
                    x.State = "*";
                })
                .DisposeWith(Disposables.Value);

            callLevelChanged
                .Where(x => callLevelIsState(x.CallLevel) || callLevelIsWafer(x.CallLevel))
                .Select(x => x.Sender)
                .Subscribe(x =>
                {
                    x.Wafer = "*";
                })
                .DisposeWith(Disposables.Value);


            // the lot can be edited if not in read-only mode, and the call-level is not PRODUCT
            //_canEditLot = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //        (readOnly, callLevel) => !readOnly && !callLevelIsProduct(Model.CallLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditLot)
            //    .DisposeWith(Disposables.Value);

            var observableLot = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
                     (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel))
                 .DistinctUntilChanged()
                 .ObserveOnDispatcher()
                 .BindTo(this, x => x.CanEditLot).DisposeWith(Disposables.Value);

            // the wafer can be edited if not in read-only mode, and the call-level is not PRODUCT, LOT, or STATE
            //_canEditWafer = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel) && !callLevelIsState(callLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditWafer)
            //    .DisposeWith(Disposables.Value);

            var observableWafer = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
                     (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel) && !callLevelIsState(callLevel))
                 .DistinctUntilChanged()
                 .ObserveOnDispatcher()
                 .BindTo(this, x => x.CanEditWafer).DisposeWith(Disposables.Value);

            // the state can be edited if not in read-only mode, and the call-level is not PRODUCT or LOT
            //_canEditState = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
            //    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel))
            //    .DistinctUntilChanged()
            //    .ToProperty(this, x => x.CanEditState)
            //    .DisposeWith(Disposables.Value);

            var observableState = this.WhenAnyValue(x => x.IsReadOnly, x => x.Model.CallLevel,
                    (readOnly, callLevel) => !readOnly && !callLevelIsProduct(callLevel) && !callLevelIsLot(callLevel))
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .BindTo(this, x => x.CanEditState).DisposeWith(Disposables.Value);

            model.WhenAnyValue(x => x.Function, x => x.Lot, x => x.Products)
                .Subscribe(_ =>
                {
                    /* NOTE: When the function, lot, or product changes, we need to refresh the categories that can be used
                    * based on the current context, which is made up of those values */
                    Categories.Refresh();
                })
                .DisposeWith(Disposables.Value);

            // when the lot changes, select the product (if any) associated with that lot
            var productForLot = model.WhenAnyValue(x => x.Lot)
                .Select(lot => lot?.Trim())
                .Select(lot => string.IsNullOrEmpty(lot) || lot.Length != 7 ? null : GetProductByLot(lot));

            /* When the Read-Only state changes, or the product associated with the lot changes, we want to update whether or not
             * products are selectable based on those values. */
            var xx = Observable.CombineLatest(
                this.WhenAnyValue(x => x.IsReadOnly),
                productForLot.Select(product => !string.IsNullOrEmpty(product) && _allProducts.Any(prod => prod.Product == product)),
                (readOnly, validProduct) => !readOnly && !validProduct)
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .BindTo(this, x => x.CanSelectProducts)
                .DisposeWith(Disposables.Value);

            productForLot
                .Skip(1)
                .DistinctUntilChanged()
                .Subscribe(product =>
                {
                    /* When the product changes in response to a change in the lot, we want to select the product associated with the lot.
                     * If there is no product associated with the lot, we'll de-select all products */
                    foreach (var prod in _allProducts)
                    {
                        prod.IsChecked = prod.Product == product;
                    }
                })
                .DisposeWith(Disposables.Value);

            ValidateProperty(nameof(Model));
        }

        private void Model_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ValidateProperty(nameof(Model));
        }

        private void LoadData()
        {
            //States
            AllStates.Add("AFTER_CAP");
            AllStates.Add("AFTER_FINAL_TRIM");
            AllStates.Add("AFTER_P2_PATTERNING");
            AllStates.Add("AFTER_ZD_TRIM");
            AllStates.Add("BEFORE_FA_TRIM");
            AllStates.Add("BEFORE_TD_TRIM");

            //Functions
            AllFunctions.Add(new BawFunction(9364, "Add Delta to YieldMap", new BawFunctionCategory(20, "PROD")));
            AllFunctions.Add(new BawFunction(18822, "Add Linear RL to RM", new BawFunctionCategory(20, "PROD")));
            AllFunctions.Add(new BawFunction(8178, "Edit TM_Lot", new BawFunctionCategory(20, "PROD")));
            AllFunctions.Add(new BawFunction(4286, "Enable trim file", new BawFunctionCategory(100, "DEV")));
            AllFunctions.Add(new BawFunction(9349, "Edit Product Ink Spec After CAP", new BawFunctionCategory(80, "ADVENG")));
            AllFunctions.Add(new BawFunction(2964, "Get Product InkSpec", new BawFunctionCategory(80, "ADVENG")));
            AllFunctions.Add(new BawFunction(3499, "Save ibe file", new BawFunctionCategory(60, "ENG")));
            AllFunctions.Add(new BawFunction(3692, "Save Scribe Codes", new BawFunctionCategory(20, "PROD")));
            AllFunctions.Add(new BawFunction(4357, "BAW Yield Analysis Toolbox", new BawFunctionCategory(0, "VIEW")));
            AllFunctions.Add(new BawFunction(10724, "Edit DeviceTable - product", new BawFunctionCategory(89, "SPECENG")));
            AllFunctions.Add(new BawFunction(10216, "Activate IBE file regrid and smooth", new BawFunctionCategory(40, "ADVPROD")));

            //Categories
            AllCategories.Add(new BawFunctionCategory(80, "ADVENG"));
            AllCategories.Add(new BawFunctionCategory(40, "ADVPROD"));
            AllCategories.Add(new BawFunctionCategory(100, "DEV"));
            AllCategories.Add(new BawFunctionCategory(60, "ENG"));
            AllCategories.Add(new BawFunctionCategory(20, "PROD"));
            AllCategories.Add(new BawFunctionCategory(89, "SPECENG"));
            AllCategories.Add(new BawFunctionCategory(0, "VIEW"));
        }

        private List<BawProduct> GetProducts()
        {
            var retVal = new List<BawProduct>
            {
                new BawProduct("EG0321", "1328218"),
                new BawProduct("EG0327", "1316418"),
                new BawProduct("EG5520", "1633638"),
                new BawProduct("EG5554", "1830439"),
                new BawProduct("EG5900", "1515306"),
                new BawProduct("EG5920", "1426706"),
                new BawProduct("EG5953", "1410201"),
                new BawProduct("EG5980", "1505219"),
                new BawProduct("EG8886", "1707209"),
                new BawProduct("EG9515", "1022402"),
                new BawProduct("EG9515", "1022502")
            };
            return retVal;
        }

        private string GetProductByLot(string lot)
        {
            var product = _allProducts.Find(p => p.Lot == lot);
            if (product != null)
            {
                return product.Product;
            }

            return "";
        }

        #region Commands
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        private void OnOk()
        {
            if (!Model.IsChanged)
            {
                CloseWindowFlag = true;
                return;
            }

            string callLevel = Model.CallLevel.Trim().Replace("'", "''");
            string lot = Model.Lot.Trim().Replace("'", "''");
            string wafer = Model.Wafer.Trim().Replace("'", "''");
            string state = Model.State.Trim().Replace("'", "''");
            string function = Model.Function.CallLabel;
            string category = Model.Category.Trim().Replace("'", "''");
            string prods = IsStarProduct ? "*" : string.Join(",", Model.Products);
            //string mfdKey = Model.Function.Key.ToString();

            var sb = new StringBuilder();
            sb.AppendLine($"Call Level: {callLevel}");
            sb.AppendLine($"Lot: {lot}");
            sb.AppendLine($"Wafer: {wafer}");
            sb.AppendLine($"State: {state}");
            sb.AppendLine($"Function: {function}");
            sb.AppendLine($"Category: {category}");
            sb.AppendLine($"Products: {prods}");

            MessageBox.Show(sb.ToString(), "Selections", MessageBoxButton.OK, MessageBoxImage.Information);
            Model.AcceptChanges();
            CloseWindowFlag = true;
        }

        #endregion

        #region Properties
        private bool _editLot = true;

        public bool CanEditLot
        {
            get => _editLot;
            set => this.RaiseAndSetIfChanged(ref _editLot, value);
        }

        private bool _editState = true;

        public bool CanEditState
        {
            get => _editState;
            set => this.RaiseAndSetIfChanged(ref _editState, value);
        }

        private bool _editWafer = true;

        public bool CanEditWafer
        {
            get => _editWafer;
            set => this.RaiseAndSetIfChanged(ref _editWafer, value);
        }

        private bool _selectProducts = true;

        public bool CanSelectProducts
        {
            get => _selectProducts;
            set => this.RaiseAndSetIfChanged(ref _selectProducts, value);
        }

        public bool? AreAllProductsSelected
        {
            get
            {
                return ProductsCollection.Cast<BawProduct>().Select(prod => prod.IsChecked).AreAllItemsSelected();
            }

            set
            {
                using (DelayChangeNotifications())
                {
                    using (Model.DelayChangeNotifications())
                    {
                        foreach (BawProduct product in ProductsCollection.OfType<BawProduct>().ToList())
                        {
                            product.IsChecked = value.HasValue && value.Value;
                        }
                    }
                }

                // the star product value can only be set when the user selects all products
                IsStarProduct = value.HasValue && value.Value && SelectedProductCount == _allProducts.Count;
            }
        }

        public ICollectionView Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = CollectionViewSource.GetDefaultView(AllCategories);
                    _categories.Filter = item => FilterCategory(item as BawFunctionCategory);
                }

                return _categories;
            }
        }

        private bool FilterCategory(BawFunctionCategory category)
        {
            if (category == null || Model.Function == null)
            {
                return true;
            }

            var defaultRank = Model.Function.DefaultCategory.Rank;
            return category.Rank >= defaultRank && category.Rank <= 100;
        }

        public bool FilterBySelectedProducts
        {
            get => _filterBySelectedProducts;
            set => this.RaiseAndSetIfChanged(ref _filterBySelectedProducts, value);
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
        }

        public bool IsStarProduct
        {
            get => _isStarProduct;
            set => this.RaiseAndSetIfChanged(ref _isStarProduct, value);
        }

        public string ProductFilter
        {
            get => _productFilter;
            set => this.RaiseAndSetIfChanged(ref _productFilter, value);
        }

        public ICollectionView ProductsCollection
        {
            get
            {
                if (_productsCollection == null)
                {
                    _productsCollection = CollectionViewSource.GetDefaultView(_allProducts);
                    _productsCollection.Filter += prod => Filter(prod as BawProduct);
                }

                return _productsCollection;
            }
        }

        private bool Filter(BawProduct bawProduct)
        {
            if (bawProduct == null)
            {
                return true;
            }

            return bawProduct.Product.IndexOf(ProductFilter ?? "", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   (!FilterBySelectedProducts || bawProduct.IsChecked);
        }

        public int SelectedProductCount => _allProducts.Count(prod => prod.IsChecked);

        private List<BawFunctionCategory> AllCategories { get; } = new List<BawFunctionCategory>();
        #endregion

        #region Closing
        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        public bool HasUnsavedChanges()
        {
            if (CloseWindowFlag == true) return false;

            if (Model.IsChanged) return true;

            return false;
        }

        protected override void DisposeManagedResources()
        {
            Model.ErrorsChanged -= Model_ErrorsChanged;

            base.DisposeManagedResources();
        }

        #endregion
    }

    public sealed class TreeAssignmentsModel : EditableBindableBase, IEquatable<TreeAssignmentsModel>
    {
        private string _callLevel;
        private string _category;
        private BawFunction _function;
        private string _lot;
        private IEnumerable<string> _products = Enumerable.Empty<string>();
        private string _state;
        private string _wafer;

        /// <summary>
        /// Gets the ID for the assignment.
        /// </summary>
        public int? AssignmentId { get; }

        [Display(Name = "Call Level")]
        [Required(AllowEmptyStrings = false)]
        public string CallLevel
        {
            get => _callLevel;
            set => this.RaiseAndSetIfChanged(ref _callLevel, value);
        }

        [Required(AllowEmptyStrings = false)]
        public string Category
        {
            get => _category;
            set => this.RaiseAndSetIfChanged(ref _category, value);
        }

        [Required]
        public BawFunction Function
        {
            get => _function;
            set => this.RaiseAndSetIfChanged(ref _function, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not this instance has changes.
        /// </summary>
        public override bool IsChanged =>
            base.IsChanged
            && (Memento?.State == null || !Equals(Memento.State));

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^\*$|^\d+$", ErrorMessage = "{0} must be a star (*) or a number.")]
        public string Lot
        {
            get => _lot;
            set => this.RaiseAndSetIfChanged(ref _lot, value);
        }

        [Required(ErrorMessage = "You must select at least one product.")]
        [CollectionMinimumLength(length: 1, ErrorMessage = "You must select at least one product.")]
        public IEnumerable<string> Products
        {
            get => _products;
            set => this.RaiseAndSetIfChanged(ref _products, value);
        }

        [Required(AllowEmptyStrings = false)]
        public string State
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        [Required(AllowEmptyStrings = false)]
        public string Wafer
        {
            get => _wafer;
            set => this.RaiseAndSetIfChanged(ref _wafer, value);
        }

        /// <summary>
        /// Create the memento representing the objects state.
        /// </summary>
        /// <returns>The memento representing the objects state.</returns>
        protected override Memento CreateMemento()
        {
            return new Memento(new TreeAssignmentsModel
            {
                CallLevel = CallLevel,
                Category = Category,
                Function = Function,
                Lot = Lot,
                Products = Products,
                State = State,
                Wafer = Wafer
            });
        }

        /// <summary>
        /// Restore the state of the object from the memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        protected override void RestoreMemento(Memento memento)
        {
            var oldState = memento?.State as TreeAssignmentsModel;
            if (oldState == null)
            {
                return;
            }

            CallLevel = oldState.CallLevel;
            Category = oldState.Category;
            Function = oldState.Function;
            Lot = oldState.Lot;
            Products = oldState.Products;
            State = oldState.State;
            Wafer = oldState.Wafer;
        }

        #region Equality

        public static bool operator !=(TreeAssignmentsModel model1, TreeAssignmentsModel model2)
        {
            return !(model1 == model2);
        }

        public static bool operator ==(TreeAssignmentsModel model1, TreeAssignmentsModel model2)
        {
            if (object.ReferenceEquals(model1, model2))
            {
                return true;
            }

            if (object.ReferenceEquals(null, model1))
            {
                return false;
            }

            return model1.Equals(model2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TreeAssignmentsModel);
        }

        public bool Equals(TreeAssignmentsModel other)
        {
            // is the other item null?
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            // is the other item the same object as this instance?
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            // check if all properties are equal
            return
                string.Equals(CallLevel, other.CallLevel)
                && string.Equals(Category, other.Category)
                && string.Equals(Lot, other.Lot)
                && string.Equals(State, other.State)
                && string.Equals(Wafer, other.Wafer)
                && Function?.Key == other.Function?.Key
                && ((Products == null && other.Products == null) || (Products != null && other.Products != null && Products.SequenceEqual(other.Products)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, CallLevel) ? CallLevel.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Category) ? Category.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Lot) ? Lot.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, State) ? State.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Wafer) ? Wafer.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Function) ? Function.Key.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Products) ? Products.Distinct().Aggregate(HashingBase, (x, y) => (x * HashingMultiplier) ^ y.GetHashCode()) : 0);

                return hash;
            }
        }

        #endregion
    }

    public sealed class BawFunction : IEquatable<BawFunction>
    {
        public BawFunction(int key, string callLabel, BawFunctionCategory defaultCategory)
        {
            if (defaultCategory == null)
            {
                throw new ArgumentNullException(nameof(defaultCategory));
            }

            Key = key;
            CallLabel = callLabel;
            DefaultCategory = defaultCategory;
        }

        public string CallLabel { get; }

        public BawFunctionCategory DefaultCategory { get; }

        public int Key { get; }

        #region Equality

        public static bool operator !=(BawFunction functionA, BawFunction functionB)
        {
            return !(functionA == functionB);
        }

        public static bool operator ==(BawFunction functionA, BawFunction functionB)
        {
            if (object.ReferenceEquals(functionA, functionB))
            {
                return true;
            }

            if (object.ReferenceEquals(null, functionA))
            {
                return false;
            }

            return functionA.Equals(functionB);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BawFunction);
        }

        public bool Equals(BawFunction other)
        {
            return !object.ReferenceEquals(null, other)
                && Key == other.Key
                && string.Equals(CallLabel, other.CallLabel)
                && DefaultCategory == other.DefaultCategory;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // choose some large prime numbers to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ Key.GetHashCode();
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, CallLabel) ? CallLabel.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, DefaultCategory) ? DefaultCategory.GetHashCode() : 0);
                return hash;
            }
        }

        #endregion
    }

    public sealed class BawFunctionCategory : IEquatable<BawFunctionCategory>
    {
        public BawFunctionCategory(int rank, string category)
        {
            Rank = rank;
            Category = category;
        }

        public string Category { get; }

        public int Rank { get; }

        #region Equality

        public static bool operator !=(BawFunctionCategory itemA, BawFunctionCategory itemB)
        {
            return !(itemA == itemB);
        }

        public static bool operator ==(BawFunctionCategory itemA, BawFunctionCategory itemB)
        {
            if (object.ReferenceEquals(itemA, itemB))
            {
                return true;
            }

            if (object.ReferenceEquals(null, itemA))
            {
                return false;
            }

            return itemA.Equals(itemB);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BawFunctionCategory);
        }

        public bool Equals(BawFunctionCategory other)
        {
            return !object.ReferenceEquals(null, other)
                   && Rank == other.Rank
                   && string.Equals(Category, other.Category);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // choose some large prime numbers to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ Rank.GetHashCode();
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Category) ? Category.GetHashCode() : 0);
                return hash;
            }
        }

        #endregion
    }

    public class BawProduct : BindableBase
    {
        private bool _isChecked;

        public BawProduct(string product, string lot)
        {
            Product = product;
            Lot = lot;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public string Product { get; }

        public string Lot { get; }
    }
}
