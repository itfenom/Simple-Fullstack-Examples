using System;
using System.IO;

namespace Playground.Core.Utilities
{
    public class FileOperations
    {
        public static int GetNumberOfRecordsInFile(string fileWithPath)
        {
            int retVal = 0;
            // ReSharper disable once UnusedVariable
            var fi = new FileInfo(fileWithPath);
            using (var sReader = new StreamReader(fileWithPath))
            {
                while (sReader.ReadLine() != null)
                {
                    retVal++;
                }
            }

            return retVal;
        }

        public static bool SplitLargeFileIntoSmallFiles(string largeFile, string destPath, int maxRows)
        {
            try
            {
                var reader = File.OpenText(largeFile);
                var fileName = DateTime.Now.ToString("yyMMdd");
                var outFileName = destPath + @"\" + fileName + ".spt" + "{0}";
                var outFileNumber = 1;
                while (!reader.EndOfStream)
                {
                    var writer = File.CreateText(string.Format(outFileName, outFileNumber++));
                    for (int idx = 0; idx < maxRows; idx++)
                    {
                        writer.WriteLine(reader.ReadLine());
                        if (reader.EndOfStream) break;
                    }
                    writer.Close();
                }
                reader.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to split file provided.", ex);
            }
        }

        public static bool DeleteOlderFiles(string targetPath, int numberOfDaysOld, string filePattern)
        {
            bool retVal = false;

            // ReSharper disable once ReplaceWithStringIsNullOrEmpty
            if (targetPath != null && targetPath.Length > 0)
            {
                string[] fileList = Directory.GetFiles(targetPath, filePattern);

                if (fileList.Length > 0)
                {
                    foreach (string file in fileList)
                    {
                        TimeSpan fileAge = File.GetLastWriteTime(file) - DateTime.Now;
                        if (fileAge.Days > numberOfDaysOld)
                        {
                            File.Delete(file);
                            retVal = true;
                        }
                    }
                }
            }
            return retVal;
        }

        public static string GetDocTypes(string docType)
        {
            string types = string.Empty;
            switch (docType)
            {
                case "DOC1":
                    types = "MISC";
                    break;
                case "DOC2":
                    types = "OTHER";
                    break;
                case "ADMIN":
                    types = "STMT";
                    break;
            }
            return types;
        }

        // ReSharper disable once UnusedMember.Local
        private static string GetUniqueFileName(string fileName, string destFolder)
        {
            var retVal = string.Empty;
            var goodName = false;
            string proposedName;
            var fileExt = GetFileExtension(fileName);
            fileName = GetFileName(fileName);

            int counter = 1;

            while (goodName == false)
            {
                proposedName = Path.Combine(destFolder, fileName + "-" + counter.ToString() + fileExt);

                if (File.Exists(proposedName))
                {
                    counter++;
                }
                else
                {
                    goodName = true;
                    retVal = proposedName;
                }
            }

            return retVal;
        }

        public static string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public static string GetFileName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public static string FileSizeAsString(long size)
        {
            return (double)size / 1024.0 >= 1.0 ? ((double)size / 1048576.0 >= 1.0 ? new Decimal(size / 1048576.0).ToString("#,##0.00") + " MBs" : new decimal(size / 1024.0).ToString("#,##0.00") + " KBs") : new decimal(size).ToString("#,##0") + " Bs";
        }
    }
}
