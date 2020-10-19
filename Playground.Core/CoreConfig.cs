using System;
using System.IO;
using Playground.Core.Diagnostics;

namespace Playground.Core
{
    public class CoreConfig
    {
        public static Func<bool> UseDevDatabase;
        public static string TnsFilePath = @"C:\oraclexe\app\oracle\product\11.2.0\server\network\ADMIN\tnsnames.ora";
        public static string LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Playground", "Logs");

        public  static void CreateApplicationRelatedFolders(string appName)
        {
            if (!Directory.Exists(LogsDirectory))
            {
                Directory.CreateDirectory(LogsDirectory);
            }

            Logger.Info($"{appName} Started....");
        }

        public static void CreateLogsLocally(string log)
        {
            string fileName = Path.Combine(LogsDirectory, @"\LocalLogs.txt");

            if (!File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(DateTime.Now + "\t" + log);
                    }
                }
            }
            else
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(DateTime.Now + "\t" + log);
                    }
                }
            }
        }

        public static Tuple<bool, string> GetAppShotDownStatus()
        {
            var filePath = @"C:\Temp\SHUT_DOWN_WINFORM_APP.txt";
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath);
                return new Tuple<bool, string>(true, fileContent);
            }

            return new Tuple<bool, string>(false, string.Empty);
        }
    }
}
