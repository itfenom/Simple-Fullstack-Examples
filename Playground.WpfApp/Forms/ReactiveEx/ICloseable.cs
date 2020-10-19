using System;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Simple interface for use by view-models that support closing.
    /// </summary>
    public interface ICloseable
    {
        /// <summary>
        /// An event fired by the view-model when it is requesting that it be closed.
        /// </summary>
        event EventHandler RequestClose;

        /// <summary>
        /// Determines whether the object can be closed
        /// </summary>
        /// <returns><c>true</c> if the object can close; otherwise, <c>false</c>.</returns>
        bool CanClose();
    }
}
