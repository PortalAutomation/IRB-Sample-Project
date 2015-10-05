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
    public class AssignPIProxyPopup : ActivityPopup
    {
        public AssignPIProxyPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Container dataTable = new Container(By.CssSelector("table[id*='DataTable']"));
        public Checkbox firstUser = new Checkbox(
                By.CssSelector(
                    "input[name='_IRBSubmission_ManagePIProxies.loggedFor.customAttributes.parentStudy.customAttributes.piProxiesPerson_setItem'][type='checkbox']"));

        public void SelectFirstPerson()
        {
            firstUser.Click();
        }

    }
}
