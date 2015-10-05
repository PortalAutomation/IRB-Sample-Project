using System.Diagnostics;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SubmitDesignatedReview : ActivityPopup
    {
        public CCElement
            ChkConflictingInterest =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.loggedFor.customAttributes.nonCommitteeReviewChecklist.customAttributes.conflictingInterest']")),
            // this will only select the first radio button for determination TODO: implement chooser
            RdoBtnFirstDetermination =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.loggedFor.customAttributes.irbDetermination']")),
            // this will only select the first radio button for review level TODO: implement chooser ("exempt")
            RdoFirstReviewLevel =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.loggedFor.customAttributes.nonCommitteeReviewChecklist.customAttributes.reviewLevel']")),
            // this will only select the first checkbox for exempt categories
            ChkFirstExemptCategory =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.loggedFor.customAttributes.nonCommitteeReviewChecklist.customAttributes.exemptDetermination_setItem']")),
            TxtLastDayApproval = new CCElement(By.Id("webr_uniqueID_0")),
            RdoReadyToSubmitThisReviewYes =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.customAttributes.readyForSubmission'][value='yes']")),
                        RdoReadyToSubmitThisReviewNo =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_SubmitDesignatedReview.customAttributes.readyForSubmission'][value='no']"));
            //BtnOk = new CCElement(By.Id("okBtn"));

        public TextBox TxtModification = new TextBox(By.CssSelector("textarea[name='_IRBSubmission_SubmitDesignatedReview.loggedFor.customAttributes.requiredModification_text']"));

        public SubmitDesignatedReview(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public void SelectStudyDeterminations(StudyDeterminations value)
        {
            string name = "";
            switch (value)
            {
                case StudyDeterminations.Approved: { name = "Approved"; break; }
                case StudyDeterminations.ModificationsRequiredToSecureApproved: { name = @"Modifications Required to Secure ""Approved"""; break; }
                case StudyDeterminations.NotHumanResearch: { name = "Not Human Research"; break; }
                case StudyDeterminations.ModificationsRequiredToSecureNotHumanResearch: { name = @"Modifications Required to Secure ""Not Human Research"""; break; }
                case StudyDeterminations.HumanResearchNotEngaged: { name = @"Human Research, Not Engaged"; break; }
                case StudyDeterminations.ModificationsRequiredToSecureHumanResearchNotEngaged: { name = @"Modifications Required to Secure ""Human Research, Not Engaged"""; break; }
            }
            var rdo = new Radio(By.XPath(".//td[text()='" + name + "']/../td/input[1]"));
            rdo.Click();
            Trace.WriteLine("Checking option: " + value);
        }

        public enum StudyDeterminations
        {
            Approved,
            ModificationsRequiredToSecureApproved,
            NotHumanResearch,
            ModificationsRequiredToSecureNotHumanResearch,
            HumanResearchNotEngaged,
            ModificationsRequiredToSecureHumanResearchNotEngaged
        }
    }
}
