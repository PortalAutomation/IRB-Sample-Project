using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    public class SubmitCommitteeReviewPopup : ActivityPopup
    {
        public SubmitCommitteeReviewPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public void SelectStudyDeterminations(Determinations value)
        {
            string name = "";
            switch (value)
            {
                case Determinations.Approved: { name = "Approved"; break; }
                case Determinations.ModificationsRequiredToSecureApproved: { name = @"Modifications Required to Secure ""Approved"""; break; }
                case Determinations.NotHumanResearch: { name = "Not Human Research"; break; }
                case Determinations.ModificationsRequiredToSecureNotHumanResearch: { name = @"Modifications Required to Secure ""Not Human Research"""; break; }
                case Determinations.HumanResearchNotEngaged: { name = @"Human Research, Not Engaged"; break; }
                case Determinations.ModificationsRequiredToSecureHumanResearchNotEngaged: { name = @"Modifications Required to Secure ""Human Research, Not Engaged"""; break; }
                case Determinations.Deferred: { name = @"Deferred"; break; }
                case Determinations.Disapproved: { name = @"Disapproved"; break; }
            }
            var rdo = new Radio(By.XPath(".//td[text()='" + name + "']/../td/input[1]"));
            rdo.Click();
            Trace.WriteLine("Checking option: " + value);
        }

        public enum Determinations
        {
            Approved,
            ModificationsRequiredToSecureApproved,
            NotHumanResearch,
            ModificationsRequiredToSecureNotHumanResearch,
            HumanResearchNotEngaged,
            ModificationsRequiredToSecureHumanResearchNotEngaged,
            Deferred,
            Disapproved
        }

        //TODO implement other inputs in this popup
        public Radio
            RdoRdyForSubmissionYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.customAttributes.readyForSubmission'][value='yes']")),
            RdoRdyForSubmissionNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.customAttributes.readyForSubmission'][value='no']"));
           
        public TextBox
            TxtFor =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesYesCount']")),
            TxtAgainst =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesNoCount']")),
            TxtRecused =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesRecusedCount']")),
            TxtAbsent =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesAbsentCount']")),
            TxtAbstained =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.votesAbstainCount']")),

            TxtSubstitutions = new TextBox(By.CssSelector(
                        "textarea[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.currentAgendaItem.customAttributes.voteInformation.customAttributes.substitutions_text']")),
                       
            TxtRecommendedChangesReasons = new TextBox(By.CssSelector(
                        "textarea[name='_IRBSubmission_RecordMeetingDecision.loggedFor.customAttributes.requiredModification_text']"));


        public Radio
            RdoReadyToSubmitYes = new Radio(By.CssSelector("input[name='_IRBSubmission_RecordMeetingDecision.customAttributes.readyForSubmission'][value='yes']")),
            RdoReadyToSubmitNo = new Radio(By.CssSelector("input[name='_IRBSubmission_RecordMeetingDecision.customAttributes.readyForSubmission'][value='no']"));

        public TextBox TxtLastApprovalDate = new TextBox(By.Id("webr_uniqueID_1"));

        public void SelectRiskLevel(bool isGreaterThanMinimalRisk)
        {
            string name = "";
            if (isGreaterThanMinimalRisk)
            {
                name = "Greater than minimal risk";
            }
            else
            {
                name = "No greater than minimal risk";
            }
            var rdo = new Radio(By.XPath(".//td[text()='" + name + "']/../td/input[1]"));
            rdo.Click();
        }

        public void SpecifyReadyToSubmit(bool value = true)
        {
            if (value)
            {
                RdoRdyForSubmissionYes.Click();
            }
            else
            {
                RdoRdyForSubmissionNo.Click();
            }
        }

    }
}
