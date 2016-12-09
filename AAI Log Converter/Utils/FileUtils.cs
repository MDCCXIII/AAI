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
        public static List<string> ValidFileExtensions = new List<string>();

        public static bool FileIsValid(string filePath)
        {
            if (ValidFileExtensions.Count > 0) {
                foreach (string extension in ValidFileExtensions) {
                    if (filePath.EndsWith(extension)) {
                        return true;
                    }
                }
            } else {
                throw new Exception("No file extensions have been added to the ValidFileExtensions List.");
            }
            return false;
        }

        public static List<string> FileToList(string filePath)
        {
            List<string> result = new List<string>();
            if (GetFileLineCount(filePath) > 1800000) {
                Console.WriteLine(filePath + " Exceeds 1.8 million lines.\nPlease split the file and try again.");
                Console.WriteLine();
            } else {
                using (TextReader tr = new StreamReader(filePath)) {
                    string line;
                    while ((line = tr.ReadLine()) != null) {
                        result.Add(line);
                    }
                }
            }
            return result;
        }

        public static int GetFileLineCount(string filePath)
        {
            try {
                return File.ReadAllLines(filePath).Count();
            } catch (Exception ex) {
                Console.WriteLine("Could not register file line count.\n Error Message: \n" + ex.Message);
            }
            return 9999999;
        }

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

        private static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DirectoryExists(string filePath)
        {
            return Directory.Exists(filePath);
        }

        public static string[] GetSubDirectories(string targetDirectory)
        {
            return Directory.GetDirectories(targetDirectory);
        }

        public static string[] GetAllFiles(string targetDirectory)
        {
            return Directory.GetFiles(targetDirectory);
        }

        public static string GetFileName(string filePath)
        {
            if (filePath.Contains("_")) {
                filePath = filePath.Split('_')[1];
            }
            return Path.GetFileName(filePath).Split('.')[0].Trim();
        }

        public static void WriteToFile(string fileName, StringBuilder sb)
        {
            string output = "";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("OutputDirectory")) {
                if (!(output = ConfigurationManager.AppSettings["OutputDirectory"]).Equals("")) {
                    if (!DirectoryExists(output)) {
                        Directory.CreateDirectory(output);
                    }
                    fileName = output + "\\" + fileName;
                }
            }
            File.AppendAllText(fileName, sb.ToString());
        }
    }
}
