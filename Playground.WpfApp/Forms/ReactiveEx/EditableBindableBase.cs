using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public enum EditState
    {
        NotChanged,
        Changed,
        New,
        Deleted
    }

    public abstract class EditableBindableBase : ValidatableBindableBase, IEditableObject, IRevertibleChangeTracking
    {
        private EditState _editState = EditState.NotChanged;
        private bool _isEditing;
        private Memento _memento;

        protected EditableBindableBase()
        {
            // get all the properties that have the DoesNotModifyEditState attribute defined
            var fieldsThatDoNotModifyState = GetAllProperties(GetType())
                .Where(prop => prop.IsDefined(typeof(DoesNotModifyEditStateAttribute), true))
                .Select(prop => prop.Name)
                .ToList();

            Changed
                .Where(_ =>
                    IsEditing
                    && EditState != EditState.New
                    && EditState != EditState.Deleted
                    && !fieldsThatDoNotModifyState.Contains(_.PropertyName))
                .Subscribe(_ => EditState = EditState.Changed)
                .DisposeWith(Disposables.Value);
        }

        /// <summary>
        /// Event fired when editing is started.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler OnBeginEdit;

        /// <summary>
        /// Event fired when editing is canceled.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler OnCancelEdit;

        /// <summary>
        /// Event fired when editing is ended.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler OnEndEdit;

        /// <summary>
        /// Gets or sets the state of the object.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public EditState EditState
        {
            get => _editState;
            set => this.RaiseAndSetIfChanged(ref _editState, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not this instance has changes.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public virtual bool IsChanged => EditState != EditState.NotChanged;

        /// <summary>
        /// Gets a value indicating whether the object is being edited.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public bool IsEditing
        {
            get => _isEditing;
            protected set => this.RaiseAndSetIfChanged(ref _isEditing, value);
        }

        /// <summary>
        /// Gets or sets the memento used to save state.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public Memento Memento
        {
            get => _memento;
            protected set => this.RaiseAndSetIfChanged(ref _memento, value);
        }

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            EndEdit();
        }

        /// <summary>
        /// Resets the object’s state to unchanged by rejecting the modifications.
        /// </summary>
        public void RejectChanges()
        {
            CancelEdit();
        }

        /// <summary>
        /// Create the memento representing the objects state.
        /// </summary>
        /// <returns>The memento representing the objects state.</returns>
        protected virtual Memento CreateMemento()
        {
            // return an empty memento by default
            return new Memento(null);
        }

        /// <summary>
        /// Restore the state of the object from the memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        protected virtual void RestoreMemento(Memento memento)
        {
            // intentionally left empty
        }

        #region IEditableObject

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (IsEditing)
            {
                return;
            }

            Memento = CreateMemento();
            IsEditing = true;
            OnBeginEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Discards changes since the last <see cref="IEditableObject.BeginEdit" /> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (!IsEditing)
            {
                return;
            }

            RestoreMemento(Memento);
            Memento?.Dispose();
            Memento = null;
            IsEditing = false;
            OnCancelEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Pushes changes since the last <see cref="IEditableObject.BeginEdit" /> or
        /// <see cref="IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        public virtual void EndEdit()
        {
            if (!IsEditing)
            {
                return;
            }

            Memento?.Dispose();
            Memento = null;
            IsEditing = false;
            //EditState = EditState.NotChanged;
            OnEndEdit?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
