using IRB.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using WebAutomation;

namespace IRB.Helpers
{
	public class Store
	{
		public static string BaseUrl;
		public static UserAccount CurrentUser;

		public static string CurrentPageSource { get { return CCWebUIAuto.webDriver.PageSource; } }

		// This does two things
		// -logs us in and out of the store
		// -tracks who is currently logged in
		public static void LoginAsUser(UserAccount user)
		{
		    var loginPage = new LoginPage();
			if (CurrentUser == user) return;

			WebDriver.Navigate(BaseUrl);
			if (CurrentUser != null) {
				new CCElement(By.LinkText("Logoff")).Click();
			}
			if (user != null) {
				loginPage.Login(user.UserName, user.Password);
			}
			CurrentUser = user;		
		}

		public static UserAccount ExpectUserAccount(string user, string pwd)
		{
			return new UserAccount(user, pwd);
		}
        
		public static void WaitForPageLoad()
		{
			ClickPortalUI.Wait.Until(d => d.ExecuteJavaScript<string>("return document.readyState").Equals("complete"));
		}
	}

    // just describes a user
    public class UserAccount
    {
        public readonly string UserName;
        public readonly string Password;

        internal UserAccount(string user, string pwd)
        {
            UserName = user;
            Password = pwd;
        }
    }
}
