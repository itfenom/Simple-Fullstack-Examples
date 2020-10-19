using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public class CategoryModel : ValidationPropertyChangedBase, IEditableObject
    {
        private int _categoryId;

        public int CategoryId
        {
            get => _categoryId;
            set => SetPropertyValue(ref _categoryId, value);
        }

        private string _categoryName;

        [Required(ErrorMessage = "Category Name is required!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category Name Should be minimum 3 characters and a maximum of 50 characters")]
        [DataType(DataType.Text)]
        public string CategoryName
        {
            get => _categoryName;
            set => SetPropertyValue(ref _categoryName, value);
        }

        #region IEditableObject implementation

        private CategoryModel _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = MemberwiseClone() as CategoryModel;
            IsDirty = true;
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            CategoryId = _backupCopy.CategoryId;
            CategoryName = _backupCopy.CategoryName;
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        #endregion IEditableObject implementation
    }
}