using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace AAI_Log_Converter
{
    class FileUtils
    {

        /// <summary>   The valid file extensions. </summary>
        public static List<string> ValidFileExtensions = new List<string>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   File is valid. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool FileIsValid(string filePath)
        {
            if (ValidFileExtensions.Count > 0) {
                foreach (string extension in ValidFileExtensions) {
                    if (filePath.EndsWith(extension)) {
                        return true;
                    }
                    else
                    {
                        ServiceLogger.WriteLine("SKIPPED FILE:: The file " + filePath + " does not have the file extension " + extension + ".");
                    }
                }
            } else {
                throw new Exception("No file extensions have been added to the ValidFileExtensions List.");
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Query if 'filePath' is file in use. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   True if filein use, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsFileinUse(string filePath)
        {
            bool result = false;
            Stream s = null;
            try {
                if (FileExists(filePath)) {
                    s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                }
            }
            catch (IOException) {
                result = true;
                throw new Exception("The File " + filePath + " is currently open.\nPlease close the file and try again.");
            }
            finally {
                if (s != null) {
                    s.Close();
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given file exists. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given directory exists. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool DirectoryExists(string filePath)
        {
            return Directory.Exists(filePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets sub directories. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="targetDirectory">  Pathname of the target directory. </param>
        ///
        /// <returns>   An array of string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string[] GetSubDirectories(string targetDirectory)
        {
            return Directory.GetDirectories(targetDirectory);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all files. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="targetDirectory">  Pathname of the target directory. </param>
        ///
        /// <returns>   An array of string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string[] GetAllFiles(string targetDirectory)
        {
            return Directory.GetFiles(targetDirectory);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets file name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   The file name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetFileName(string filePath)
        {
            if (filePath.Contains("_")) {
                filePath = filePath.Split('_')[1];
            }
            return Path.GetFileName(filePath).Split('.')[0].Trim();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes to file. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileName"> Filename of the file. </param>
        /// <param name="sb">       A string builder. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void WriteToFile(string fileName, StringBuilder sb)
        {
            fileName = Program.OutputDirectory + "\\" + fileName;
            File.AppendAllText(fileName, sb.ToString());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a directory. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="directory">    Pathname of the directory. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreateDirectory(string directory)
        {
            if (!DirectoryExists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
}
}
