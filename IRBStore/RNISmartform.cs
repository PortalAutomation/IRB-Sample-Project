using System;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class RNISmartform : SmartFormPage
    {
        public TextBox
            TxtRNIShortTitle = new TextBox(By.CssSelector("input[name='_IRBSubmission.name']")),
            TxtDateAware = new TextBox(By.CssSelector("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.dateOfAwareness']")),
            TxtDescriptionOfProblem = new TextBox(By.CssSelector
                ("textarea[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.descriptionOfProblem_text']"));

        public Radio
            RdoIsIncreasedRiskNo = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.isIncreasedRisk'][value='no']")),
            RdoIncreasedRiskYes = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.isIncreasedRisk'][value='yes']")),
            RdoNeedRevisionNo = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.requiresProtocolRevision'][value='no']")),
            RdoNeedRevisionYes = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.requiresProtocolRevision'][value='yes']")),
            RdoConsentRequiresRevisionkNo = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.consentRequiresRevision'][value='no']")),
            RdoConsentRequiresRevisionYes = new Radio(By.CssSelector
                ("input[name='_IRBSubmission.customAttributes.reportableNewInformation.customAttributes.consentRequiresRevision'][value='yes']"));
    }
}
