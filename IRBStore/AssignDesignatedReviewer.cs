using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AssignDesignatedReviewer : ActivityPopup
    {
        public CCElement
            CmbDesignatedReviewer = new CCElement(By.CssSelector("select[name='_IRBSubmission_SendToDesignatedReviewer.loggedFor.customAttributes.primaryReviewer.customAttributes.reviewer']")),
            OkBtn = new CCElement(By.Id("okBtn"));

        public AssignDesignatedReviewer(string projectId, string activityName)
            : base(projectId, activityName)
        {
        }
    }
}
