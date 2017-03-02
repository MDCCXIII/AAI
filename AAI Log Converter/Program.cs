using AAI_Log_Converter.Export;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using JR.Utils.GUI.Forms;

namespace AAI_Log_Converter
{

    class Program
    {
        /// <summary>   Name of the call log. </summary>
        public const string CallLogName = "Call Log";

        /// <summary>   The date header. </summary>
        public const string Header_Date = "Date";

        /// <summary>   The time header. </summary>
        public const string Header_Time = "Time";

        /// <summary>   The partner identifier header. </summary>
        public const string Header_PartnerID = "Partner ID";

        /// <summary>   The service name header. </summary>
        public const string Header_ServiceName = "Service Name";


        /// <summary>   Name of the service to be converted from the App.config. </summary>
        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        /// <summary>   Pathname of the output directory. </summary>
        public static string OutputDirectory = "";

        /// <summary>   Pathname of the working directory. </summary>
        public static string WorkingDirectory = Directory.GetCurrentDirectory();

        /// <summary>   The service file paths. </summary>
        public static Dictionary<string, List<string>> serviceFilePaths = new Dictionary<string, List<string>>();

        /// <summary>   The service columns. </summary>
        public static Dictionary<string, OrderedDictionary> serviceColumns = new Dictionary<string, OrderedDictionary>();

        /// <summary>   Number of column nulls. </summary>
        public static Dictionary<string, Dictionary<string, int>> columnNullCount = new Dictionary<string, Dictionary<string, int>>();
        //each parameter as a column in a service and the count of how many times that parameters value was empty

        /// <summary>   Number of column empties. </summary>
        public static Dictionary<string, Dictionary<string, int>> columnEmptyCount = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>   Number of times a column has been used. </summary>
        public static Dictionary<string, Dictionary<string, int>> columnSeenCount = new Dictionary<string, Dictionary<string, int>>();

        public static string FileConfirmationMessage = "";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     This is the main entry point of the application. This application optionally takes in
        ///     directory or file paths as arguments use the App.Config file to control execution without
        ///     arguments.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/12/2016. </remarks>
        ///
        /// <param name="args"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [STAThread]
        static void Main(string[] args)
        {
            // Hide the console window
            NativeMethods.ShowWindow(NativeMethods.GetConsoleWindow(), NativeMethods.SW_HIDE);

            ServiceLogger.CreateLogFileDirectory();
            ServiceLogger.WriteLine();
            OutputDirectory = GetOutputDirectory();
            FileUtils.ValidFileExtensions.Add(".txt");
            try
            {
                //if no arguments where passed set the default source directory from the appconfig
                if (args.Length == 0) {
                    if(!ConfigurationManager.AppSettings.AllKeys.Contains("DefaultSourceDirectory"))
                    {
                        throw new Exception("App.config, Key: DefaultSourceDirectory, missing. Please pass source directory as argument.");
                    }
                    if (ConfigurationManager.AppSettings["DefaultSourceDirectory"].Equals(""))
                    {
                        throw new Exception("App.config, Key: DefaultSourceDirectory, value is empty. Please pass source directory as argument.");
                    }
                    args = new string[] { ConfigurationManager.AppSettings["DefaultSourceDirectory"] };
                }

                //Gather the filePaths into the serviceFilePath collection
                FilePathImporter.ParseArgs(args);

                if(FlexibleMessageBox.Show("The following files have been loaded for conversion:\n" + FileConfirmationMessage + "\n\nWould you like to start the conversion?", "AAI Log Converter - Continue?", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
                {
                    

                    Thread t = new Thread(new ThreadStart(() => { MessageBox.Show("The Conversion will continue in the background.\nFeel free to continue working while you wait.", "AAI Log Converter - Info", MessageBoxButtons.OK); }));
                    t.Start();

                    //start the conversion process
                    StartProc();

                    if(MessageBox.Show("The log file conversion has Sucessfully Completed.\nWould you like to open the output directory now?", "AAI Log Converter - Success!", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
                    {
                        Process.Start(OutputDirectory);
                    }
                    
                }
                else
                {
                    ServiceLogger.WriteLine("End user did not accept prompt to continue.");
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.WriteLine("ERROR:: " + ex.Message, new StackTrace(ex).GetFrame(0).GetMethod().Name, ex.Source, new StackTrace(ex).GetFrame(0).GetFileLineNumber());
                MessageBox.Show(ex.Message, "AAI Log Converter - ERROR", MessageBoxButtons.OK);
            }
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     This is a 4 step process 1. gather the file paths for each service into a collection of
        ///     service names to file paths 2. for each file parse the parameter names seen by a single
        ///     service into unique column names 3. for each file write the data obtained from a single
        ///     service call into a the target files as a new row under their appropriate column 4.
        ///     repeat steps 2 and 3 foreach service.
        /// </summary>
        ///
        /// <remarks>   Ahaynes, 12/12/2016. </remarks>
        ///
        /// ### <param name="args"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void StartProc()
        {
            ServiceLogger.WriteLine();
            FileImporter fileImporter = new FileImporter();
            //Write column headers for the call log only once
            CallLogBuilder.WriteColumnHeaders();
            //itterate through each service 
            foreach (KeyValuePair<string, List<string>> kvp in serviceFilePaths)
            {
                //itterate through each file for this service
                foreach (string filePath in kvp.Value)
                {
                    //gather parameter names used in all calls of a service into the serviceColumns collection
                    fileImporter.ImportColumns(kvp.Key, filePath);
                }

                //print column headers to their appropriate files
                DataSheetBuilder.WriteColumnHeaders(kvp.Key);
                UsageStatisticsBuilder.WriteColumnHeaders(kvp.Key);

                //itterate through each file for this service again
                foreach (string filePath in kvp.Value)
                {
                    //Gather data per service call adding counts to the count collections (columnNullCount, columnEmptyCount, columnSeenCount)
                    //prints data per service call to the call log and data sheets
                    fileImporter.ImportValues(kvp.Key, filePath);
                }
                //print all of the statistics of a service to the usage statics sheet at once
                UsageStatisticsBuilder.AppendRowToFile(kvp.Key);
                // clear the collections for this service, they are no longer needed in memory
                ClearServiceCollections(kvp);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Clears the service collections. </summary>
        ///
        /// <remarks>   Ahaynes, 12/12/2016. </remarks>
        ///
        /// <param name="kvp">  The kvp. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void ClearServiceCollections(KeyValuePair<string, List<string>> kvp)
        {
            ServiceLogger.WriteLine();
            serviceColumns[kvp.Key].Clear();
            serviceColumns.Clear();
            columnEmptyCount.Clear();
            columnNullCount.Clear();
            columnSeenCount.Clear();
        }

        private static string GetOutputDirectory()
        {
            ServiceLogger.WriteLine();
            string result = Directory.GetCurrentDirectory() + "\\Converted AAI Logs\\";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("OutputDirectory"))
            {
                if (!ConfigurationManager.AppSettings["OutputDirectory"].Equals(""))
                {
                    result = ConfigurationManager.AppSettings["OutputDirectory"];
                }
            }
            FileUtils.CreateDirectory(result);
            return result;
        }
    }
}
