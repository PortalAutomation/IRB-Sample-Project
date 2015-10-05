using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AssignIRB : ActivityPopup
    {
        public AssignIRB(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Select
            SelIrbOffice = new Select(By.CssSelector("select[name='_IRBSubmission_AssignIRB.customAttributes.irbOffice']"));
    }
}
