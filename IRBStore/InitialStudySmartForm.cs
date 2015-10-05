using System;
using System.Collections.Generic;
using System.Linq;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore.Popups;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class InitialStudySmartForm : SmartFormPage
    {
        public CCElement
            //imgCreateNewStudyLink = new CCElement(By.CssSelector("img[alt='Create New Study'"));
            imgCreateNewStudyLink =
                new CCElement(
                    By.XPath(
                        "html/body/span[5]/span/span[2]/table/tbody/tr[1]/td/table/tbody/tr[2]/td[1]/span/span/span/div/table[2]/tbody/tr/td/span/div/span/span/span/span[2]/span/span/div/table[2]/tbody/tr/td/form/span[2]/a/img"));
        
        public NewSubmissionSmartform NewSubmissionSmartformPage = new NewSubmissionSmartform();
        public StudyTeamMembers StudyTeamMembersPage = new StudyTeamMembers();
        public SelectPerson SelectPersonPage = new SelectPerson("Person");
        public StudyScope StudyScopePage = new StudyScope();
        public ConsentFormsRecruitment ConsentFormsRecruitmentPage = new ConsentFormsRecruitment();
        public StudyModCustom StudyModCustomPage = new StudyModCustom();
        public FundingSources FundingSourcesPage = new FundingSources();
        public SupportingDocuments SupportingDocumentsPage = new SupportingDocuments();
        public Final FinalPage = new Final();
        public ExternalIRB ExternalIrbPage = new ExternalIRB();

    }

    //public class AddAttachment : SmartFormPage
    //{
    //    public static string windowTitle = "Add Attachment";

    //    public Button 
    //        BtnBrowse = new Button(By.CssSelector("input[name='_Attachment.customAttributes.draftVersion.targetURL']")),
    //        BtnOk = new Button(By.CssSelector("input[value='OK']"));
    //}

    //public class AddStudyTeamMember : SmartFormPage
    //{

    //    public Button 
    //        BtnSelectTeamMember = new Button(By.CssSelector("input[value='Select...']")),
    //        BtnOk = new Button(By.CssSelector("input[value='OK']"));

    //    public Radio
    //        RadioConsentProcessNo =
    //            new Radio(
    //                By.CssSelector(
    //                    "input[name='_StudyTeamMemberInfo.customAttributes.involvedInConsentProcess'][value='no']")),
    //        RadioConsentProcessYes =
    //            new Radio(
    //                By.CssSelector(
    //                    "input[name='_StudyTeamMemberInfo.customAttributes.involvedInConsentProcess'][value='yes']")),
    //        RadioFinancialInterestNo =
    //            new Radio(
    //                By.CssSelector(
    //                    "input[name='_StudyTeamMemberInfo.customAttributes.hasFinancialInterest'][value='no']")),
    //        RadioFinancialInterestYes =
    //            new Radio(
    //                By.CssSelector(
    //                    "input[name='_StudyTeamMemberInfo.customAttributes.hasFinancialInterest'][value='yes']"));
    //}

    public class FundingSources : SmartFormPage
    {
        public CCElement
            BtnAddFunding = new CCElement(By.XPath("/html/body/form/table[2]/tbody/tr/td/span[2]/span/ol/li/span/span[3]/table/tbody/tr/td/table/tbody/tr/td/input"));
    }

    public class NewSubmissionSmartform : SmartFormPage
    {

        public AddAttachmentPopup addAttachment = new AddAttachmentPopup();

        public TextBox
            TxtTitleStudy = new TextBox(By.CssSelector("textarea[name='_IRBSubmission.customAttributes.studyTeamDescription_text']")),
            TxtShortTitle = new TextBox(By.CssSelector("input[name='_IRBSubmission.name']")),
            TxtDescription = new TextBox(By.CssSelector("textarea[name='_IRBSubmission.description']"));

        public Radio
            RdoFinancialInterestNo = new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.investigator.customAttributes.hasFinancialInterest'][value='no']")),
            RdoYesFinancialInterestYes = new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.investigator.customAttributes.hasFinancialInterest'][value='yes']")),
            RdoExternalIrbNo = new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.externalIRBInvolved'][value='no']")),
            RdoExternalIrbYes = new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.externalIRBInvolved'][value='yes']"));

        public Button BtnAddDocument = new Button(By.CssSelector("input[value='Add']"));

        public void AttachProtocol(string docLocation)
        {
            BtnAddDocument.Click();
            addAttachment.SwitchTo();
            addAttachment.BtnBrowse.SendKeys(docLocation);
            addAttachment.BtnOk.Click();
            addAttachment.SwitchBackToParent();
        }

    }

    public class SelectPerson : ChooserPopup
    {
        public CCElement
            //BtnOk = new CCElement(By.Id("btnOk")),
            FirstUser = new CCElement(By.Id("webrRSV__SelectedItem_0")),
            DivContainer = new CCElement(By.Id("_webrRSV_DIV_0"));

        public SelectPerson(string dataTypeDisplayName, bool allowMultiSelect = false) : base(dataTypeDisplayName, allowMultiSelect)
        {
        }

        public void SelectUser(string lastName)
        {
            CCElement targetElement, targetParent, rdoTarget;
            Wait.Until(h => DivContainer.Displayed);
            List<CCElement> lastNames = DivContainer.GetDescendants(".//table/tbody/tr/td[2]");
            targetElement = lastNames.FirstOrDefault(h => h.Text == lastName);
            if (targetElement == null)
            {
                throw new Exception("Could not find user with last name: " + lastName);
            }
            targetParent = targetElement.GetParent();
            rdoTarget = targetParent.GetDescendant(".//td[1]/input");
            rdoTarget.Click();
            BtnOk.Click();
        }

        public void SelectFirstUser()
        {
            FirstUser.Click();
            BtnOk.Click();
        }

        //public string Title { get { return "Select Person"; } }
    }

    public class ExternalIRB : SmartFormPage
    {
        public Button BtnSelectExternalIRB = new Button(By.CssSelector("input[value='Select...']"));

        public ChooserPopup SelectOrgPopup = new ChooserPopup("Organization");
        
    }

    public class StudyScope : SmartFormPage
    {
        public Radio
            RadioExternalSitesNo =
                new Radio(
                    By.CssSelector("input[name='_IRBSubmission.customAttributes.externalSitesPresent'][value='no']")),
            RadioExternalSitesYes =
                new Radio(
                    By.CssSelector("input[name='_IRBSubmission.customAttributes.externalSitesPresent'][value='yes']")),
            RadioDrugsInvolvedNo =
                new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.drugInvolved'][value='no']")),
            RadioDrugsInvolvedYes =
                new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.drugInvolved'][value='yes']")),
            RadioDeviceInvolvedNo =
                new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.deviceInvolved'][value='no']")),
            RadioDeviceInvolvedYes =
                new Radio(By.CssSelector("input[name='_IRBSubmission.customAttributes.deviceInvolved'][value='yes']"));

        public void SpecifyExternalSite(bool value = false)
        {
            if (value)
            {
                RadioExternalSitesYes.Click();
            }
            else
            {
                RadioExternalSitesNo.Click();
            }
        }

        public void SpecifyDrugsInvolved(bool value = false)
        {
            if (value)
            {
                RadioDrugsInvolvedYes.Click();
            }
            else
            {
                RadioDrugsInvolvedNo.Click();
            }
        }

        public void SpecifyDevicesInvolved(bool value = false)
        {
            if (value)
            {
                RadioDeviceInvolvedYes.Click();
            }
            else
            {
                RadioDeviceInvolvedNo.Click();
            }
        }

    }

    public class StudyTeamMembers : SmartFormPage
    {
        public Button
                BtnAddTeamMember = new Button(By.CssSelector("input[value='Add']"));

        public AddStudyTeamMemberPopup addTeamMember = new AddStudyTeamMemberPopup();

        public void AddStudyTeamMember(bool isInvolvedConsentProcess, bool hasFinancialInterest, string userLastName = "", params AddStudyTeamMemberPopup.Roles[] roles)
        {
            BtnAddTeamMember.Click();
            addTeamMember.SwitchTo();
            addTeamMember.SelectTeamMember(userLastName);
            addTeamMember.SpecifyConsentProcessInvolvement();
            addTeamMember.SpecifyFinancialInterest();
            addTeamMember.SelectRoles(roles);
            addTeamMember.BtnOk.Click();
            addTeamMember.SwitchBackToParent();
        }
        
    }

    public class ConsentFormsRecruitment : SmartFormPage
    {
        // add consent forms
        // add recruitment materials
    }

    public class StudyModCustom : SmartFormPage
    {
        // test SF page
    }

    public class SupportingDocuments : SmartFormPage
    {
        // Add supporting files
    }

    public class Final : SmartFormPage
    {
        
    }



    
    
}
