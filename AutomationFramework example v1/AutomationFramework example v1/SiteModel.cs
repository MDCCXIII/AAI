using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework_example_v1
{
    class SiteModel
    {
        protected static IWebDriver browser = null;

        //define Selenium Master Logo
        public static IWebElement SeleniumMasterLogo()
        {
            return browser.FindElement(By.CssSelector("img[alt=\"SeleniumMaster\"]"));
        }

        //define user name text box
        public static IWebElement UserNameTextBox()
        {
            return browser.FindElement(By.Id("login_login_username"));
        }

        //define user password text box
        public static IWebElement UserPasswordTextBox()
        {
            return browser.FindElement(By.Id("login_login_password"));
        }

        //define submit button
        public static IWebElement SubmitButton()
        {
            return browser.FindElement(By.Id("Login_submit"));
        }

        //define online user message label
        public static IWebElement OnLineUserMessage()
        {
            return browser.FindElement(By.CssSelector("ul.cr > li > a"));
        }

        //define user settings link
        public static IWebElement UserSettingsLink()
        {
            return browser.FindElement(By.LinkText("Settings"));
        }

        //define user authorization radio button
        public static IWebElement AuthorizationRadioButton()
        {
            return browser.FindElement(By.XPath("//input[@value='auth']"));
        }

        //define save settings button
        public static IWebElement SaveSettings()
        {
            return browser.FindElement(By.Id("accountprefs_submit"));
        }

        //define prefernce saved massage label
        public static IWebElement PreferenceSavedMessageLabel()
        {
            return browser.FindElement(By.CssSelector("div.ok"));
        }

        //define logout link
        public static IWebElement LogoutLink()
        {
            return browser.FindElement(By.LinkText("Logout"));
        }
    }
}
