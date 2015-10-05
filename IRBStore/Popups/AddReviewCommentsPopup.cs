using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    class AddReviewCommentsPopup : ActivityPopup
    {
        public AddReviewCommentsPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public TextBox TxtReviewComments = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_SubmitCommitteeMemberReviewNotes.customAttributes.reviewingNotes_text']"));
    }
}
