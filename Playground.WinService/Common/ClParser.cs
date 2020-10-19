using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Playground.WinService.Common
{
    public class ClParser
    {
        private StringDictionary _parameters;
        private string _programFullName;
        private string _programName;
        private string _programPath;

        public ClParser()
        {
            _programPath = null;
            _programFullName = null;
            _programName = null;
            Parse(Environment.GetCommandLineArgs());
        }

        public ClParser(string[] args)
        {
            _programPath = null;
            _programFullName = null;
            _programName = null;
            Parse(args);
        }

        public void Parse(string[] args)
        {
            ProgramNamePath(args);
            _parameters = new StringDictionary();
            Regex regex = new Regex("^-{1,2}|^/|=|:", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex regex2 = new Regex("^['\"]?(.*?)['\"]?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string key = null;
            foreach (string str2 in args)
            {
                string[] strArray = regex.Split(str2, 3);
                switch (strArray.Length)
                {
                    case 1:
                        if (key != null)
                        {
                            if (!_parameters.ContainsKey(key))
                            {
                                strArray[0] = regex2.Replace(strArray[0], "$1");
                                _parameters.Add(key, strArray[0]);
                            }
                            key = null;
                        }
                        break;

                    case 2:
                        if ((key != null) && !_parameters.ContainsKey(key))
                        {
                            _parameters.Add(key, "true");
                        }
                        key = strArray[1];
                        break;

                    case 3:
                        if ((key != null) && !_parameters.ContainsKey(key))
                        {
                            _parameters.Add(key, "true");
                        }
                        key = strArray[1];
                        if (!_parameters.ContainsKey(key))
                        {
                            strArray[2] = regex2.Replace(strArray[2], "$1");
                            _parameters.Add(key, strArray[2]);
                        }
                        key = null;
                        break;
                }
            }
            if ((key != null) && !_parameters.ContainsKey(key))
            {
                _parameters.Add(key, "true");
            }
        }

        private void ProgramNamePath(string[] args)
        {
            try
            {
                int startIndex = args[0].LastIndexOf('\\') + 1;
                _programPath = args[0].Substring(0, startIndex - 1);
                _programFullName = args[0].Substring(startIndex, args[0].Length - startIndex);
                int length = _programFullName.LastIndexOf('.');
                _programName = _programFullName.Substring(0, length);
            }
            catch
            {
                // ignored
            }
        }

        public string this[string param] => _parameters[param];

        public string ProgramFullName => _programFullName;

        public string ProgramName => _programName;

        public string ProgramPath => _programPath;
    }
}
