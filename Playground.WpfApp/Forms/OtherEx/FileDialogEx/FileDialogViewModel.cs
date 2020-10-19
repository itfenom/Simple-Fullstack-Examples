using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Services;

namespace Playground.WpfApp.Forms.OtherEx.FileDialogEx
{
    public class FileDialogViewModel : PropertyChangedBase
    {
        public override string Title => "File Dialog Ex";

        private readonly IOpenFileDialogService _openFileDialogService;

        public FileDialogViewModel(IOpenFileDialogService openFileDialogService)
        {
            _openFileDialogService = openFileDialogService;
            _displayTextBoxValueCommand = new DelegateCommand(() => DisplayTextBoxValue(), () => !string.IsNullOrEmpty(SelectedFile));

            PropertyChanged += FileDialogViewModel_PropertyChanged;
        }

        private void FileDialogViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _displayTextBoxValueCommand.RaiseCanExecuteChanged();
            }
        }

        private string _selectedFile;

        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                NotifyPropertyChanged("SelectedFile");
            }
        }

        private DelegateCommand _displayTextBoxValueCommand;

        public DelegateCommand DisplayTextBoxValueCommand
        {
            get => _displayTextBoxValueCommand;
            set => _displayTextBoxValueCommand = value;
        }

        private void DisplayTextBoxValue()
        {
            string value = Convert.ToString(SelectedFile);

            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("No value to display!");
                return;
            }

            MessageBox.Show(value);
        }

        public ICommand LaunchFileDialogCommand
        {
            get { return new DelegateCommand(() => LaunchFileDialog()); }
        }

        private void LaunchFileDialog()
        {
            var filter = @"JPG (*.jpg,*.jpeg)|*.jpg;*.jpeg
                       |TIFF (*.tif,*.tiff)|*.tif;*.tiff
                       |Log(*.Log, *.log)|*.Log;*.log
                       |C# Csharp(*.CS, *.cs)|*.CS;*.cs";

            SelectedFile = _openFileDialogService.GetSelectedFileName(filter, false);
        }

        public ICommand CloseCommand
        {
            get { return new DelegateCommand(() => Close()); }
        }

        private void Close()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == Title)
                {
                    window.Close();
                }
            }
        }
    }
}