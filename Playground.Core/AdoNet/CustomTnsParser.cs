using System;
using System.Collections;
using System.IO;

namespace Playground.Core.AdoNet
{
    internal sealed class CustomTnsParser
    {
        private string[] _descriptionAlias;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private ArrayList _descriptionString = new ArrayList();

        public static readonly CustomTnsParser Instance = new CustomTnsParser();

        private CustomTnsParser()
        {
            LoadAliasDescriptions(CoreConfig.TnsFilePath);
        }

        private void LoadAliasDescriptions(string tnsFilePath)
        {
            var output = "";
            var description = "";
            var parens = new Stack();

            StreamReader sr = null;
            try
            {
                sr = new StreamReader(tnsFilePath);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Could not locate " + tnsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Read the first line of the file
            if (sr != null)
            {
                try
                {
                    var fileLine = sr.ReadLine();
                    // loop through, reading each line of the file
                    while (fileLine != null)
                    {
                        // if the first non whitespace character is a #, ignore the line
                        // and go to the next line in the file
                        if (fileLine.Length > 0 && fileLine.Trim().Substring(0, 1) != "#")
                        {
                            // Read through the input line character by character
                            for (int i = 0; i < fileLine.Length; i++)
                            {
                                var lineChar = fileLine[i];
                                if (parens.Count != 0)
                                {
                                    description += lineChar;
                                }

                                if (lineChar == '(')
                                {
                                    // if the char is a ( push it onto the stack
                                    parens.Push(lineChar);
                                }
                                else if (lineChar == ')')
                                {
                                    // if the char is a ), pop the stack
                                    parens.Pop();
                                    if (parens.Count == 0)
                                    {
                                        _descriptionString.Add("(" + description);
                                        description = "";
                                    }
                                }
                                else
                                {
                                    // if there is nothing in the stack, add the character to the ouput
                                    if (parens.Count == 0)
                                    {
                                        output += lineChar;
                                    }
                                }
                            }
                        }

                        // Read the next line of the file
                        fileLine = sr.ReadLine();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                // Close the stream reader
                sr.Close();
            }

            // Split the output string into a string[]
            _descriptionAlias = output.Split('=');

            // trim each string in the array
            for (var i = 0; i < _descriptionAlias.Length; i++)
            {
                _descriptionAlias[i] = _descriptionAlias[i].Trim();
            }
        }

        public string GetDatabasesDescription(string pShortName)
        {
            var index = -1;
            for (var i = 0; i < _descriptionAlias.Length; i++)
            {
                if (_descriptionAlias[i] == pShortName)
                {
                    index = i;
                }
            }
            return index != -1 ? _descriptionString[index].ToString() : "";
        }
    }
}
