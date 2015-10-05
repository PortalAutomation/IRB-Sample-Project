using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.Helpers;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.TestCases
{
    [TestFixture]
    public class CRSystemTests : BaseTest   
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
        public void ContinuingReviewThroughCommitteeReviewToApprovedAndStudyClosed()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2","2","1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete,InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();
            
            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");
            
            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review");

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Approved,"2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter (Study Closure).
            //TODO -- change the letter
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Prepare Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());

        }

        [Test]
        public void ContinuingReviewThroughCommitteeReviewToDeferred()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

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
        public void ContinuingReviewThroughCommitteeReviewToDisapproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

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
        public void ContinuingReviewThroughCommitteeReviewToHumanResearchNotEngaged()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.HumanResearchNotEngaged, "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Human Research, Not Engaged", "State expected not in 'Human Research, Not Engaged'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void ContinuingReviewThroughCommitteeReviewToNotHumanResearch()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.NotHumanResearch, "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Not Human Research", "State expected not in 'Not Human Research'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void ContinuingReviewThroughCommitteeReviewWithModificationsRequiredToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.ModificationsRequiredToSecureApproved, "modification comments","2", "0", "0", "0", "0", "", true);
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
        public void ContinuingReviewThroughNonCommitteeReviewThroughHumanResearchNotEngaged()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (human research not engaged determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.HumanResearchNotEngaged);
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
            Assert.IsTrue(workspace.GetStudyState() == "Human Research, Not Engaged", "State expected not in 'Human Research, Not Engaged'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void ContinuingReviewThroughNonCommitteeReviewThroughModsRequiredToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (human research not engaged determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "These are comments for the modification!  :/ ",true);
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
        public void ContinuingReviewThroughNonCommitteeReviewThroughNotHumanResearch()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (human research not engaged determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.NotHumanResearch);
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
            Assert.IsTrue(workspace.GetStudyState() == "Not Human Research", "State expected not in 'Not Human Research'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void ContinuingReviewThroughNonCommitteeReviewToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            //Log in as the PI to an Approved study and select Create Modification/CR. Select “Continuing Review”  and on the Continuing Review/Study Closure Information select the top 4 Research milestones. 
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.ContinuingReview);
            CRSmartForm.BtnContinue.Click();

            CRSmartForm.SpecifyEnrollmentTotals("2", "2", "1");
            CRSmartForm.ChooseResearchMilestone(InitialModCrSmartForm.MileStones.StudyPermanentlyClosedToEnrollment, InitialModCrSmartForm.MileStones.AllSubjectCompletedStudyRelatedInterventions,
                InitialModCrSmartForm.MileStones.CollectionOfPrivateInfoComplete, InitialModCrSmartForm.MileStones.AnalysisOfPrivateInfoComplete);
            CRSmartForm.RdoFinancialInterestNo.Click();
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.BtnFinish.Click();

            //As the PI, Submit the Continuing Review.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var CRName = Web.PortalDriver.Title;
            
            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.SubmitPreReviewForCR();

            // Log in as an IRB coordinator and Assign Coordinator
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(CRName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (human research not engaged determination).
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
        [Ignore("Under Construction")]
        public void VAEnabledContinuingReviewthroughCRToApproved()
        {
            
        }

    }
}
