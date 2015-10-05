
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.TestCases
{
    [TestFixture]
    public class InitialStudySystemTests : BaseTest
    {
        [Test]
        public void CreateAndSubmitStudyAsPIProxy()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();
            var ActivitiesNav = new ActivitiesNav();

            //Log in as PI and Create New Study. 
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

            // get the id of the study
            string id = workspace.GetStudyID();
            ActivitiesNav.LnkAssignPIProxy.Click();

            // helper method for assignPI proxy?  not frequently used...
            var AssignPIProxyPopup = new AssignPIProxyPopup(id, "Assign PI Proxy");
            AssignPIProxyPopup.SwitchTo();
            AssignPIProxyPopup.SelectFirstPerson();
            AssignPIProxyPopup.BtnOk.Click();
            AssignPIProxyPopup.ConfirmCredentials(Users.Pi.UserName, Users.Pi.Password);
            AssignPIProxyPopup.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("PI Proxies updated")).Exists);
            Assert.IsTrue(new Link(By.LinkText("PI Proxies updated")).Exists, "'PI Proxies updated' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Submission");

            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);

            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review");

        }

        [Test]
        public void InitialCreationToNonCommitteeReview_Approved()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();
            
            //Log in as PI and Create New Study. 
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
            InitialStudySF.StudyTeamMembersPage.AddStudyTeamMember(false,false,"Bivens (pi2)",AddStudyTeamMemberPopup.Roles.CoInvestigator, AddStudyTeamMemberPopup.Roles.ResearchAssistant);
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
            
            //As the PI, Assign Primary Contact.
            workspace.AssignPrimaryContact("Bivens (pi2)");
            
            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the IRBC, Submit Pre-Review (yes to “Are you ready to Submit this pre-review?)
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //As the IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");

            //Log in as Designated Reviewer, Request Clarifications by Designated Reviewer.
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.RequestClarificationByDesignatedReviewer();

            //Log in as PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password,"Green Lantern here:  everything is green and good to go.");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as Designated Reviewer, Submit Designated Review (approved determination and “yes” to submit the review).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());
            
            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationForNonCommitteeReview_NotHumanResearch()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned IRB coordinator, Request Pre-Review Clarification. 
            workspace.RequestPreReviewClarification();
            Assert.IsTrue(workspace.GetStudyState() == "Clarification Requested (Pre-Review)", "State expected not in 'Clarification Requested (Pre-Review)'.  State currently: " + workspace.GetStudyState());
            
            //Log in as the PI, Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            
            //Log in as the assigned IRB coordinator, Submit Pre-Review (“yes” to submit).
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //As the IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently: " + workspace.GetStudyState());

            //Log in as the Designated Reviewer, Submit Designated Review (not human research determination/yes to submit review).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.NotHumanResearch);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());
           
            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Not Human Research", "State expected not in 'Not Human Research'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationThroughCommitteeReview_Deferred()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned IRBC, Submit Pre-Review (yes to submit pre-review).
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently: " + workspace.GetStudyState());

            //As the assigned IRB Coordinator,  Submit Committee Review (Deferred)
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Deferred,"2","1","0","0","0","",true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());
            
            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Deferred", "State expected not in 'Deferred'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationThroughCommitteeReview_Disapproved()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned IRBC, Submit Pre-Review (yes to submit pre-review).
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently: " + workspace.GetStudyState());

            //As the assigned IRB Coordinator,  Submit Committee Review (Deferred)
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Disapproved, "2", "1", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Disapproved", "State expected not in 'Disapproved'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationThroughCommitteeReview_HumanResearchNotEngaged()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned IRBC, Submit Pre-Review (yes to submit pre-review).
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently: " + workspace.GetStudyState());

            //As the assigned IRB Coordinator,  Submit Committee Review (Deferred)
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.HumanResearchNotEngaged, "2", "1", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Human Research, Not Engaged", "State expected not in 'Human Research, Not Engaged'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        
        public void InitialStudyCreationThroughCommitteeReview_ModsRequired_Approved()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            // As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            // Log in as the assigned IRBC, Submit Pre-Review (yes to submit pre-review).
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            // Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently: " + workspace.GetStudyState());

            //As the assigned IRB Coordinator,  Submit Committee Review (Modifications Required Secure to Approved)
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.ModificationsRequiredToSecureApproved, "Reason #1","2", "1", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyID());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitResponse(Users.Pi.UserName,Users.Pi.Password,"This is my respone!  :/ ");

            // Log in as the assigned IRB Coordinator and Assign to Committee Review.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignToCommitteeReview();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned IRB Coordinator, Add Review Comments.
            workspace.AddReviewComments("Reviewed!");
            // TODO VERIFY THE REVIEW COMMENTS
            //Assert.IsTrue(workspace.ProjectLogReviews.DivComponentArea.Text.Contains("Reviewed!"));
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned IRB Coordinator, update review comments by Add Review Comments.
            workspace.AddReviewComments("More revised comments!");
            // TODO VERIFY THE REVIEW COMMENTS
            
            // Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());
            
            // As the assigned IRB Coordinator¸ Submit Committee Review (Approved determination, “yes” to submit).
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Approved, "2", "1", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned IRB Coordinator and Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned IRB Coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationThroughCommitteeReview_NotHumanResearch()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned IRBC, Submit Pre-Review (yes to submit pre-review).
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //Log in as the assigned IRB Coordinator, Assign to Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently: " + workspace.GetStudyState());

            //As the assigned IRB Coordinator,  Submit Committee Review (Deferred)
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.NotHumanResearch, "2", "1", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Not Human Research", "State expected not in 'Not Human Research'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void InitialStudyCreationToNCRWithModsRequiredToApproved()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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

            
            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the IRBC, Submit Pre-Review (yes to “Are you ready to Submit this pre-review?)
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //As the IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");

            // 6.  Log in as Designated Reviewer, Submit Designated Review (approved determination and “yes” to submit the review).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "Modification Notes", true);
            
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());
            
            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyID());
            
            //Log in as PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password, "Green Lantern here:  everything is green and good to go.");
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyID());

            //Log in as the assigned IRBC, Review Required Modifications (Yes to the question “Were the modifications completed as required?”)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyID());
            
        }

        [Test]
        public void InitialStudyCreationToNCRWithModsRequiredToNotHumanResearch()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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


            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the IRBC, Submit Pre-Review (yes to “Are you ready to Submit this pre-review?)
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //As the IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");

            // 6.  Log in as Designated Reviewer, Submit Designated Review (approved determination and “yes” to submit the review).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureNotHumanResearch, "Modification Notes", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyID());

            //Log in as PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password, "Green Lantern here:  everything is green and good to go.");
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyID());

            //Log in as the assigned IRBC, Review Required Modifications (Yes to the question “Were the modifications completed as required?”)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Not Human Research", "State expected not in 'Not Human Research'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void InitialStudyCreationWithExternalIRB()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
            Store.LoginAsUser(Users.Pi);
            actionsNav.ImgCreateNewStudyLink.Click();

            InitialStudySF.NewSubmissionSmartformPage.TxtDescription.Value = "Random Automated test for IRB submission";
            InitialStudySF.NewSubmissionSmartformPage.TxtTitleStudy.Value = targetStudy;
            InitialStudySF.NewSubmissionSmartformPage.TxtShortTitle.Value = targetStudy;
            InitialStudySF.NewSubmissionSmartformPage.RdoFinancialInterestNo.Selected = true;
            InitialStudySF.NewSubmissionSmartformPage.RdoExternalIrbYes.Selected = true;
            //InitialStudySF.NewSubmissionSmartformPage.AttachProtocol(@"\\pdxstor\public\Aaron.Bentley\automation\testDoc.docx");
            InitialStudySF.NewSubmissionSmartformPage.BtnContinue.Click();
            
            // External IRB Page

            InitialStudySF.ExternalIrbPage.BtnSelectExternalIRB.Click();
            InitialStudySF.ExternalIrbPage.SelectOrgPopup.SwitchTo();
            InitialStudySF.ExternalIrbPage.SelectOrgPopup.SelectValue("Dummy External Institute");
            InitialStudySF.ExternalIrbPage.SelectOrgPopup.BtnOk.Click();
            InitialStudySF.ExternalIrbPage.SelectOrgPopup.SwitchBackToParent();
            InitialStudySF.ExternalIrbPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Study Created")).Exists);
            
            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Confirm External IRB.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.ConfirmExternalIRB();
            Assert.IsTrue(workspace.GetStudyState() == "External IRB", "State expected not in 'External IRB'.  State currently: " + workspace.GetStudyState());

            //Log in as the PI and Update External IRB Status (yes to close study).
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.UpdateExternalIRBStatus();
            Assert.IsTrue(workspace.GetStudyState() == "Closed", "State expected not in 'Closed'.  State currently: " + workspace.GetStudyState());
        }

        [Test]
        public void InitialStudyCreationWithNonCommitteeReview_HumanResearchNotEngaged()
        {
            string targetStudy = "AutoTestStudy-" + DataGen.String(5);
            var InitialStudySF = new InitialStudySmartForm();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();
            var workspace = new IRBWorkspace();

            //Log in as PI and Create New Study. 
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
            
            //As the PI, Submit the study.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as an IRB coordinator and Assign Coordinator.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the IRBC, Submit Pre-Review (yes to “Are you ready to Submit this pre-review?)
            workspace.SubmitPreReviewForStudy(SubmitPreReviewPopup.TypeOfResearch.BiomedicalClinical);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently: " + workspace.GetStudyState());

            //As the IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");

            //Log in as Designated Reviewer, Submit Designated Review (approved determination and “yes” to submit the review).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.HumanResearchNotEngaged);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Prepare Letter (Approved should be the only choice).
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Human Research, Not Engaged", "State expected not in 'Human Research, Not Engaged'.  State currently:  " + workspace.GetStudyID());
            
        }



    }
}
