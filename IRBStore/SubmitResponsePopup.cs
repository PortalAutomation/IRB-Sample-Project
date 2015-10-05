using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SubmitResponsePopup : ActivityPopup
    {
        public SubmitResponsePopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        // Add buton  TODO
        public TextBox TxtInfo = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_SubmitChanges.notesAsStr']"));
        
    }
}
