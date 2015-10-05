using System;
using System.Diagnostics;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{

    public class ModCrStudyClosureSf : SmartFormPage
    {
        public readonly InitialModCrSmartForm InitialModCrSmartForm = new InitialModCrSmartForm();
        }

    public class InitialModCrSmartForm : SmartFormPage
    {

        public readonly TextBox
            TxtInvestigatorSiteTotal =
                new TextBox(
                    By.CssSelector(
                        "input[name='_IRBSubmission.customAttributes.continuingReviewProgressReport.customAttributes.enrollmentInfo.customAttributes.localSinceActivation']")),
            TxtStudyWideTotal = 
                new TextBox(
                    By.CssSelector("input[name='_IRBSubmission.customAttributes.continuingReviewProgressReport.customAttributes.enrollmentInfo.customAttributes.allSiteSinceActivation']")),

            TxtSinceLastApprovalTotal = new TextBox(By.CssSelector("input[name='_IRBSubmission.customAttributes.continuingReviewProgressReport.customAttributes.enrollmentInfo.customAttributes.localSinceLastApproval']")),

            TxtSummarizeModifications = new TextBox(By.CssSelector("textarea[name='_IRBSubmission.customAttributes.modificationDetails.customAttributes.modificationSummary_text']"));

        public readonly CCElement
            ContainerEntityView =
                new CCElement(By.XPath("/html/body/form/table[2]/tbody/tr/td/span[2]/span/table/tbody/tr/td/input")),
            // using OID -- susceptible to break if switching test data
            RdoModification =
                new CCElement(
                    By.CssSelector(
                        "input[type='radio'][value='com.webridge.entity.Entity[OID[4CFD3AD1EB89D34B8347AB87EA4E1A9E]]']")),
            RdoContinuingReview = new CCElement(By.Id("_webr_EntityView")),
            ChkAddStudyMemeber =
                new CCElement(
                    By.CssSelector(
                        "input[type='checkbox'][value='com.webridge.entity.Entity[OID[BF4CBE688150574391CC5E1ACFF7602C]]']"));

        public readonly Checkbox
            ChkOtherPartsOfStudy = new Checkbox(By.CssSelector("input[type='checkbox'][value='com.webridge.entity.Entity[OID[BF4CBE688150574391CC5E1ACFF7602C]]']"));
                
        public readonly Radio
            RdoFinancialInterestYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission.customAttributes.continuingReviewProgressReport.customAttributes.financialInterestInHUD'][value='yes']")),
            RdoFinancialInterestNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission.customAttributes.continuingReviewProgressReport.customAttributes.financialInterestInHUD'][value='no']"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public void ChooseResearchMilestone(params MileStones[] value)
        {
            string name = "";
            foreach (var milestone in value)
            {
                switch (milestone)
                {
                    case MileStones.StudyPermanentlyClosedToEnrollment:
                    {
                        name = "Study is permanently closed to enrollment OR was never open for enrollment";
                        break;
                    }
                    case MileStones.AllSubjectCompletedStudyRelatedInterventions:
                    {
//                        name = @"
//	                    All subjects have completed all study-related interventions OR not applicable (e.g. study did not include interventions, no subjects were enrolled)";
                        name = @"All subjects have completed";
                        break;
                    }
                    case MileStones.CollectionOfPrivateInfoComplete:
                    {
                        //name =
                        //    @"Collection of private identifiable information is complete OR not applicable (no subjects were enrolled)";
                        name = @"Collection of private";
                        break;
                    }
                    case MileStones.AnalysisOfPrivateInfoComplete:
                    {
                        //name =
                        //    @"Analysis of private identifiable information is complete OR not applicable (no subjects were enrolled)";
                        name = @"Analysis of private";
                        break;
                    }
                    case MileStones.RemainingStudyActivitiesLimited:
                    {
                        name = @"Remaining study activities are limited to data analysis";
                        break;
                    }
                    case MileStones.StudyRemainsActive:
                    {
                        name = @"Study remains active only for long-term follow-up of subjects";
                        break;
                    }
                }
                //var chkbox = new Checkbox(By.XPath(".//td[text()='" + name + "']/../td/table/tbody/tr/td/input[1]"));
                var chkbox = new Checkbox(By.XPath(".//td[contains(.,'" + name + "')]/../td/table/tbody/tr/td/input[1]"));
                chkbox.Click();
                Trace.WriteLine("Checking option: " + value);
            }
        }

        public enum MileStones
        {
            StudyPermanentlyClosedToEnrollment,
            AllSubjectCompletedStudyRelatedInterventions,
            CollectionOfPrivateInfoComplete,
            AnalysisOfPrivateInfoComplete,
            RemainingStudyActivitiesLimited,
            StudyRemainsActive
        }


        public void CheckItemsTrueSinceLastApproval()
        {
            throw new NotImplementedException();
        }

        public void ChooseModCRPurpose(SubmissionPurpose purpose)
        {
            string purposeString = "";

            switch (purpose)
            {
                case SubmissionPurpose.ContinuingReview:
                {
                    purposeString = "Continuing Review";
                    break;
                }
                case SubmissionPurpose.Modification:
                {
                    purposeString = "Modification";
                    break;
                }
                case SubmissionPurpose.ModAndCR:
                {
                    purposeString = "Modification and Continuing Review";
                    break;
                }
            }
            Radio rdoPurpose = new Radio(By.XPath(".//td[text()='" + purposeString + "']/../td[1]/input[1]"));
            rdoPurpose.Click();
        }

        public void ChooseModificationScope(Scope scope)
        {
            string name = "";
            if (scope == Scope.OtherPartsOfTheStudy)
            {
                name = "Other parts of the study";
            }
            if (scope == Scope.StudyTeamMemberInformation)
            {
                name = "Study team member information";
            }
            Checkbox scopePurpose = new Checkbox(By.XPath(".//td[text()='" + name + "']/../td[1]/table/tbody/tr/td/input[1]"));
            scopePurpose.Click();

        }

        public void SpecifyEnrollmentTotals(string atSite, string studyWide, string sinceLastApproval)
        {
            TxtInvestigatorSiteTotal.Value = atSite;
            TxtSinceLastApprovalTotal.Value = sinceLastApproval;
            TxtStudyWideTotal.Value = studyWide;
        }

    }

    public class ModificationInformation : SmartFormPage
    {
        public TextBox
            TxtSummary = new TextBox(By.CssSelector("textarea[name='_IRBSubmission.customAttributes.modificationDetails.customAttributes.modificationSummary_text']"));

    }

    public enum Scope
    {
        StudyTeamMemberInformation,
        OtherPartsOfTheStudy
    }

    public enum SubmissionPurpose
    {
        ContinuingReview,
        Modification,
        ModAndCR
    }


}
