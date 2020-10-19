using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// A simple interface which must be supported by the view-model classes using the ValidationExceptionBehavior.
    /// </summary>
    public interface IValidationExceptionHandler
    {
        void ValidationExceptionsChanged(int count);
    }
}
