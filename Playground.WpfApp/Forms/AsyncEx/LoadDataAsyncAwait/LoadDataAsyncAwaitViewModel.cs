using Playground.WpfApp.Behaviors;
using Playground.WpfApp.Forms.ReactiveEx;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Playground.WpfApp.Forms.AsyncEx.LoadDataAsyncAwait
{
    public class LoadDataAsyncAwaitViewModel : BindableBase, ICloseWindow
    {
        private string _nowProcessingEmployeeText;

        public string NowProcessingEmployeeText
        {
            get => _nowProcessingEmployeeText;
            set => this.RaiseAndSetIfChanged(ref _nowProcessingEmployeeText, value);
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        private FlowDocument _document;

        public FlowDocument Document
        {
            get => _document;
            set => this.RaiseAndSetIfChanged(ref _document, value);
        }

        private Paragraph _paragraph;

        public Paragraph Paragraph
        {
            get => _paragraph;
            set => this.RaiseAndSetIfChanged(ref _paragraph, value);
        }

        private readonly ProcessDataAsyncAwait _processData;

        private CancellationTokenSource _tokenSource;

        public LoadDataAsyncAwaitViewModel()
        {
            _processData = new ProcessDataAsyncAwait();
            _processData.WorkPerformed += ProcessData_WorkPerformed;
            _nowProcessingEmployeeText = string.Empty;

            _document = new FlowDocument();
            _document.Blocks.Add(_paragraph = new Paragraph());

            var canExecuteStart = this.WhenAnyValue(x => x.IsProcessing, (isProcessing) =>
            {
                return isProcessing == false;
            });

            var canExecuteStop = this.WhenAnyValue(x => x.IsProcessing, (isProcessing) =>
            {
                return isProcessing == true;
            });

            StartCommand = ReactiveCommand.Create(() => OnStart(), canExecuteStart).DisposeWith(Disposables.Value);
            StopCommand = ReactiveCommand.Create(() =>
            {
                var userResponse = MessageBox.Show("Are you sure, you want to cancel loading data?", "Confirm cancel", MessageBoxButton.YesNo);

                if (userResponse == MessageBoxResult.Yes)
                {
                    _tokenSource.Cancel();
                }
            }, canExecuteStop).DisposeWith(Disposables.Value);
        }

        private void ProcessData_WorkPerformed(object sender, WorkPerformedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.IsPresident)
                {
                    Run item = new Run(e.Msg);
                    Paragraph.Inlines.Add(new Bold(item));
                }
                else
                {
                    Paragraph.Inlines.Add(e.Msg);
                }

                if (!string.IsNullOrEmpty(e.CurrentlyProcessingEmployee))
                {
                    NowProcessingEmployeeText = e.CurrentlyProcessingEmployee;
                }

                Paragraph.Inlines.Add(new LineBreak());
                Document.Blocks.Add(Paragraph);

                Console.WriteLine($"{e.Percentage}%");
            });
        }

        public ICommand StopCommand { get; }

        public ICommand StartCommand { get; }

        private async Task OnStart()
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            IsProcessing = true;
            var sw = Stopwatch.StartNew();
            try
            {
                Paragraph.Inlines.Add($"{DateTime.Now} -  Starting...");
                Paragraph.Inlines.Add(new LineBreak());

                var result = await _processData.LoadDataAsync(token);
                Paragraph.Inlines.Add(result);
                Paragraph.Inlines.Add(new LineBreak());
            }
            catch (OperationCanceledException canceledException)
            {
                Console.WriteLine(canceledException.Message);
                MessageBox.Show("Cancelled by user");
            }
            finally
            {
                IsProcessing = false;
                NowProcessingEmployeeText = string.Empty;
                Paragraph.Inlines.Add(@"Total Time spent: " + sw.Elapsed.TotalMinutes);
                MessageBox.Show("Loading employee data is complete!");
                _tokenSource.Dispose();
            }
        }

        #region Closing
        public Action Close { get; set; }

        public bool CanClose()
        {
            if(IsProcessing)
            {
                var userResponse = MessageBox.Show("Are you sure, you want to cancel loading data?", "Confirm cancel", MessageBoxButton.YesNo);
                if(userResponse == MessageBoxResult.Yes)
                {
                    _tokenSource.Cancel();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void DisposeResources()
        {
            _processData.WorkPerformed -= ProcessData_WorkPerformed;
            _tokenSource.Dispose();
        }
        #endregion
    }
}
