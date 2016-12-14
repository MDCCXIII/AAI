using System;
using System.Collections.Generic;

namespace AAI_Log_Converter
{
    class FilePathImporter
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     For each path provided, determin if the path is a file or directory and processes that
        ///     path.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="args"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ParseArgs(string[] args)
        {
            ServiceLogger.WriteLine();
            foreach (string path in args) {
                ServiceLogger.WriteLine("--- Picked up arg: " + path + " ---");
                if (FileUtils.FileExists(path)) {
                    // This path is a file
                    if (FileUtils.FileIsValid(path) && !FileUtils.IsFileinUse(path)) {
                        ProcessFile(path);
                    }
                } else if (FileUtils.DirectoryExists(path)) {
                    // This path is a directory
                    ProcessDirectory(path);
                } else {
                    throw new Exception(path + " is not a valid file or directory.");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Process all files in the directory passed in, recurse on any directories that are found,
        ///     and process the files they contain.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="targetDirectory">  The fully qualified directory path to the output directory. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ProcessDirectory(string targetDirectory)
        {
            
            ServiceLogger.WriteLine();
            string[] fileEntries = FileUtils.GetAllFiles(targetDirectory);
            foreach (string fileName in fileEntries) {
                // Process the array of files found in the directory.
                ProcessFile(fileName);
            }
            string[] subdirectoryEntries = FileUtils.GetSubDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries) {
                // Recurse into subdirectories of this directory.
                ProcessDirectory(subdirectory);
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     pass through for AddServiceFilePath method. if the service name is set in the App.Config
        ///     only add files with a service name that matches the value from the app.config.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="filePath"> The fully qualified file path. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void ProcessFile(string filePath)
        {
            ServiceLogger.WriteLine();
            if (Program.ServiceName != null && !Program.ServiceName.Equals("")) {
                if (filePath.ToLower().Contains(Program.ServiceName.ToLower())) {
                    AddServiceFilePath(Program.ServiceName, filePath);
                }
            } else {
                AddServiceFilePath(FileUtils.GetFileName(filePath), filePath);
            } 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     adds the file name as the service name and adds the file path to a collection of paths
        ///     for that service.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  The name of the service to be added to. </param>
        /// <param name="filePath">     The fully qualified file path. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void AddServiceFilePath(string serviceName, string filePath)
        {
            ServiceLogger.WriteLine();
            if (!Program.serviceFilePaths.ContainsKey(serviceName)) {
                Program.serviceFilePaths.Add(serviceName, new List<string>());
            }
            Program.serviceFilePaths[serviceName].Add(filePath);
            ServiceLogger.WriteLine("--- Loaded File: " + filePath + " for conversion ---");
            Program.FileConfirmationMessage += filePath + "\n";
        }
    }
}
