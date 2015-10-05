using System;
using System.Diagnostics;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SubmitPreReviewPopup : ActivityPopup
    {
        public CCElement

            ChkBoxBioMedicalClinical =
                new CCElement(
                    By.CssSelector(
                        "input[name='_IRBSubmission_ConductPreReview.loggedFor.customAttributes.preReviewChecklist.customAttributes.researchType_setItem']"));
            
        public Radio
            RdoRiskLevelGreater =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_ConductPreReview.loggedFor.customAttributes.preReviewChecklist.customAttributes.riskLevel']")),
            RdoRiskLevelYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_IRBSubmission_ConductPreReview.loggedFor.customAttributes.preReviewChecklist.customAttributes.riskLevel']")),
            RadioBtnSubmitPreReviewYes =
                new Radio(
                    By.CssSelector(
                        "input[type='radio'][value='yes'][name='_IRBSubmission_ConductPreReview.customAttributes.readyForSubmission']")),
            RadioBtnSubmitPreReviewNo =
                new Radio(
                    By.CssSelector(
                        "input[type='radio'][value='no'][name='_IRBSubmission_ConductPreReview.customAttributes.readyForSubmission']"));

        public SubmitPreReviewPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        // regulator oversight

        // special determinations and waivers

        /// <summary>
        /// TODO Need to reimplement this as radio buttons are identical
        /// </summary>
        /// <param name="greaterThanMinRisk"></param>
        public void SpecifyRiskLevel(bool greaterThanMinRisk = true)
        {
            if (greaterThanMinRisk)
            {
                RdoRiskLevelGreater.Click();
            }
            else
            {
                RdoRiskLevelYes.Click();
            }
        }

        public void SpecifyResearchTypes(params TypeOfResearch[] researchTypes)
        {
            string name = "";
            foreach (var type in researchTypes)
            {
                switch (type)
                {
                    case TypeOfResearch.BiomedicalClinical:
                    {
                        name = "Biomedical";
                        break;
                    }
                    case TypeOfResearch.SocialBehavioralEducation:
                    {
                        name = "Social";
                        break;
                    }
                    case TypeOfResearch.Other:
                    {
                        name = "Other";
                        break;
                    }
                }
                var chkbox = new Checkbox(By.XPath(".//td[contains(.,'" + name + "')]/../td/table/tbody/tr/td/input[1]"));
                chkbox.Click();
                Trace.WriteLine("Checking option: " + type);

            }
        }

        public void SubmitPreview(bool value = true)
        {
            if (value)
            {
                RadioBtnSubmitPreReviewYes.Click();
            }
            else
            {
                RadioBtnSubmitPreReviewNo.Click();
            }
        }

        public enum TypeOfResearch
        {
            BiomedicalClinical,
            SocialBehavioralEducation,
            Other
        }



    }
}
