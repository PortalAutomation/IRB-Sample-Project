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
    public class SubmitActionResponsePopup : ActivityPopup
    {
        public SubmitActionResponsePopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public TextBox TxtNotes = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_SubmitActionResponse.notesAsStr']"));
    }
}
