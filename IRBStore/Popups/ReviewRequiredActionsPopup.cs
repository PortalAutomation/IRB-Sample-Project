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
    public class ReviewRequiredActionsPopup : ActivityPopup
    {
        public ReviewRequiredActionsPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Radio
            RdoActionsCompletedYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_ReviewRequiredActions.loggedFor.customAttributes.reportableNewInformation.customAttributes.actionCompleted'][value='yes']")),
            RdoActionsCompletedNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_ReviewRequiredActions.loggedFor.customAttributes.reportableNewInformation.customAttributes.actionCompleted'][value='no']"));

        public TextBox
            TxtActionPlan =
                new TextBox(
                    By.CssSelector(
                        "textarea[name='_IRBSubmission_ReviewRequiredActions.loggedFor.customAttributes.reportableNewInformation.customAttributes.requiredAction_text']")),
            TxtNotes =
                new TextBox(
                    By.CssSelector(
                        "textarea[name='_IRBSubmission_ReviewRequiredActions.notesAsStr']"));
    }
}
