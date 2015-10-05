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
    public class ReviewRequiredModificationsPopup : ActivityPopup
    {
        public ReviewRequiredModificationsPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Radio
            RdoModsRequiredYes = new Radio(By.CssSelector("input[name='_IRBSubmission_RequiredModificationsVerified.customAttributes.confirm'][value='yes']")),
            RdoModsRequiredNo = new Radio(By.CssSelector("input[name='_IRBSubmission_RequiredModificationsVerified.customAttributes.confirm'][value='no']"));
    }
}
