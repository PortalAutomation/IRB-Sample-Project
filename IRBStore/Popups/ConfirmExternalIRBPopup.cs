using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;

namespace IRBAutomation.IRBStore.Popups
{
    public class ConfirmExternalIRBPopup : ActivityPopup
    {
        public ConfirmExternalIRBPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }
}
