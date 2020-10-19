using System;
using System.IO;
using System.Reflection;

namespace Playground.WpfApp
{
    public static class ApplicationInfo
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<string> productName = new Lazy<string>(GetProductName);
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<string> version = new Lazy<string>(GetVersion);
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<string> company = new Lazy<string>(GetCompany);
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<string> copyright = new Lazy<string>(GetCopyright);
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<string> applicationPath = new Lazy<string>(GetApplicationPath);

        /// <summary>
        /// Gets the product name of the application.
        /// </summary>
        public static string ProductName => productName.Value;

        /// <summary>
        /// Gets the version number of the application.
        /// </summary>
        public static string Version => version.Value;

        /// <summary>
        /// Gets the company of the application.
        /// </summary>
        public static string Company => company.Value;

        /// <summary>
        /// Gets the copyright information of the application.
        /// </summary>
        public static string Copyright => copyright.Value;

        /// <summary>
        /// Gets the path for the executable file that started the application, not including the executable name.
        /// </summary>
        public static string ApplicationPath => applicationPath.Value;

        private static string GetProductName()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyProductAttribute attribute = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    entryAssembly, typeof(AssemblyProductAttribute)));
                return (attribute != null) ? attribute.Product : "";
            }
            return "";
        }

        private static string GetVersion()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                return entryAssembly.GetName().Version.ToString();
            }
            return "";
        }

        private static string GetCompany()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyCompanyAttribute attribute = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                    entryAssembly, typeof(AssemblyCompanyAttribute)));
                return (attribute != null) ? attribute.Company : "";
            }
            return "";
        }

        private static string GetCopyright()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyCopyrightAttribute attribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                    entryAssembly, typeof(AssemblyCopyrightAttribute));
                return attribute != null ? attribute.Copyright : "";
            }
            return "";
        }

        private static string GetApplicationPath()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                return Path.GetDirectoryName(entryAssembly.Location);
            }
            return "";
        }
    }
}
