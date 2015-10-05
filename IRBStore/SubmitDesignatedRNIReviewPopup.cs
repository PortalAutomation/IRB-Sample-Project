using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore.Popups;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SubmitDesignatedRNIReviewPopup : ActivityPopup
    {
        public SubmitDesignatedRNIReviewPopup(string projectId, string activityName)
            : base(projectId, activityName)
        {
        }

        public Radio RdoSubmitReviewYes = new Radio(By.CssSelector("input[name='_IRBSubmission_SubmitRNIDesignatedReview.customAttributes.readyForSubmission'][value='yes']"));
        public Radio RdoSubmitReviewNo = new Radio(By.CssSelector("input[name='_IRBSubmission_SubmitRNIDesignatedReview.customAttributes.readyForSubmission'][value='no']"));
        public Checkbox ChkConflictingInterest = new Checkbox(By.CssSelector("input[name='_IRBSubmission_SubmitRNIDesignatedReview.loggedFor.customAttributes.nonCommitteeReviewChecklist.customAttributes.conflictingInterest']"));

        /// <summary>
        /// Shared codebase -- same as SubmitRNICommitteeReview
        /// </summary>
        /// <param name="option"></param>
        public void SelectDetermination(Determinations value)
        {
            string name = "";
            switch (value)
            {
                case Determinations.UnanticipatedProblem: { name = "Unanticipated problem involving risks to subjects or others"; break; }
                case Determinations.SuspensionOrTermination: { name = "Suspension or termination of IRB approval"; break; }
                case Determinations.SeriousNonCompliance: { name = "Serious non-compliance"; break; }
                case Determinations.ContinuingNonCompliance: { name = "Continuing non-compliance"; break; }
                case Determinations.NonComplianceNotSerious: { name = "Non-compliance that is neither serious nor continuing"; break; }
                case Determinations.AllegationOfNonCompliance: { name = "Allegation of non-compliance with no basis in fact"; break; }
                case Determinations.NoneOfTheAbove: { name = "None of the above"; break; }
                case Determinations.AdditionalReviewRequired: { name = "Additional review required"; break; }
            }
            var chkbox = new Checkbox(By.XPath(".//td[text()='" + name + "']/../td/table/tbody/tr/td/input[1]"));
            chkbox.Click();
            Trace.WriteLine("Checking option: " + value);
        }
        
        public void SpecifyReadyToSubmit(bool value = true)
        {
            if (value)
            {
                RdoSubmitReviewYes.Click();
            }
            else
            {
                RdoSubmitReviewNo.Click();
            }
        }

        public enum Determinations
        {
            UnanticipatedProblem,
            SuspensionOrTermination, SeriousNonCompliance,
            ContinuingNonCompliance,
            NonComplianceNotSerious,
            AllegationOfNonCompliance,
            NoneOfTheAbove,
            AdditionalReviewRequired
        }

        
    }
}
