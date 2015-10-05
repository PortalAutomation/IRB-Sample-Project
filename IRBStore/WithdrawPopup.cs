using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class WithdrawPopup : ActivityPopup
    {
        public WithdrawPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public TextBox TxtComment = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_Withdraw.notesAsStr']"));
    }
}
