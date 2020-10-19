using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Playground.WpfApp.Properties;

namespace Playground.WpfApp.WpfUtilities
{
    /// <summary>
    /// A markup extension that provides simple access to a given entry in the application's
    /// <see cref="Resources" /> file.
    /// </summary>
    public class ResourceExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes the <see cref="ResourceExtension" /> markup extension with the key to be assigned.
        /// </summary>
        /// <param name="resourceKey">The resource key to be assigned.</param>
        public ResourceExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// The resource key to be used for the lookup.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Performs a lookup for the defined <see cref="ResourceKey" />.
        /// </summary>
        /// <returns>
        /// The value of the resource that is specified by the <see cref="ResourceKey" /> property.
        /// If the property is not set, a null reference is returned.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(ResourceKey))
            {
                return null;
            }

            // get the item from the resource
            var item = Resources.ResourceManager.GetObject(ResourceKey);

            if (item is System.Drawing.Bitmap bitmap)
            {
                return GetImageStream(bitmap);
            }

            return item;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        private static BitmapSource GetImageStream(System.Drawing.Bitmap bitmap)
        {
            bitmap.MakeTransparent(System.Drawing.Color.Magenta);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPt,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
    }
}
