using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    public class SubmitRNICommitteeReviewPopup : ActivityPopup
    {
        public SubmitRNICommitteeReviewPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        /// <summary>
        /// 
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

        //TODO implement other inputs in this popup
        public Radio
            RdoRdyForSubmissionYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.customAttributes.readyForSubmission'][value='yes']")),
            RdoRdyForSubmissionNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.customAttributes.readyForSubmission'][value='no']")),
            RdoFurtherActionReqYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.reportableNewInformation.customAttributes.actionRequired'][value='yes']")),
            RdoFurtherActionReqNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.reportableNewInformation.customAttributes.actionRequired'][value='no']"));

        public TextBox
            TxtFor =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesYesCount']")),
            TxtAgainst =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesNoCount']")),
            TxtRecused =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesRecusedCount']")),
            TxtAbsent =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesAbsentCount']")),
            TxtAbstained =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesAbstainCount']")),
                 
            TxtActionPlan = new TextBox(By.CssSelector(
                        "textarea[name='_IRBSubmission_submitCommitteeRNIReview.loggedFor.customAttributes.reportableNewInformation.customAttributes.requiredAction_text']"));
        
        public Button
            BtnResponsibleParty = new Button(By.CssSelector("input[value='Select...'][class='Button2']"));

    }
}
