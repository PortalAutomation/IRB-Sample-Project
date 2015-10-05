using System.Diagnostics;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.Pages.Components;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.Helpers;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.TestCases 
{
    [TestFixture]
    public class MiscTests : BaseTest
    {
        [Test]
        public void A1_CreateAndSubmitIRBSubmission()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            var Inbox = new Inbox();
            var actionsNav = new ActivitiesNav();
            var Workspace = new IRBWorkspace();
            var InitialStudySF = new InitialStudySmartForm();

            // Log in as PI and create a study
            Store.LoginAsUser(Users.Pi);
            actionsNav.ImgCreateNewStudyLink.Click();
            InitialStudySF.NewSubmissionSmartformPage.TxtDescription.Value = "Random Automated test for IRB submission";
            InitialStudySF.NewSubmissionSmartformPage.TxtTitleStudy.Value = targetStudy;
            InitialStudySF.NewSubmissionSmartformPage.TxtShortTitle.Value = targetStudy;
            InitialStudySF.NewSubmissionSmartformPage.RdoExternalIrbNo.Selected = true;
            InitialStudySF.NewSubmissionSmartformPage.RdoFinancialInterestNo.Selected = true;
            InitialStudySF.NewSubmissionSmartformPage.AttachProtocol(@"\\pdxstor\public\Aaron.Bentley\automation\testDoc.docx");
            InitialStudySF.NewSubmissionSmartformPage.BtnContinue.Click();
            // Funding Sources
            InitialStudySF.FundingSourcesPage.BtnContinue.Click();
            // Study Team Members
            InitialStudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "Bivens (pi2)", AddStudyTeamMemberPopup.Roles.CoInvestigator, AddStudyTeamMemberPopup.Roles.ResearchAssistant);
            InitialStudySF.StudyTeamMembersPage.BtnContinue.Click();
            // Study Scope
            InitialStudySF.StudyScopePage.SpecifyExternalSite(false);
            InitialStudySF.StudyScopePage.SpecifyDrugsInvolved(false);
            InitialStudySF.StudyScopePage.SpecifyDevicesInvolved(false);
            InitialStudySF.StudyScopePage.BtnContinue.Click();
            // Consent Forms and Recuitment Materials -- skip
            InitialStudySF.ConsentFormsRecruitmentPage.BtnContinue.Click();
            // title / description -- skip
            InitialStudySF.StudyModCustomPage.BtnContinue.Click();
            // Add supporting documents -- skip
            InitialStudySF.SupportingDocumentsPage.BtnContinue.Click();
            // Final Page
            InitialStudySF.FinalPage.BtnFinish.Click();

            // Assert the study appears in window title
            Wait.Until(h => Web.PortalDriver.Title == targetStudy);
            
            // Verify a link called Study Created for this study exists
            Assert.IsTrue(new CCElement(By.LinkText("Study Created")).Exists, "Study does not exist:  " + targetStudy);
            
        }

        /// <summary>
        /// Assign a coordinator, submit pre-review.  Assumes there is a submission with "AutoTest" in pre-review state.
        /// Using AutoTest-wmdZH (ID STUDY00000015) as clone template to create more pre-review submissions.
        /// </summary>
        [Test]
        public void A2_SubmitPreReview()
        {
            // take a study in the previous pre-review state, clone it.

            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000011", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var AssignCoordinator = new AssignCoordinator("STUDY-" + targetStudy, "Assign Coordinator");
            
            var IRBSubmissionsPage = new IRBSubmissions();
            var Workspace = new IRBWorkspace();

            // Login as irbd
            Store.LoginAsUser(Users.Irbd);
            IRBSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
          
            // Assign a coordinator
            Workspace.AssignCoordinator("Orlando Max (irbc)");
            
            // verify link IRB Coordinator Assigned
            Assert.IsTrue(new CCElement(By.LinkText("IRB Coordinator Assigned")).Exists, "Cannot find 'IRB Coordinator Assigned' activity in history");
            
            // Submit Pre-Review
            var PreReview = new SubmitPreReviewPopup(Workspace.GetStudyID(), "Submit Pre-Review");
            
            ActionsNav.LnkSubmitPreReview.Click();
            PreReview.SwitchTo();
            PreReview.RdoRiskLevelGreater.Click();
            PreReview.ChkBoxBioMedicalClinical.Click();
            PreReview.RadioBtnSubmitPreReviewYes.Click();
            PreReview.BtnOk.Click();
            PreReview.SwitchBackToParent();
            // verify link Pre-Review Submitted exists
            Wait.Until((d) => new CCElement(By.LinkText("Pre-Review Submitted")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Pre-Review Submitted")).Exists, "Cannot find 'Pre-Review Submitted' activity in history");
        }

        /// <summary>
        /// Places a Pre-Review Complete into Non-Committee Review
        ///  </summary>
        [Test]
        public void PutIntoReview()
        {
            // Take an existing study in the pre-review completed state, and clone it, put into non-committe review -- use one of the clones in this test
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY-AutoTest-SMVPz", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var Inbox = new Inbox();
            //var DesignatedReviewerPopup = new AssignDesignatedReviewer();
            var IRBSubmissionPage = new IRBSubmissions();
            var StudyWorkspace = new IRBWorkspace();

            // Login as irbd
            Store.LoginAsUser(Users.Irbd);
            IRBSubmissionPage.OpenSubmissionByAllSubmissions(targetStudy);
            
            // Assign Designated Reviewer
            var DesignatedReviewerPopup = new AssignDesignatedReviewer(StudyWorkspace.GetStudyID(),"Assign Designated Reviewer");
            ActionsNav.LnkAssignDesignatedReviewer.Click();
            DesignatedReviewerPopup.SwitchTo();
            DesignatedReviewerPopup.CmbDesignatedReviewer.SelectByInnerText("Harry Smith (comm4)");
            DesignatedReviewerPopup.OkBtn.Click();
            DesignatedReviewerPopup.SwitchBackToParent();
            
            Wait.Until((d) => new CCElement(By.LinkText("Assigned to Designated Reviewer")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Assigned to Designated Reviewer")).Exists);

        }

        /// <summary>
        /// Submits a designated review for a non-committee Review
        /// </summary>
        [Test]
        public void FinalizeDocuments()
        {
            // Using STUDY-AutoTest-uqqHJ as template
           string targetStudy = "AutoTest-" + DataGen.String(5);
           CloneEntity("STUDY-AutoTest-nGPTf", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var Workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            
            var submitDesignatedReviewPopup = new SubmitDesignatedReview(Workspace.GetStudyID(), "Submit Designated Review");
            ActionsNav.LnkSubmitDesignatedReview.Click();
            submitDesignatedReviewPopup.SwitchTo();
            submitDesignatedReviewPopup.ChkConflictingInterest.Click();
            submitDesignatedReviewPopup.RdoBtnFirstDetermination.Click();
            submitDesignatedReviewPopup.RdoFirstReviewLevel.Click();
            submitDesignatedReviewPopup.ChkFirstExemptCategory.Click();
            submitDesignatedReviewPopup.TxtLastDayApproval.Text = "2/6/2030";
            submitDesignatedReviewPopup.RdoReadyToSubmitThisReviewYes.Click();
            submitDesignatedReviewPopup.BtnOk.Click();
            submitDesignatedReviewPopup.SwitchBackToParent();
            
            Wait.Until((d) => new CCElement(By.LinkText("Designated Review Submitted")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Designated Review Submitted")).Exists);
            Assert.IsTrue(Workspace.GetStudyState() == "Post-Review");

        }

        /// <summary>
        /// Move from Post-Review to Review-Complete for study
        /// Finalizes a document, prepares a letter, send letter -- moving from Post-Revew to Review Complete.
        /// </summary>
        
        [Test]
        public void PrepareLetter()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY-AutoTest-hWfUU", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var Inbox = new Inbox();
            var finalizeDocumentsPage = new FinalizeDocuments();
            //var prepareLetterPage = new PrepareLetterPopup();
            //var sendLetterPage = new SendLetterPopup();
            var IRBSubmissionPage = new IRBSubmissions();
            var studyworkspace = new IRBWorkspace();

            Store.LoginAsUser(Users.Irbd);
            
            ActionsNav.LnkSubmissions.Click();
            IRBSubmissionPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", targetStudy);
            IRBSubmissionPage.OpenSubmission(targetStudy);
           
            // finalize the documents
            ActionsNav.LnkFinalizeDocuments.Click();
            PopUpWindow.SwitchTo("Execute \"Finalize Documents\"",true);
            finalizeDocumentsPage.ChkApprove.Click();
            finalizeDocumentsPage.BtnOk.Click();
            // Assert that the "Finalized Documents" appear in History tab
            PopUpWindow.SwitchTo("AutoTest",true);
            Wait.Until((d) => new CCElement(By.LinkText("Finalized Documents")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Finalized Documents")).Exists);
            Assert.IsTrue(ActionsNav.ContainerIRBState.Text == "Post-Review");

            // Prepare Letter
            var prepareLetterPage = new PrepareLetterPopup(studyworkspace.GetStudyID(),"Prepare Letter");
            ActionsNav.LnkPrepareLetter.Click();
            PopUpWindow.SwitchTo("Execute \"Prepare Letter\"",true);
            prepareLetterPage.CmbDraftLetterTemplate.SelectByInnerText("Approval");
            prepareLetterPage.BtnGenerate.Click();
            // wait for draft letter link to appear
            Wait.Until(d => new CCElement(By.PartialLinkText("Correspondence")).Exists);
            prepareLetterPage.BtnOk.Click();
            PopUpWindow.SwitchTo("AutoTest",true);
            Wait.Until((d) => new CCElement(By.LinkText("Prepared Letter")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Prepared Letter")).Exists);
            Assert.IsTrue(ActionsNav.ContainerIRBState.Text == "Post-Review");

            // Send Letter
            var sendLetterPage = new SendLetterPopup(studyworkspace.GetStudyID(), "Send Letter");
            ActionsNav.LnkSendLetter.Click();
            PopUpWindow.SwitchTo("Execute \"Send Letter\"", true);
            sendLetterPage.BtnOk.Click();
            PopUpWindow.SwitchTo("AutoTest", true);
            Wait.Until((d) => new CCElement(By.LinkText("Letter Sent")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Letter Sent")).Exists);
            Assert.IsTrue(ActionsNav.ContainerIRBState.Text == "Approved");

        }


        

        /// <summary>
        /// Create a modification for a study in the review complete state
        /// </summary>
        [Test]
        public void CreateModification()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000024", targetStudy, true);

            var ActionsNav = new ActivitiesNav();
            var IRBSubmissionPage = new IRBSubmissions();
            var ModificationsPage = new InitialModCrSmartForm();
            var ModificationInfoPage = new ModificationInformation();
            var StudyTeamMembersPage = new StudyTeamMembers();
            var Workspace = new IRBWorkspace();
            var InitialStudySF = new InitialStudySmartForm();
            
            Store.LoginAsUser(Users.Pi);

            IRBSubmissionPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name",targetStudy);
            IRBSubmissionPage.OpenSubmission(targetStudy);

            // Create modification 
            ActionsNav.ImgCreateModCr.Click();
            ModificationsPage.RdoModification.Click();
            ModificationsPage.ChkAddStudyMemeber.Click();
            ModificationsPage.BtnContinue.Click();
            ModificationInfoPage.TxtSummary.Value = "This is a test summary for modification.";
            ModificationsPage.BtnContinue.Click();
            InitialStudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "Elmira (comm1)", AddStudyTeamMemberPopup.Roles.CoInvestigator, AddStudyTeamMemberPopup.Roles.ResearchAssistant);
            InitialStudySF.StudyTeamMembersPage.BtnContinue.Click();
            
            // save and exit smartform
            InitialStudySF.LnkSave.Click();
            InitialStudySF.LnkExit.Click();

            Workspace.SubmitMod(Users.Pi.UserName, Users.Pi.Password);
            PopUpWindow.SwitchTo(targetStudy,true);
           
            // why does Exists not retry?
            Wait.Until(h => new CCElement(By.LinkText("Submitted")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Submitted")).Exists, "Attempted submitted mod does not exist for:  " + targetStudy);
        }

        [Test]
        public void DiscardModification()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000024", targetStudy, true);

            var ActionsNav = new ActivitiesNav();
            var IRBSubmissionPage = new IRBSubmissions();
            var ModificationsPage = new InitialModCrSmartForm();
            var ModificationInfoPage = new ModificationInformation();
            var StudyTeamMembersPage = new StudyTeamMembers();
            var Workspace = new IRBWorkspace();
            var InitialStudySF = new InitialStudySmartForm();

            Store.LoginAsUser(Users.Pi);

            IRBSubmissionPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", targetStudy);
            IRBSubmissionPage.OpenSubmission(targetStudy);

            // Create modification 
            ActionsNav.ImgCreateModCr.Click();
            ModificationsPage.RdoModification.Click();
            ModificationsPage.ChkAddStudyMemeber.Click();
            ModificationsPage.BtnContinue.Click();
            ModificationInfoPage.TxtSummary.Value = "This is a test summary for modification.";
            ModificationsPage.BtnContinue.Click();

            InitialStudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "Czerch (comm2)", AddStudyTeamMemberPopup.Roles.CoInvestigator, AddStudyTeamMemberPopup.Roles.ResearchAssistant);
            InitialStudySF.StudyTeamMembersPage.BtnContinue.Click();

            // save and exit smartform
            InitialStudySF.LnkSave.Click();
            InitialStudySF.LnkExit.Click();

            // Discard the modification
            ActionsNav.LnkDiscard.Click();
            PopUpWindow.SwitchTo("Execute \"Discard\" on", true);
            new Button(By.Id("okBtn")).Click();

            PopUpWindow.SwitchTo("Modification",true);
            Wait.Until(h => new CCElement(By.LinkText("Discarded")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Discarded")).Exists, "Discarded link does not exist:  " + targetStudy);
        }

        [Test]
        public void ManageAncillaryReviews()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var ManageAncillaryReviewsPage = new ManageAncillaryReviews("STUDY-" + targetStudy,"Manage Ancillary Reviews");
            var SelectPersonPage = new SelectPerson("Person");
            var AddAncillaryReviewPage = new AddAncillaryReview();

            Store.LoginAsUser(Users.Irbd);
            
            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkManageAncillaryReviews.Click();
            ManageAncillaryReviewsPage.SwitchTo();
            ManageAncillaryReviewsPage.BtnAdd.Click();
            AddAncillaryReviewPage.SwitchTo();
            AddAncillaryReviewPage.SelectPerson("Max (irbc)");
            //AddAncillaryReviewPage.BtnSelectPerson.Click();
            //SelectPersonPage.SwitchTo();
            //SelectPersonPage.SelectFirstUser();
            //PopUpWindow.SwitchTo("Add Ancillary Review",true);
            AddAncillaryReviewPage.SelReviewType.SelectOption("Faculty");
            AddAncillaryReviewPage.RdoResponseRequiredNo.Click();
            AddAncillaryReviewPage.BtnOk.Click();
            PopUpWindow.SwitchTo("Manage Ancillary Review", true);
            ManageAncillaryReviewsPage.BtnOk.Click();
            PopUpWindow.SwitchTo(targetStudy);

            Wait.Until(h => new CCElement(By.LinkText("Managed Ancillary Reviews")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Managed Ancillary Reviews")).Exists, "Managed Ancillary Reviews not found for:  " + targetStudy);
        }

        [Test]
        public void AssignPrimaryContact()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var workspace = new IRBWorkspace();
            var SelectPersonPage = new SelectPerson("Person");
            
            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();
            var assignPrimaryContact = new AssignPrimaryContact(workspace.GetStudyID(), "Assign Primary Contact");
            ActionsNav.LnkAssignPrimaryContact.Click();
            assignPrimaryContact.SwitchTo();
            PopUpWindow.SwitchTo("Execute \"Assign Primary Contact\"",true);
            assignPrimaryContact.SelectPrimaryContact("Bivens (pi2)");
            
            PopUpWindow.SwitchTo(targetStudy);
            Wait.Until(h => new CCElement(By.LinkText("Assigned Primary Contact")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Assigned Primary Contact")).Exists, "Assigned primary contact not found for:  " + targetStudy);
        }

        [Test]
        public void AssignIRB()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var AssignIRBPage = new AssignIRB("STUDY-" + targetStudy, "Assign IRB");

            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkAssignIRB.Click();

            AssignIRBPage.SwitchTo();
            AssignIRBPage.SelIrbOffice.SelectOption("IRB 1");
            AssignIRBPage.BtnOk.Click();
            //AssignIRBPage.SwitchBackToParent();
            PopUpWindow.SwitchTo(targetStudy);

            Wait.Until(h => new CCElement(By.LinkText("Assigned IRB office")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Assigned IRB office")).Exists, "Assigned IRB office not found for:  " + targetStudy);
        }

        /// <summary>
        /// Add public comment
        /// </summary>
        [Test]
        public void AddComments()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var AddCommentsPage = new AddComments("STUDY-" + targetStudy, "Add Comment");
            
            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkAddComment.Click();
            AddCommentsPage.SwitchTo();
            AddCommentsPage.TxtComment.Value = "This is a public comment";
            AddCommentsPage.BtnOk.Click();
            AddCommentsPage.SwitchBackToParent();
            
            Wait.Until(h => new CCElement(By.LinkText("Comment Added")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Comment Added")).Exists, "Comment Added not found for:  " + targetStudy);
        }

        [Test]
        public void AddPrivateComment()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var AddPrivateCommentsPage = new AddPrivateComments("STUDY-" + targetStudy, "Add Private Comment");

            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkAddPrivateComment.Click();
            AddPrivateCommentsPage.SwitchTo();
            AddPrivateCommentsPage.TxtComment.Value = "This is a private comment";
            AddPrivateCommentsPage.BtnOk.Click();
            AddPrivateCommentsPage.SwitchBackToParent();

            Wait.Until(h => new CCElement(By.LinkText("Private Comment Added")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Private Comment Added")).Exists, "Private Comment Added not found for:  " + targetStudy);
        }

        [Test]
        public void CopySubmission()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var CopySubmissionPage = new CopySubmission("STUDY-" + targetStudy, "Copy Submission");
            //var CopySubmissionPage = new CopySubmission("STUDY-AutoTest-iyULl", "Copy Submission");

            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            string copyId = "Auto-SubmissionCopy" + DataGen.String(5);

            ActionsNav.LnkCopySubmission.Click();
            CopySubmissionPage.SwitchTo();
            CopySubmissionPage.TxtNewSubmissionName.Value = copyId;
            CopySubmissionPage.BtnOk.Click();
            CopySubmissionPage.SwitchBackToParent();

            Wait.Until(h => new CCElement(By.LinkText("Copied Submission")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Copied Submission")).Exists, "Copied Submission not found for:  " + targetStudy);
            
            // Move this validation logic to page object
            Container divContainer = new Container(By.XPath("//div[@class='Component_ProjectLog_Notes'][\"Auto-SubmissionCopy\"]"));
            int i = 0;
            const int maxRetries = 15;
            while (!(divContainer.Text.Contains(copyId)) && i < maxRetries)
            {
                System.Threading.Thread.Sleep(1000);
                // have to manually refresh
                Web.PortalDriver.Navigate().Refresh();
                i++;
            }
            Assert.IsTrue(divContainer.Text.Contains(copyId), "Cannot find the 'submission copy':  " + copyId);

        }

        [Test]
        public void ManageGuestList()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var ManageGuestListPagePopup = new ManageGuestList("STUDY-" + targetStudy, "Manage Guest List");
            var SelectPersonsPage = new ChooserPopup("Persons", true);

            Store.LoginAsUser(Users.Irbd);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkManageGuestList.Click();
            ManageGuestListPagePopup.SwitchTo();
            ManageGuestListPagePopup.BtnAddGuest.Click();

            SelectPersonsPage.SwitchTo();
            SelectPersonsPage.SelectValue("Jones (irbc2)", "Last");
            SelectPersonsPage.BtnOk.Click();
            SelectPersonsPage.SwitchBackToParent();

            ManageGuestListPagePopup.BtnOk.Click();
            ManageGuestListPagePopup.SwitchBackToParent();

            Wait.Until(h => new CCElement(By.LinkText("Guest List Updated")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Guest List Updated")).Exists, "'Guest List Updated' activity not found for:  " + targetStudy);
        }

        [Test]
        public void AddRelatedGrant()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000017", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var AddRelatedGrantPopup = new AddRelatedGrant("STUDY-" + targetStudy, "Add Related Grant");
            var ClickIRBGrantInfoPopup = new AddClickIRBGrantInfo();
            var Inbox = new Inbox();
            //var SelectPersonsPage = new ChooserPopup("Persons", true);

            Store.LoginAsUser(Users.Irbd);
            ActionsNav.LnkSubmissions.Click();
            Inbox.LnkAdvanced.Click();
            Wait.Until(h => Inbox.QueryField1.Displayed);
            Inbox.QueryField1.SelectByInnerText("Name");
            Wait.Until(h => Inbox.QueryCriteria1.Enabled);
            Inbox.QueryCriteria1.Text = targetStudy;
            Wait.Until(d => Inbox.BtnGo.Enabled);
            Inbox.BtnGo.Click();
            Wait.Until(d => Inbox.BtnGo.Enabled);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();

            ActionsNav.LnkAddRelatedGrant.Click();
            AddRelatedGrantPopup.SwitchTo();
            AddRelatedGrantPopup.BtnAdd.Click();
            ClickIRBGrantInfoPopup.SwitchTo();
            ClickIRBGrantInfoPopup.TxtGrantId.Value = "Grant-" + DataGen.String(4);
            ClickIRBGrantInfoPopup.TxtGrantTitle.Value = "Grant Title Automation Test" + DataGen.String(4);
            ClickIRBGrantInfoPopup.BtnOk.Click();
            ClickIRBGrantInfoPopup.SwitchBackToParent();
            AddRelatedGrantPopup.BtnOk.Click();
            AddRelatedGrantPopup.SwitchBackToParent();

            Wait.Until(h => new CCElement(By.LinkText("Related Grants Updated")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Related Grants Updated")).Exists, "'Related Grants Updated' activity not found for:  " + targetStudy);
        }

        public void RequestPreReviewClarification()
        {
            
        }

        public void SubmitResponse()
        {
            // clone STUDY00000014

            
        }

        public void EditPreReviewerNotePopup()
        {
            
        }

        public void TerminateStudy()
        {
            // use STUDY00000024 as template
            string targetStudy = "AutoTest-" + DataGen.String(5);
            CloneEntity("STUDY00000024", targetStudy);

            var ActionsNav = new ActivitiesNav();
            var TerminatePopup = new Terminate("STUDY-" + targetStudy, "Terminate");
            var Inbox = new Inbox();
            
            Store.LoginAsUser(Users.Irbd);
            ActionsNav.LnkSubmissions.Click();
            Inbox.LnkAdvanced.Click();
            Wait.Until(h => Inbox.QueryField1.Displayed);
            Inbox.QueryField1.SelectByInnerText("Name");
            Wait.Until(h => Inbox.QueryCriteria1.Enabled);
            Inbox.QueryCriteria1.Text = targetStudy;
            Wait.Until(d => Inbox.BtnGo.Enabled);
            Inbox.BtnGo.Click();
            Wait.Until(d => Inbox.BtnGo.Enabled);

            var studyForReview = new CCElement(By.LinkText(targetStudy));
            studyForReview.Click();
            ActionsNav.LnkTerminate.Click();
            TerminatePopup.SwitchTo();
            TerminatePopup.TxtComments.Value = "Terminating this study for automation!";
            TerminatePopup.BtnOk.Click();
            TerminatePopup.SwitchBackToParent();

            Wait.Until(h => new CCElement(By.LinkText("Terminated")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Terminated")).Exists, "'Terminated' activity not found for:  " + targetStudy);
        }

        
        public void test()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);
            
        }

        private void CloneEntity(string EntityToClone, string uniqueStudyID = "", bool isMod = false)
        {
            string uniqueStudyName = DataGen.String(5);
            Store.LoginAsUser(Users.Admin);
            var cmd = new CommandWindow();

            if (isMod)
            {
                var script = @"                
                var proj = getResultSet('_IRBSubmission').query(""ID='" + EntityToClone + @"'"").elements.item(1);
                var cloneJob = EntityCloner.createRequest(proj, proj, 'j');
                cloneJob.customizeHandling('_IRBSubmission.resourceContainer', 'cloneEntity');
                cloneJob.customizeHandling('_IRBSubmission.customAttributes.draftStudy', 'cloneEntity');
                cloneJob.startRequestNow();
                var clone = cloneJob.rootEntity;
                clone.ID = 'STUDY-" + uniqueStudyID + @"'
                clone.Name = '" + uniqueStudyID + @"'
                clone.ResourceContainer.Name = '" + uniqueStudyID + @"'
                clone.customAttributes.draftStudy.Name = 'DRAFTSTUDY" + uniqueStudyID + @"'
                 ";
                Assert.AreEqual("", cmd.Run(script));
            }

            else
            {
                var script = @"                
                var proj = getResultSet('_IRBSubmission').query(""ID='" + EntityToClone + @"'"").elements.item(1);
                var cloneJob = EntityCloner.createRequest(proj, proj, 'j');
                cloneJob.customizeHandling('_IRBSubmission.resourceContainer', 'cloneEntity');
                cloneJob.customizeHandling('_IRBSubmission.customAttributes.draftStudy', 'cloneEntity');
                cloneJob.startRequestNow();
                var clone = cloneJob.rootEntity;
                clone.ID = 'STUDY-" + uniqueStudyID + @"'
                clone.Name = '" + uniqueStudyID + @"'
                clone.ResourceContainer.Name = '" + uniqueStudyID + @"'
                ";
                Assert.AreEqual("", cmd.Run(script));
            }
         }
    }
}
