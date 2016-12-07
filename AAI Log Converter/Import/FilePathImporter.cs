using System;
using System.Collections.Generic;

namespace AAI_Log_Converter
{
    class FilePathImporter
    {
        public static ColumnInfo CallLogInfo = new ColumnInfo();
        private static FileImporter fileImporter = new FileImporter();

        public static void ParseArgs(string[] args)
        {
            if (!Program.callLogData.ContainsKey(Program.CallLogName)) {
                Program.callLogData.Add(Program.CallLogName, new BigList());
            }
            foreach (string path in args) {
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

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
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

        //Process the files
        private static void ProcessFile(string filePath)
        {
            if (Program.ServiceName != null && !Program.ServiceName.Equals("")) {
                if (filePath.ToLower().Contains(Program.ServiceName.ToLower())) {
                    AddServiceFilePath(Program.ServiceName, filePath);
                }
            } else {
                AddServiceFilePath(FileUtils.GetFileName(filePath), filePath);
            } 
        }

        private static void AddServiceFilePath(string serviceName, string filePath)
        {
            if (!Program.serviceFilePaths.ContainsKey(serviceName)) {
                Program.serviceFilePaths.Add(serviceName, new List<string>());
            }
            Program.serviceFilePaths[serviceName].Add(filePath);
        }
    }
}
