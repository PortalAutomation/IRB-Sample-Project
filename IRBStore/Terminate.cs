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
    public class Terminate : ActivityPopup
    {
        public Terminate(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public TextBox TxtComments = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_Terminate.notesAsStr']"));
    }
}
