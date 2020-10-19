using System;
using Microsoft.Win32;

namespace Playground.WpfApp.Services
{
    public interface IOpenFileDialogService
    {
        string GetSelectedFileName(string filter, bool multiSelect);
    }

    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string GetSelectedFileName(string filter, bool multiSelect)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = filter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = multiSelect
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }

            return null;
        }
    }
}
