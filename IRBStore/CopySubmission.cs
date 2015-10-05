using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class CopySubmission : ActivityPopup
    {

        public TextBox
            TxtNewSubmissionName = new TextBox(By.CssSelector("input[name='_IRBSubmission_CopyStudy.customAttributes.newStudyName']"));

        public CopySubmission(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }
}
