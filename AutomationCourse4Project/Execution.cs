using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationCourse4Project
{
    [TestFixture]
    public class Execution : Core
    {
        string user_name = "test_626103309";
        string email = "test_626103309@test.com";
        string password = "test123";

        [OneTimeSetUp]
        public void SetUp()
        {
            Initialize();
        }

        [Test]
        public void test01_GoToApp()
        {
            bool passed = false;

            driver.Navigate().GoToUrl("http://ornament.esy.es/");
            passed = GetElement("//img[@alt='Lucky_11']").Displayed;

            Assert.IsTrue(passed, "Sajt je nedostupan");
        }

        [Test]
        public void test02_RegisterNewAccount()
        {
            bool passed = false;

            SystemClick("//a[@data-original-title='Your Account']");

            Thread.Sleep(1000);
            SystemClick("//input[@value='Register']");

            int timeStamp = GetTimeStamp();

            email = "test_" + timeStamp + "@test.com";
            user_name = "test_" + timeStamp;
            password = "test123";

            Console.WriteLine("Registracija novog accounta: username='" + user_name + "' password='" + password + "'");

            TypeText("//input[@name='tbEmail']", email);
            TypeText("//input[@name='tbUserName']", user_name);
            TypeText("//input[@name='tbPassword']", password);
            TypeText("//input[@name='tbPassword2']", password);
            SystemClick("//input[@name='btnRegister']");

            passed = GetElement("//span[@class='label label-warning']").Text.Contains("You can now login with your data");

            Assert.IsTrue(passed, "Nije uspesna registracija");
        }

        [Test]
        public void test03_Login()
        {
            bool passed = false;

            SystemClick("//a[@data-original-title='Your Account']");

            TypeText("//input[@name='tbUserName']", user_name);
            TypeText("//input[@name='tbPassword']", password);
            SystemClick("//input[@name='btnLogin']");

            passed = driver.Url.Contains("/account");

            Assert.IsTrue(passed, "Logovanje nije uspesno");
        }

        [Test]
        public void test04_ValidateNewAccount()
        {
            

            bool passed = true;
            string message = "";

            if (!GetElement("//h2[@class='page-title']").Text.ToLower().Contains("your account must be completed to do scheduling"))
            {
                passed = false;
                message = "Labela 'Account' prikazuje losu poruku\n";
            }
            else
                Console.WriteLine("Labela 'Account' prikazuje odgovarajucu poruku");

            string mailValue = GetElement("//input[@name='email']").GetAttribute("value");

            if (!mailValue.ToLower().Contains(email.ToLower()))
            {
                passed = false;
                message += "Registrovan mail i mail iz prikaza se ne podudaraju\n";
            }
            else
                Console.WriteLine("Registrovan mail i mail iz prikaza se podudaraju");

            string userValue = GetElement("//input[@name='user']").GetAttribute("value");

            if (!userValue.ToLower().Contains(user_name.ToLower()))
            {
                passed = false;
                message += "Registrovan username i username iz prikaza se ne podudaraju\n";
            }
            else
                Console.WriteLine("Registrovan username i username iz prikaza se podudaraju");

            /*
             * div[@class='account-menu']/ul/li[1]/a
             * a[contains(@href,'/account') and contains(.,'Schedule')]
            */
            string url = GetElement("//div[@class='account-menu']/ul/li[1]/a").GetAttribute("href");
            SystemClick("//div[@class='account-menu']/ul/li[1]/a");
            if (!UrlStaysTheSame(url))
            {
                passed = false;
                message += "Uspesno je otisao na ne dozvoljenu stranicu\n";
            }
            else
                Console.WriteLine("Ocekivano ne moze da ode na 'Scheduele' stranicu");

            Assert.IsTrue(passed, message);
        }

        [Test]
        public void test05_FinalizeAccount()
        {
            bool passed = true;
            string message = "";

            SystemClick("//input[@value='Confirm']");
            string alertMessage = GetElement("//div[@class='alert alert-danger']").Text;

            if (!alertMessage.ToLower().Contains("the full name field is required"))
            {
                passed = false;
                message += "Ne prikazuje se poruka da je polje 'Full name' obavezno";
            }
            else
                Console.WriteLine("Prikazuje se poruka da je polje 'Full name' obavezno");

            if (!alertMessage.ToLower().Contains("the phone number field is required"))
            {
                passed = false;
                message += "Ne prikazuje se poruka da je polje 'Phone number' obavezno";
            }
            else
                Console.WriteLine("Prikazuje se poruka da je polje 'Phone number' obavezno");

            TypeText("//input[@name='fullname']", "Aleksandar Kostic");
            TypeText("//input[@name='phone']", "123123123");

            SystemClick("//input[@value='Confirm']");

            Thread.Sleep(1000);

            string successMessage = GetElement("//div[@class='alert alert-success']").Text;
            
            if (!successMessage.ToLower().Contains("success"))
            {
                passed = false;
                message += "Neuspesno kompletiranje account-a";
            }
            else
            {
                Console.WriteLine("Uspesno kompletiranje account-a");

                Thread.Sleep(3000);
                if (!IsAtUrl("account/Login"))
                {
                    passed = false;
                    message += "Automatska odjava ne funckionise";
                }
                else
                    Console.WriteLine("Automatska odjava funckionise");
            }

            Assert.IsTrue(passed, message);
        }

        [Test]
        public void test06_Login()
        {
            bool passed = false;

            SystemClick("//a[@data-original-title='Your Account']");

            TypeText("//input[@name='tbUserName']", user_name);
            TypeText("//input[@name='tbPassword']", password);
            SystemClick("//input[@name='btnLogin']");

            passed = driver.Url.Contains("/account");
            TakeScreenshoot("test");
            Assert.IsTrue(passed, "Logovanje nije uspesno");
        }

        [Test]
        public void test07_ValidateScheduleInitalDisplay()
        {
            bool passed = true;
            string message = "";

            if (!IsElementPresent("//table[@class='schedule_table']"))
            {
                passed = false;
                message += "Tabela scheduling nije ucitana\n";
                TakeScreenshoot("logged_schedule_table");
            }
            else
                Console.WriteLine("Tabela scheduling je ucitana\n");

            if (!IsElementPresent("//div[@class='account-menu']/ul/li[1]/a/font[@class='active_acc']"))
            {
                passed = false;
                message += "Schedule link ne pokazuje da smo na pravoj stranici\n";
                TakeScreenshoot("logged_schedule_link");
            }
            else
                Console.WriteLine("Schedule link pokazuje da smo na pravoj stranici\n");

            Assert.IsTrue(passed, message);
        }

        [Test]
        public void test08_ValidateWeekShift()
        {
            bool passed = true;
            string message = "";

            string lMonday1 = GetElement("//table[@class='schedule_table']/tbody/tr[1]/td[2]").Text;
            DateTime dateWeek1 = DateTime.ParseExact(lMonday1, "ddd, dd-MM", System.Globalization.CultureInfo.InvariantCulture);

            Console.WriteLine(dateWeek1.ToString("MM-dd-yyyy"));

            SystemClick("//input[@value='Next week']");
            Thread.Sleep(1000);

            string lMonday2 = GetElement("//table[@class='schedule_table']/tbody/tr[1]/td[2]").Text;
            DateTime dateWeek2 = DateTime.ParseExact(lMonday2, "ddd, dd-MM", System.Globalization.CultureInfo.InvariantCulture);

            Console.WriteLine(dateWeek2.ToString("MM-dd-yyyy"));

            dateWeek1 = dateWeek1.AddDays(7);

            if (!dateWeek1.Equals(dateWeek2))
            {
                passed = false;
                message += "Pomeranje kalendara u desno ne radi";
                TakeScreenshoot("logged_schedule_table_shift_next");
            }
            else
                Console.WriteLine("Pomeranje kalendara u desno radi");

            SystemClick("//input[@value='Previous week']");
            Thread.Sleep(1000);

            string lMonday3 = GetElement("//table[@class='schedule_table']/tbody/tr[1]/td[2]").Text;
            DateTime dateWeek3 = DateTime.ParseExact(lMonday3, "ddd, dd-MM", System.Globalization.CultureInfo.InvariantCulture);

            Console.WriteLine(dateWeek3.ToString("MM-dd-yyyy"));

            dateWeek1 = dateWeek1.AddDays(-7);

            if (!dateWeek1.Equals(dateWeek3))
            {
                passed = false;
                message += "Pomeranje kalendara u levo ne radi";
                TakeScreenshoot("logged_schedule_table_shift_previous");
            }
            else
                Console.WriteLine("Pomeranje kalendara u levo radi");

            Assert.IsTrue(passed, message);
        }
    }
}
