using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class RequestPreReviewClarificationPopup : ActivityPopup
    {
        public RequestPreReviewClarificationPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        // Add buton  TODO
        public TextBox TxtInfo = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_RequestClarification.notesAsStr']"));

    }
}
