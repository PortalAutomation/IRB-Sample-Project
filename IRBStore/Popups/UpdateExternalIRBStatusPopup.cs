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
    class UpdateExternalIRBStatusPopup : ActivityPopup
    {
        public UpdateExternalIRBStatusPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Radio
            RdoExternalIRBClosedYes = new Radio(By.CssSelector("input[name='_IRBSubmission_Update External IRB Status.customAttributes.closeStudy'][value='yes']")),
            RdoExternalIRBClosedNo = new Radio(By.CssSelector("input[name='_IRBSubmission_Update External IRB Status.customAttributes.closeStudy'][value='no']"));
    }
}
