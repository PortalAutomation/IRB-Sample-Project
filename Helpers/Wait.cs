using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomation;

namespace IRB.Helpers
{
    public static class Wait
    {
        private readonly static WebDriverWait Pause = new WebDriverWait(CCWebUIAuto.webDriver, TimeSpan.FromSeconds(7));

        public static void Until(Func<IWebDriver, bool> func)
        {
            RetriableRunner.Run(() => Pause.Until(func));
        }
    }
}