using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationCourse4Project
{
    public class Core
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;
        protected Actions builder;
        protected IJavaScriptExecutor scriptExec;

        public void Initialize()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(options);

            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            builder = new Actions(driver);
            scriptExec = (IJavaScriptExecutor)driver;
        }

        public IWebElement GetElement(string sXPath)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(sXPath)));
            IWebElement tmpEl = driver.FindElement(By.XPath(sXPath));
            return tmpEl;
        }

        public void SystemClick(string sXPath)
        {
            IWebElement tmpEl = GetElement(sXPath);
            scriptExec.ExecuteScript("window.scrollTo(0, arguments[0])", tmpEl.Location.Y);
            wait.Until(ExpectedConditions.ElementToBeClickable(tmpEl));
            tmpEl.Click();
        }

        public void TypeText(string sXPath, string Value)
        {
            IWebElement tmpEl = GetElement(sXPath);
            scriptExec.ExecuteScript("window.scrollTo(0, arguments[0])", tmpEl.Location.Y);
            tmpEl.SendKeys(Value);
        }

        public int GetTimeStamp()
        {
            DateTime date = DateTime.Now;
            int tmpTime = Int32.Parse(date.ToString("MMddHHmmss"));
            return tmpTime;
        }

        public bool UrlStaysTheSame(string newUrl, int timeout=5)
        {
            bool passed = false;

            wait.Timeout = TimeSpan.FromSeconds(timeout);

            try
            {
                wait.Until(ExpectedConditions.UrlToBe(newUrl));
            }

            catch (Exception)
            {
                passed = true;
            }

            wait.Timeout = TimeSpan.FromSeconds(30);

            return passed;
        }

        public bool IsAtUrl(string partialUrl)
        {
            return driver.Url.Contains(partialUrl);
        }

        public bool IsElementPresent(string sXPath, int timeout = 5)
        {
            bool passed = false;

            wait.Timeout = TimeSpan.FromSeconds(timeout);

            try
            {
                GetElement(sXPath);
                Console.WriteLine("Elenment '" + sXPath + "' uspesno pronadjen na stranici");
                passed = true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Element '" + sXPath + "' se ne nalazi na stranici (cekano " + timeout + " sekundi)");
            }

            wait.Timeout = TimeSpan.FromSeconds(30);
            return passed;
        }

        public void TakeScreenshoot(string fName)
        {
            ITakesScreenshot screen = (ITakesScreenshot)driver;
            string folderName = "report_" + DateTime.Now.ToString("yyyy_MM_dd_hh");

            if (!Directory.Exists(@"C:\ReportSS"))
                Directory.CreateDirectory(@"C:\ReportSS");

            if (!Directory.Exists(@"C:\ReportSS\" + folderName))
                Directory.CreateDirectory(@"C:\ReportSS\" + folderName);

            screen.GetScreenshot().SaveAsFile(@"C:\ReportSS\" + folderName + "\\" + fName+".jpg", ScreenshotImageFormat.Jpeg);
        }
    }
}
