using System;
using System.Collections.Generic;
using System.IO;

namespace SIC.Assembler.Utilities
{
    public class FileIO
    {
        public static string[] GetAllNonEmptyLines(string filePath)
        {
            return GetAllLines(filePath, true);
        }
        public static string[] GetAllLines(string filePath, bool ignoreBlankLines)
        {
            var lines = new List<string>();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Must provide a non-null OR empty file path");
            }

            using (StreamReader symbolFile = File.OpenText(filePath))
            {
                string line;
                while ((line = symbolFile.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (ignoreBlankLines == false)
                        {
                            lines.Add(line);
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.ToArray();
        }
    }
}