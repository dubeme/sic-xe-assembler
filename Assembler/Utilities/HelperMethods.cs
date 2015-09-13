using System;
using System.Collections.Generic;
using System.IO;

namespace SIC.Assembler.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Gets all lines.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="ignoreBlankLines">if set to <c>true</c> [ignore blank lines].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Must provide a non-null OR empty file path</exception>
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

        /// <summary>
        /// Gets all non empty lines.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string[] GetAllNonEmptyLines(string filePath)
        {
            return GetAllLines(filePath, true);
        }

        /// <summary>
        /// Throws the null or white space string exception.
        /// </summary>
        /// <param name="argName">Name of the argument.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ThrowNullOrWhiteSpaceStringException(string argName)
        {
            throw new ArgumentNullException(argName, string.Format("{0} can't be null, empty OR only whitespace(s)", argName));
        }
    }
}