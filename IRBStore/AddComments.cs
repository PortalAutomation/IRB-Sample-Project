using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AddComments : ActivityPopup
    {
        public TextBox
            TxtComment = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_Comment.notesAsStr']"));
        // TODO supporting documents, who should receive an e-mail notification


        public AddComments(string projectId, string activityName) : base(projectId, activityName)
        {

        }
    }

    public class AddPrivateComments : ActivityPopup
    {
        public TextBox
            TxtComment = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_PrivateComment.notesAsStr']"));
        // TODO supporting documents, who should receive an e-mail notification
        
        public AddPrivateComments(string projectId, string activityName)
            : base(projectId, activityName)
        {

        }
    }
}
