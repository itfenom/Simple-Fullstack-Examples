using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Playground.WpfApp.WpfUtilities
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            //this.Source = SettingsManager.Settings; ==> if using from a different assemly
            Source = Properties.Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }

    public class SettingsManager
    {
        private static ApplicationSettingsBase _settings;

        static SettingsManager()
        {
            _settings = GetSettings();
        }

        public static ApplicationSettingsBase Settings
        {
            get => _settings;
            set => _settings = value;
        }

        private static ApplicationSettingsBase GetSettings()
        {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm != null)
            {
                Type settingsType = (from t in asm.GetTypes()
                                     where t.IsSubclassOf(typeof(ApplicationSettingsBase))
                                     select t).FirstOrDefault();
                if (settingsType != null)
                {
                    PropertyInfo pi = settingsType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
                    if (pi != null)
                    {
                        return pi.GetValue(null, null) as ApplicationSettingsBase;
                    }

                    return Activator.CreateInstance(settingsType) as ApplicationSettingsBase;
                }
            }
            return null;
        }
    }
}