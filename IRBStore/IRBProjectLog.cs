using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages.Components;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class IRBProjectLog : ProjectLogComponent
    {
        public IRBProjectLog(string displayName) : base(displayName)
        {
        }

        public Link 
            HistoryTab = new Link(By.LinkText("History")),
            FundingTab = new Link(By.LinkText("Funding")),
            ProjectContactsTab = new Link(By.LinkText("Project Contacts")),
            DocumentsTab = new Link(By.LinkText("Documents")),
            FollowOnSubmissionsTab = new Link(By.LinkText("Follow-on Submissions")),
            ReviewsTab = new Link(By.LinkText("Reviews")),
            SnapshotsTab = new Link(By.LinkText("Snapshots"));

        /// <summary>
        /// Verify the first link the history log
        /// </summary>
        /// <param name="textLink"></param>
        /// <returns></returns>
        public bool VerifyFirstRowLink(string textLink)
        {
            HistoryTab.Click();
            //Link firstLink = new Link(By.XPath(".//tr[@data-drsv-row='0']/td[2]/span/a"));
            //Link firstLink = new Link(By.XPath("//a[text()='" + textLink + "']"));
            Link firstLink = new Link(By.XPath("//a[contains(text(),'" + textLink + "')]"));
            Wait.Until(h => firstLink.Exists);
            if (firstLink.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
