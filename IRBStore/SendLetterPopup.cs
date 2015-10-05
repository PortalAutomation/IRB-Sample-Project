using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SendLetterPopup : ActivityPopup
    {
        //public CCElement
            //BtnOk = new CCElement(By.Id("okBtn")),
            //BtnCancel = new CCElement(By.Id("cancelBtn"));

        // Ajax operations being shown on this page

        public SendLetterPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }
}
