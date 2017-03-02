using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework_example_v1
{
    class TestResultUtility
    {
        public StringBuilder testResultHtmlString;

        public void InitializeTestResultString(String TestSuiteName)
        {
            testResultHtmlString = new StringBuilder();

            testResultHtmlString.Append("<html><header><title>").Append(TestSuiteName).Append("</title></header><body><table border=\"1\">");
            testResultHtmlString.Append("<tr style=\"background-color:#99CCFF\"><td><b> Test Case </b></td><td><b> Test Result </b></td></tr>");
        }

        public void AddTestPassToTestResultString(String TestMethod, String TestResult)
        {
            //add green color to the background if pass
            testResultHtmlString.Append("<tr style=\"background-color:#33CC33\"><td>").Append(TestMethod).Append("</td><td>").Append(TestResult).Append("</td></tr>");
        }

        public void AddTestFailToTestResultString(String TestMethod, String TestResult)
        {
            //add yellow color to the background if fail
            testResultHtmlString.Append("<tr style=\"background-color:#FFFF00\"><td>").Append(TestMethod).Append("</td><td>").Append(TestResult).Append("</td></tr>");

        }

        public void EndTestResultString()
        {
            testResultHtmlString.Append("</table></body></html>");
        }

        public void WriteToHtmlFile(String content, String filename)
        {
            //Create a file stream
            FileStream file = new FileStream(filename, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(file);
            //writing to the file
            sw.WriteLine(content);
            sw.Close();
        }
    }
}
