using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.MultipleDataGrids
{
    public partial class AddFoodDialogView
    {
        private readonly AddFoodDialogViewModel _viewModel;

        public AddFoodDialogView(List<FavoriteFoodModel> foodList)
        {
            InitializeComponent();
            _viewModel = new AddFoodDialogViewModel(foodList);
            DataContext = _viewModel;
        }
    }

    public class AddFoodDialogViewModel : ValidatableBindableBase
    {
        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        private string _foodText;

        [DisplayName("Food")]
        [Required(ErrorMessage = "Food is required.", AllowEmptyStrings = false)]
        [StringLength(20, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string FoodText
        {
            get => _foodText;
            set
            {
                this.RaiseAndSetIfChanged(ref _foodText, value);

                ValidateProperty(nameof(FoodText));
                this.RaisePropertyChanged(nameof(HasErrors));
            }
        }

        public bool OkToAddFood { get; set; }

        public AddFoodDialogViewModel(List<FavoriteFoodModel> foodList)
        {
            OkToAddFood = false;

            var canExecuteOk = this.WhenAnyValue(
                x => x.FoodText,
                x => x.HasErrors,
                (lot, hasErr)
                    =>
                {
                    return !hasErr;
                });

            OkCommand = ReactiveCommand.Create(() =>
            {
                var existingLotCount = foodList.Where(l => l.FoodName == FoodText.Trim()).ToList().Count;
                if (existingLotCount > 0)
                {
                    MessageBox.Show("Food already exist!", "Invalid Food", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                OkToAddFood = true;

                CloseWindowFlag = true;

            }, canExecuteOk).DisposeWith(Disposables.Value);

            //Close/Exit
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });

            ValidateProperty(nameof(FoodText));
        }

        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}
