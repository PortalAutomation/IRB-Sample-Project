using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.Pages.Components;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class Inbox : PageElement
    {

        //public Inbox() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[13C56671E5C07547AA5B6CA87323B16E]]") { }
        public Link myInboxLink = new Link(By.LinkText("My Inbox"));
        public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("My Inbox");

        public Button
            ImgCreateNewStudyLink = new Button(By.CssSelector("img[alt='Create New Study']")),
            ImgCreateNewRNI = new Button(By.CssSelector("img[alt='Report New Information']"));
        
        public CCElement
            // refactor this as component
            DivMyInbox = new CCElement(By.Id("component06AA0B1592E1B24F96A8D37FD7F8B851")),
            LnkAdvanced = new CCElement(By.CssSelector("a[id*='advancedLink']")),
            QueryCriteria1 = new CCElement(By.CssSelector("input[id*='queryCriteria1']")),
            QueryCriteria2 = new CCElement(By.CssSelector("input[id*='queryCriteria2']")),
            QueryCriteria3 = new CCElement(By.CssSelector("input[id*='queryCriteria3']")),
            QueryField1 = new CCElement(By.CssSelector("select[id*='queryField1']")),
            QueryField2 = new CCElement(By.CssSelector("select[id*='queryField2']")),
            QueryField3 = new CCElement(By.CssSelector("select[id*='queryField3']")),
            BtnGo = new CCElement(By.CssSelector("input[id*='requery']")),
            BtnClear = new CCElement(By.CssSelector("input[id*='clearquery']"));

        public void NavigateTo()
        {
            Trace.WriteLine("Navigating to My Inbox");
            myInboxLink.Click();
        }

       public void OpenStudy(string name, bool partialMatch = false)
        {
            if (partialMatch)
            {
                var targetLink = new CCElement(By.PartialLinkText(name));
                targetLink.Click();
                Wait.Until(h => Web.PortalDriver.Title == name);
            }
            else
            {
                var targetLink = new CCElement(By.LinkText(name));
                targetLink.Click();
                Wait.Until(h => Web.PortalDriver.Title == name);
            }
            
        }

        
       
    }
}
