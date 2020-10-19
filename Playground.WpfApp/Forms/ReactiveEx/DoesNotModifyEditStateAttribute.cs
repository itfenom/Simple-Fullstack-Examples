using System;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Marker attribute used to mark a property that does not modify the edit state of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DoesNotModifyEditStateAttribute : Attribute
    {
    }
}
