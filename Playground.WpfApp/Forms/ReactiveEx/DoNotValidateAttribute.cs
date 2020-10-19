using System;

namespace Playground.WpfApp.Forms.ReactiveEx
{

    /// <summary>
    /// Marker attribute that denotes that changes to the property should not cause validation to occur.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DoNotValidateAttribute : Attribute
    {
    }
}
