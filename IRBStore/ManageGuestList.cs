using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class ManageGuestList : ActivityPopup
    {
        public Button BtnAddGuest = new Button(By.CssSelector("input[value='Add']"));


        public ManageGuestList(string projectId, string activityName) : base(projectId, activityName)
        {
        }


    }
}
