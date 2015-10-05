using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.Helpers;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.TestCases
{
    public class MODCRSystemTests : BaseTest
    {
        /// <summary>
        /// Sets up meeting agenda for use in other tests
        /// </summary>
        //[TestFixtureSetUp] -- not working -- 
        [Test]
        public void A1_CR_TestFixtureSetup()
        {
            var ActivitiesNavPage = new ActivitiesNav();
            var ActionsNavPage = new ActionsNav();
            var NewMeetingPage = new CreateNewMeeting();

            Store.LoginAsUser(Users.Admin);
            ActivitiesNavPage.LnkMeetings.Click();
            ActionsNavPage.ImgCreateNewMeeting.Click();
            string location = "Conference Room " + DataGen.String(4);
            NewMeetingPage.CreateMeeting("IRB Committee", location);
        }

        [Test]
        public void MODCROtherPartsOfTheStudyCreationThroughNonCommitteeReviewToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();

            // Make a change to the view of the study....
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Make a change to the brief description
            StudySF.NewSubmissionSmartformPage.TxtDescription.Value =
                "TODO:  This study needs a more descriptive brief description!";
            // Save, exit
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();
            
            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            // Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // As the assigned IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void MODCROtherPartsOfStudyCreationThroughNonCommitteeReviewWithModsRequiredToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();

            // Make a change to the view of the study....
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Make a change to the brief description
            StudySF.NewSubmissionSmartformPage.TxtDescription.Value =
                "TODO:  This study needs a more descriptive brief description!";
            // Save, exit
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            // Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // As the assigned IRBC, Assign Designated Reviewer.
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "These modifications are required:  #1 #2 #3", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password, "This is my response!  :P");
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned IRB Coordinator, Review Required Modifications (Yes to the question “Were the modifications completed as required?” “yes” response)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());

        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughCommitteeReviewToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "Czerch (comm2)", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review");

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Approved, "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughCommitteeReviewToDeferred()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            //StudySF.StudyTeamMembersPage.AddStudyTeamMember("Czerch (comm2)", false, false, AddStudyTeamMemberPopup.Roles.CoInvestigator);
            // test to see if "" selects first value...
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review");

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Deferred, "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Deferred", "State expected not in 'Deferred'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughCommitteeReviewToDisapproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            //StudySF.StudyTeamMembersPage.AddStudyTeamMember("Czerch (comm2)", false, false, AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review");

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Disapproved, "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Disapproved", "State expected not in 'Disapproved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughCommitteeReviewWithModsRequiredToApproved()
        {

            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            //StudySF.StudyTeamMembersPage.AddStudyTeamMember("Czerch (comm2)", false, false, AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.ModificationsRequiredToSecureApproved, "modification comments", "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned IRB Coordinator, Review Required Modifications (Yes to the question “Were the modifications completed as required?” “yes” response)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughNonCommitteeReviewToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit the Mod.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // As the assigned IRBC, Assign Designated Reviewer.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review approved.
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void MODCRStudyTeamOnlyCreationThroughNonCommitteeReviewWithModificationsRequiredToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , 
            // select “Modification and Continuing” and “Other parts of the study”.  Make changes to one or more views of the study.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ModAndCR);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit the Mod.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // As the assigned IRBC, Assign Designated Reviewer.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review approved.
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "These are modifications required:  :/", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password, "This is my response!  :P");
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned IRB Coordinator, Review Required Modifications (Yes to the question “Were the modifications completed as required?” “yes” response)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());


        }

    }
}
