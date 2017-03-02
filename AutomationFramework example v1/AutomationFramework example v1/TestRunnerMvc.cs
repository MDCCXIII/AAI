using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework_example_v1
{
    class TestRunnerMvc:SiteModel
    {
        private static String baseUrl;
        TestResultUtility testResultUtility = new TestResultUtility();

        //static void Main(string[] args)
        //{
        //    //create a new test
        //    TestRunnerMvc test = new TestRunnerMvc();
        //    //execute Setup method
        //    test.SetUp();
        //    //run Launch Site and Login Method
        //    test.launchSiteAndLogin();
        //    //run Open User Setting Page Method
        //    test.openUserSettingPage();
        //    //run Change User Setting Method
        //    test.ChangeUserSettings();
        //    //run Logout Method
        //    test.Logout();
        //    //teardown the browser
        //    test.TearDown();
        //}
        //setup
        public void SetUp()
        {
            //Initialize test result string
            testResultUtility.InitializeTestResultString("Selenium Master Login Test Suite");
            //test base url
            baseUrl = "http://www.seleniummaster.com";
            try
            {
                //use firefox browser
                browser = new FirefoxDriver();
                //set browser time out time to 60 seconds
                browser.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
                //add test result to test result string when passed
                testResultUtility.AddTestPassToTestResultString("Test Setup", "Pass");

            }
            catch (Exception e)
            {
                //add message to the console
                Console.WriteLine(e.Message);
                Console.WriteLine("Cannot start Web Driver and Launch the browser");
                //add test result to test result string when failed
                testResultUtility.AddTestFailToTestResultString("Test Setup", "Fail");
            }

        }

        //Launches the Selenium Master Test Application and Login
        public void launchSiteAndLogin()
        {
            browser.Navigate().GoToUrl(baseUrl + "/seleniummastertestapp/index.php");
            //is site Logo displayed
            IsElementPresentByElementName(SiteModel.SeleniumMasterLogo(), "Selenium Master Logo", 60);
            //enter user name by using site model
            SiteModel.UserNameTextBox().Clear();
            SiteModel.UserNameTextBox().SendKeys("test");
            //enter password by using site model
            SiteModel.UserPasswordTextBox().Clear();
            SiteModel.UserPasswordTextBox().SendKeys("XXXXXXX"); //password is omitted
                                                                 //click on the submit button by using site model
            SiteModel.SubmitButton().Click();
            try
            {

                Assert.AreEqual(SiteModel.OnLineUserMessage().Text, "Test Selenium");
                testResultUtility.AddTestPassToTestResultString("Launch Site And Login Test", "Pass");
            }

            catch
            {
                testResultUtility.AddTestFailToTestResultString("Launch Site And Login Test", "Fail");
            }
        }

        //Navigates to the User Settings page
        public void openUserSettingPage()
        {
            //click on Settings link by using site model
            SiteModel.UserSettingsLink().Click();
            // is login user name text box displayed
            IsElementPresentByElementName(SiteModel.UserNameTextBox(), "User name box", 60);
            //enter user name by using site model
            SiteModel.UserNameTextBox().Clear();
            SiteModel.UserNameTextBox().SendKeys("test");
            //enter password by using site model
            SiteModel.UserPasswordTextBox().Clear();
            SiteModel.UserPasswordTextBox().SendKeys("XXXXXXX"); //password is omitted
                                                                 //click on the submit button by using site model
            SiteModel.SubmitButton().Click();
            //is user authorization radio button displayed
            IsElementPresentByElementName(SiteModel.AuthorizationRadioButton(), "user authrization button", 60);
            try
            {
                Assert.IsTrue(IsElementPresentByElementName(SiteModel.AuthorizationRadioButton(), "user authrization button", 60));
                testResultUtility.AddTestPassToTestResultString("Open User Setting Page Test", "Pass");
            }
            catch
            {
                testResultUtility.AddTestFailToTestResultString("Open User Setting Page Test", "Fail");

            }


        }

        //Change a User settings to add as a friends after authorization")
        public void ChangeUserSettings()
        {
            //click on the user authorization radio button click by using site model
            SiteModel.AuthorizationRadioButton().Click();
            //click on the save settings button by using site model
            SiteModel.SaveSettings().Click();
            //is preference saved label displayed by using site model
            IsElementPresentByElementName(SiteModel.PreferenceSavedMessageLabel(), "Save Settings", 60);
            try
            {
                Assert.AreEqual("Preferences saved", SiteModel.PreferenceSavedMessageLabel().Text);
                testResultUtility.AddTestPassToTestResultString("Change User Settings Test", "Pass");
            }

            catch
            {
                testResultUtility.AddTestFailToTestResultString("Change User Settings Test", "Fail");
            }

        }

        //Log out the system
        public void Logout()
        {
            //is logout link displayed
            IsElementPresentByElementName(SiteModel.LogoutLink(), "Logout link", 60);
            //click on logout link
            SiteModel.LogoutLink().Click();
            try
            {
                Assert.IsTrue(IsElementPresentByElementName(SiteModel.SubmitButton(), "Submit Button", 60));
                testResultUtility.AddTestPassToTestResultString("Logout Test", "Pass");
            }
            catch

            {
                testResultUtility.AddTestFailToTestResultString("Logout Test", "Fail");
            }

        }


        //tear down
        public void TearDown()
        {
            testResultUtility.EndTestResultString();
            testResultUtility.WriteToHtmlFile(testResultUtility.testResultHtmlString.ToString(), "SeleniumMasterLoginTestResult.html");
            browser.Quit();
        }
        //define a method that keep checking if an element is displayed until time reaches 60 seconds
        private bool IsElementPresentByElementName(IWebElement webElement, string elementName, int timeOutSeconds)
        {
            IWebElement currentElement;
            DateTime currentTime = DateTime.Now;
            TimeSpan duration;
            for (int second = 0; ; second++)
            {
                DateTime newTime = DateTime.Now;
                currentElement = webElement;
                duration = newTime - currentTime;
                if (currentElement.Displayed)
                {
                    Console.WriteLine("{0} is found", elementName);
                    break;
                }
                if (duration.TotalSeconds >= timeOutSeconds)
                {
                    Console.WriteLine("{0} is Not found in {1} seconds", elementName, duration.TotalSeconds);
                    break;
                }
            }
            return currentElement.Displayed;
        }
    }
}
