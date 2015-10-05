using System.Diagnostics;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.Pages.Components;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IRBAutomation.TestCases
{

    

    /// <summary>
    /// These tests are replicated from system tests located at '\\pdxstor\ClickSync\Application Teams\System Test\IRB System Test for 7.5'
    /// </summary>
    [TestFixture]
    public class RNISystemTests : BaseTest
    {

        /// <summary>
        /// Sets up meeting agenda for use in other tests
        /// </summary>
        //[TestFixtureSetUp] -- not working -- 
        [Test]
        public void A1_TestFixtureSetup()
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

        // Studies



        // RNI Tests

        [Test]
        public void InsignificantRniWithClarificationRequestedToAcknowledged()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var InboxPage = new Inbox();
            var ActionsNav = new ActivitiesNav();
            var RNISmartformPage = new RNISmartform();
            var StudyWorkspacePage = new IRBWorkspace();
            var IRBSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Irbc);

            InboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            RNISmartformPage.TxtRNIShortTitle.Value = RNITitle;
            RNISmartformPage.TxtDateAware.Value = "03/02/2014";
            RNISmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            RNISmartformPage.RdoIsIncreasedRiskNo.Click();
            RNISmartformPage.RdoNeedRevisionNo.Click();
            RNISmartformPage.RdoConsentRequiresRevisionkNo.Click();
            RNISmartformPage.BtnContinue.Click();
            RNISmartformPage.BtnFinish.Click();

            // verify in history tab, pre-submission
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);

            // Grab the ID of the study, switch to submit RNI popup (move this functionality to page)
            string id = StudyWorkspacePage.GetStudyID();
            var SubmitRNIPopup = new ActivityPopup(id, "Submit RNI");

            // Submit the RNI
            ActionsNav.LnkSubmitRNI.Click();
            SubmitRNIPopup.SwitchTo();
            SubmitRNIPopup.BtnOk.Click();
            // change this to "confirm credentials page"
            SubmitRNIPopup.ConfirmCredentials(Users.Irbc.UserName, Users.Irbc.Password);
            
            // Switch back to main page
            PopUpWindow.SwitchTo(RNITitle);
            Wait.Until(h => new Link(By.LinkText("RNI Submitted")).Exists);
            
            ActionsNav.LnkAssignCoordinator.Click();
            var AssignCoordinatorPopup = new AssignCoordinator(id, "Assign Coordinator");
            AssignCoordinatorPopup.SwitchTo();
            AssignCoordinatorPopup.FirstUser.Click();
            AssignCoordinatorPopup.BtnOk.Click();
            AssignCoordinatorPopup.SwitchBackToParent();

            // Log in as IRBD
            Store.LoginAsUser(Users.Irbd);

            IRBSubmissionsPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name",RNITitle);
            IRBSubmissionsPage.OpenSubmission(RNITitle);

            ActionsNav.LnkRequestPreReviewClarification.Click();
            var RequestPreReviewPopup = new RequestPreReviewClarificationPopup(id, "Request Pre-Review Clarification");
            RequestPreReviewPopup.SwitchTo();
            RequestPreReviewPopup.TxtInfo.Value = "Addition information required regarding RNI";
            RequestPreReviewPopup.BtnOk.Click();
            RequestPreReviewPopup.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("Clarification Requested")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Clarification Requested")).Exists, "'Clarification Requested' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Clarification Requested (Pre-Review)");

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            //InboxPage.OpenStudy(RNITitle);
            IRBSubmissionsPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", RNITitle);
            IRBSubmissionsPage.OpenSubmission(RNITitle);

            ActionsNav.LnkSubmitResponse.Click();
            var SubmitResponsePage = new SubmitResponsePopup(id, "Submit Response");
            SubmitResponsePage.SwitchTo();
            SubmitResponsePage.TxtInfo.Value = "I cleared this up with batman.  We are good.";
            SubmitResponsePage.BtnOk.Click();
            SubmitResponsePage.ConfirmCredentials(Users.Irbc.UserName, Users.Irbc.Password);
            
            PopUpWindow.SwitchTo(RNITitle, true);
            Wait.Until(h => new Link(By.LinkText("Response Submitted")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Response Submitted")).Exists, "'Response Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Pre-Review");

            // Log in as IRBD
            Store.LoginAsUser(Users.Irbd);
            IRBSubmissionsPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", RNITitle);
            IRBSubmissionsPage.OpenSubmission(RNITitle);

            ActionsNav.LnkSubmitRNIPreReview.Click();
            var SubmitRNIPreReviewPopup = new SubmitRNIPreReview(id, "Submit RNI Pre-Review");
            SubmitRNIPreReviewPopup.SwitchTo();
            SubmitRNIPreReviewPopup.SelectDetermination(SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            SubmitRNIPreReviewPopup.rdoSubmitPreviewYes.Click();
            SubmitRNIPreReviewPopup.BtnOk.Click();
            SubmitRNIPreReviewPopup.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("RNI Pre-Review Submitted")).Exists);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Acknowledged");
            
        }


        /// <summary>
        /// This test seems to be incomplete / duplicate of InsignificantRniWithClarificationRequestedToAcknowledged -- Verify with the IRB TEAM
        /// </summary>
        [Test]
        [Ignore]
        public void InsignificantRNISubmissionClarificationRequestedToDesignatedReviewToAcknowledged()
        {
            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as PI
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBD
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBD
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI-Preview as IRBD
            studyWorkspacePage.SubmitRNIPreReview("",true,SubmitRNIPreReview.Determinations.SeriousNonCompliance);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            //PopUpWindow.SwitchTo(RNITitle);
            // Assign to Meeting as IRBC
            studyWorkspacePage.AssignMeetingByFirstMeeting();
            Assert.IsTrue(new Link(By.PartialLinkText("Assigned to Meeting")).Exists, "'Assigned to Meeting' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Request Clarification By Committee Member TODO FIX THE LINK
            studyWorkspacePage.RequestClarificationByCommitteeMember();
            Assert.IsTrue(new Link(By.LinkText("Clarification Requested by Committee Member")).Exists, "'Clarification Requested by Committee Member' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Clarification Requested (Committee Review)");

            // Log in as submitt, Submit Response
            Store.LoginAsUser(Users.Pi);
            inboxPage.OpenStudy(RNITitle);
            studyWorkspacePage.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Response Submitted")).Exists, "'Response Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Log in as irbd, Submit RNI Committee REview (no)
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.SubmitRniCommitteeReviewPopupRequiredOnlyFields("1");
            Assert.IsTrue(new Link(By.LinkText("Committee RNI Review Submitted")).Exists, "'Committee RNI Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review");

            // log in as assigned coordinator, prepare letter
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.PrepareLetter("Review of New Information");
            Assert.IsTrue(new CCElement(By.LinkText("Prepared Letter")).Exists);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review");

            // Send Letter
            studyWorkspacePage.SendLetter();
            Assert.IsTrue(new CCElement(By.LinkText("Letter Sent")).Exists);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Complete");
        }

        [Test]
        public void InsignificantRNIThroughAssignToMeetingThenWithdrawn()
        {

            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var InboxPage = new Inbox();
            var ActivitiesNavPage = new ActivitiesNav();
            var RNISmartformPage = new RNISmartform();
            var StudyWorkspacePage = new IRBWorkspace();
            var IRBSubmissionsPage = new IRBSubmissions();
            
            Store.LoginAsUser(Users.Pi);
            InboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            RNISmartformPage.TxtRNIShortTitle.Value = RNITitle;
            RNISmartformPage.TxtDateAware.Value = "03/02/2014";
            RNISmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            RNISmartformPage.RdoIsIncreasedRiskNo.Click();
            RNISmartformPage.RdoNeedRevisionNo.Click();
            RNISmartformPage.RdoConsentRequiresRevisionkNo.Click();
            RNISmartformPage.BtnContinue.Click();
            RNISmartformPage.BtnFinish.Click();

            // verify in history tab, pre-submission
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + StudyWorkspacePage.GetStudyState());

            // Grab the ID of the study, switch to submit RNI popup (move this functionality to page)
            string id = StudyWorkspacePage.GetStudyID();
            var SubmitRNIPopup = new ActivityPopup(id, "Submit RNI");

            // Submit the RNI
            ActivitiesNavPage.LnkSubmitRNI.Click();
            SubmitRNIPopup.SwitchTo();
            SubmitRNIPopup.BtnOk.Click();
            // change this to "confirm credentials page"
            SubmitRNIPopup.ConfirmCredentials(Users.Pi.UserName, Users.Pi.Password);
            SubmitRNIPopup.SwitchBackToParent();
            Wait.Until(h => new Link(By.LinkText("RNI Submitted")).Exists);

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            IRBSubmissionsPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", RNITitle);
            IRBSubmissionsPage.OpenSubmission(RNITitle);
            
            // Assign coordinator as IRBC
            ActivitiesNavPage.LnkAssignCoordinator.Click();
            var AssignCoordinatorPopup = new AssignCoordinator(id, "Assign Coordinator");
            AssignCoordinatorPopup.SwitchTo();
            AssignCoordinatorPopup.FirstUser.Click();
            AssignCoordinatorPopup.BtnOk.Click();
            AssignCoordinatorPopup.SwitchBackToParent();
            
            // Submit RNI-Preview as IRBC
            ActivitiesNavPage.LnkSubmitRNIPreReview.Click();
            var SubmitRNIPreReviewPopup = new SubmitRNIPreReview(id, "Submit RNI Pre-Review");
            SubmitRNIPreReviewPopup.SwitchTo();
            SubmitRNIPreReviewPopup.SelectDetermination(SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            SubmitRNIPreReviewPopup.SelectDetermination(SubmitRNIPreReview.Determinations.AdditionalReviewRequired);
            SubmitRNIPreReviewPopup.rdoSubmitPreviewYes.Click();
            SubmitRNIPreReviewPopup.BtnOk.Click();
            SubmitRNIPreReviewPopup.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("RNI Pre-Review Submitted")).Exists);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            PopUpWindow.SwitchTo(RNITitle);
            // Assign to Meeting as IRBC
            ActivitiesNavPage.LnkAssignToMeeting.Click();

            var assignToMeetingPage = new AssignToMeetingPopup(id, "Assign to Meeting");
            assignToMeetingPage.SwitchTo();
            assignToMeetingPage.RdoFirstMeeting.Click();
            assignToMeetingPage.BtnOk.Click();
            assignToMeetingPage.SwitchBackToParent();

            Wait.Until(h => new Link(By.PartialLinkText("Assigned to Meeting:")).Exists);
            Assert.IsTrue(new Link(By.PartialLinkText("Assigned to Meeting:")).Exists, "'Assigned to Meeting' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Committee Review");
            
            // Withdraw as PI
            Store.LoginAsUser(Users.Pi);
            IRBSubmissionsPage.AllSubmissionsTab.NavigateTo();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            IRBSubmissionsPage.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", RNITitle);
            IRBSubmissionsPage.OpenSubmission(RNITitle);

            ActivitiesNavPage.LnkWithdraw.Click();
            var WithDrawPage = new WithdrawPopup(id, "Withdraw");
            WithDrawPage.SwitchTo();
            WithDrawPage.BtnOk.Click();
            WithDrawPage.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("Withdrawn")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Withdrawn")).Exists, "'Withdrawn' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Pre-Submission");
        }

        [Test]
        public void InsignificantRNISubmissionToAcknowledged()
        {
            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();

            // verify in history tab, pre-submission
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBD
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review");

            // Submit RNI-Preview as IRBD
            studyWorkspacePage.SubmitRNIPreReview("",true,SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged");
        }

        /// <summary>
        /// Submits RNI Committee Review twice (from "none of the above" to "action required" 
        /// </summary>
        [Test]
        public void InsignificantRNISubmissionThroughCommitteeReviewToAcknowledgedMadeSignificantToPostReview()
        {
            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            
            // Assign coordinator as IRBC
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI-Preview as IRBC
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.AdditionalReviewRequired,SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            // Assign to Meeting as IRBC
            studyWorkspacePage.AssignMeetingByFirstMeeting();
            Assert.IsTrue(new Link(By.PartialLinkText("Assigned to Meeting")).Exists, "'Assigned to Meeting' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Log in as irbd, Submit RNI Committee Review twice
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            // check none of the above, uncheck additional review required
            studyWorkspacePage.SubmitRNICommitteeReview("1","0","0","0","0");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged");
            studyWorkspacePage.SubmitRNICommitteeReview("1", "2", "0", "0", "0", SubmitRNICommitteeReviewPopup.Determinations.NoneOfTheAbove,SubmitRNICommitteeReviewPopup.Determinations.UnanticipatedProblem);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review", "'Post-Review' activity not found for:  " + RNITitle);
            
        }

        /// <summary>
        /// Possible bug?  Missing "additional review required" option, or by design
        /// </summary>
        [Test]
        [Ignore("Have questions regarding this test.")]
        public void  InsignificantRNIsubmissionthroughCommitteeReviewToPostReviewToAcknowledged()
        {
            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI-Preview as IRBC
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.NoneOfTheAbove, SubmitRNIPreReview.Determinations.AdditionalReviewRequired);
            //studyWorkspacePage.SubmitRNIPreReview(SubmitRNIPreReview.Determinations.AdditionalReviewRequired);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            // Assign to Meeting as IRBC
            studyWorkspacePage.AssignMeetingByFirstMeeting();
            Assert.IsTrue(new Link(By.PartialLinkText("Assigned to Meeting")).Exists, "'Assigned to Meeting' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Log in as irbd, Submit RNI Committee Review twice
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            // check none of the above, uncheck additional review required
            studyWorkspacePage.SubmitRNICommitteeReview("1", "0", "0", "0", "0");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review", "State not in 'Post-Review'.  Currently in state:  " + studyWorkspacePage.GetStudyState());
            studyWorkspacePage.SubmitRNICommitteeReview("1", "2", "0", "0", "0", SubmitRNICommitteeReviewPopup.Determinations.AdditionalReviewRequired);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged", "State not in 'Acknowledged'.  Currently in state:  " + studyWorkspacePage.GetStudyState());

        }

       //Insignificant RNI through Designated Review to Acknowledged made Significant and transition to Committee Review
        [Test]
        public void InsignificantRNIThroughDesignatedReviewToAcknowledgedMadeSignificantAndTransitionToCommitteeReview()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBD
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBD
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI-Preview as IRBD
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.SuspensionOrTermination);
            //studyWorkspacePage.SubmitRNIPreReview(SubmitRNIPreReview.Determinations.AdditionalReviewRequired);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            // Login as IRBC, assign designated reviewer
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.AssignDesignatedReviewer("Harry Smith (comm4)");

            // Log in as the Designated Reviewer and Submit RNI Designated Review 
            //(for insignificant RNI -one of last four determinations and YES to “Are you ready to submit this Review?).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.SubmitRNIDesignatedReview(SubmitDesignatedRNIReviewPopup.Determinations.NoneOfTheAbove, SubmitDesignatedRNIReviewPopup.Determinations.SuspensionOrTermination);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged", "State not in 'Acknowledged'.  Currently in state:  " + studyWorkspacePage.GetStudyState());
            
            // Log in as the IRB Director and Submit RNI Designated Review (changing the determination to one of the top 4).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.SubmitRNIDesignatedReview(SubmitDesignatedRNIReviewPopup.Determinations.UnanticipatedProblem, SubmitDesignatedRNIReviewPopup.Determinations.NoneOfTheAbove);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review", "State not in 'Committee Review'.  Currently in state:  " + studyWorkspacePage.GetStudyState());
            

        }

        // Insignificant RNI to Acknowledged and then resubmitted RNI pre-review for Significant RNI
        [Test]
        public void  InsignificantRNIToAcknowledgedAndThenResubmittedRNIPreReviewForSignificantRNI()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // As the assigned Coordinator, Submit RNI Pre-Review (select only 5 through 8 determinations to create an insignificant RNI and NO to “Are you ready to submit this pre-review?”)
            studyWorkspacePage.SubmitRNIPreReview("", false, SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged", "State not in 'Acknowledged'.  Currently in state:  " + studyWorkspacePage.GetStudyState());

            // As the assigned coordinator, Submit RNI Pre -Review (change the determination to be one of the top 4)
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.NoneOfTheAbove, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");
        }

        [Test]
        public void InsignificantRNIWithNoToSubmitRNIPreReviewToAcknowledged()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var InboxPage = new Inbox();
            var RNISmartformPage = new RNISmartform();
            var StudyWorkspacePage = new IRBWorkspace();
            var IRBSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);

            InboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            RNISmartformPage.TxtRNIShortTitle.Value = RNITitle;
            RNISmartformPage.TxtDateAware.Value = "03/02/2014";
            RNISmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            RNISmartformPage.RdoIsIncreasedRiskNo.Click();
            RNISmartformPage.RdoNeedRevisionNo.Click();
            RNISmartformPage.RdoConsentRequiresRevisionkNo.Click();
            RNISmartformPage.BtnContinue.Click();
            RNISmartformPage.BtnFinish.Click();

            // verify in history tab, pre-submission
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);

            // Grab the ID of the study, switch to submit RNI popup (move this functionality to page)
            StudyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            
            // Log in as IRBC, assign coordinator
            Store.LoginAsUser(Users.Irbc);
            IRBSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            StudyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");

            //As the assigned Coordinator, Submit RNI Pre-Review 
            //(select only 5 through 8 determinations to create an insignificant RNI and NO to “Are you ready to submit this pre-review?”)
            StudyWorkspacePage.SubmitRNIPreReview("",false,SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Acknowledged", "State not in 'Acknowledged'.  Currently in state:  " + StudyWorkspacePage.GetStudyState());
        }

        /// <summary>
        /// Use this as a template for RNI tests?
        /// </summary>
        [Test]
        public void SignificantRNIThroughCommitteeReviewThroughClarificationRequestedWithNoRequiredAction()
        {
            // This test requires a meeting agenda created within next 60 days of current date
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");

            // Submit RNI-Preview as IRBC
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.SeriousNonCompliance);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Pre-Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed");

            // Assign to Meeting as IRBC
            studyWorkspacePage.AssignMeetingByFirstMeeting();
            Assert.IsTrue(new Link(By.PartialLinkText("Assigned to Meeting")).Exists, "'Assigned to Meeting' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Request Clarification By Committee Member
            studyWorkspacePage.RequestClarificationByCommitteeMember();
            Assert.IsTrue(new Link(By.LinkText("Clarification Requested by Committee Member")).Exists, "'Clarification Requested by Committee Member' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Clarification Requested (Committee Review)");

            // Log in as submitt, Submit Response
            Store.LoginAsUser(Users.Pi);
            inboxPage.OpenStudy(RNITitle);
            studyWorkspacePage.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Response Submitted")).Exists, "'Response Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Committee Review");

            // Log in as irbd, Submit RNI Committee REview (no)
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.SubmitRniCommitteeReviewPopupRequiredOnlyFields("1");
            Assert.IsTrue(new Link(By.LinkText("Committee RNI Review Submitted")).Exists, "'Committee RNI Review Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review");

            // log in as assigned coordinator, prepare letter
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.PrepareLetter("Review of New Information");
            Assert.IsTrue(new CCElement(By.LinkText("Prepared Letter")).Exists);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review");

            // Send Letter
            studyWorkspacePage.SendLetter();
            Assert.IsTrue(new CCElement(By.LinkText("Letter Sent")).Exists);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Complete");
        }

        [Test]
        public void SignificantRNIThroughCommitteeReviewWithActionRequired()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var studyWorkspacePage = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var ActivitiesNav = new ActivitiesNav();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Submit RNI as Pi
            studyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            studyWorkspacePage.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + studyWorkspacePage.GetStudyState());

            // As the assigned Coordinator (IRBC) and Submit RNI Pre-Review (One of the top four determinations)
            studyWorkspacePage.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + studyWorkspacePage.GetStudyID());

            // As IRBC, Assign to Meeting
            studyWorkspacePage.AssignMeetingByFirstMeeting();

            //As the Director (IRBD), Submit RNI Committee Review (further action required, add Responsible Party and Action Plan).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            ActivitiesNav.LnkSubmitRNICommitteeReview.Click();
            var SubmitRNICommitteeReview = new SubmitRNICommitteeReviewPopup(studyWorkspacePage.GetStudyID(), "Submit RNI Committee Review");
            SubmitRNICommitteeReview.SwitchTo();
            SubmitRNICommitteeReview.RdoFurtherActionReqYes.Click();
            SubmitRNICommitteeReview.BtnResponsibleParty.Click();
            var SelectPerson = new SelectPerson("Person");
            SelectPerson.SwitchTo();
            SelectPerson.SelectUser("Max (irbc)");
            SelectPerson.SwitchBackToParent();
            SubmitRNICommitteeReview.TxtActionPlan.Value = "This is the action plan:  TODO";
            SubmitRNICommitteeReview.TxtFor.Value = "2";
            SubmitRNICommitteeReview.TxtAgainst.Value = "0";
            SubmitRNICommitteeReview.TxtRecused.Value = "0";
            SubmitRNICommitteeReview.TxtAbsent.Value = "0";
            SubmitRNICommitteeReview.TxtAbstained.Value = "0";
            SubmitRNICommitteeReview.RdoRdyForSubmissionYes.Click();
            SubmitRNICommitteeReview.BtnOk.Click();
            SubmitRNICommitteeReview.SwitchBackToParent();
            Wait.Until(h => new Link(By.LinkText("Committee RNI Review Submitted")).Exists); 
        
            studyWorkspacePage.PrepareLetter("Review of New Information");
            studyWorkspacePage.SendLetter();
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Action Required", "State expected not in 'Action Required'.  State currently:  " + studyWorkspacePage.GetStudyID());

            Store.LoginAsUser(Users.Irbc);
            // why does Action Required RNI not show up under all submissions?
            inboxPage.OpenStudy(RNITitle);
            studyWorkspacePage.SubmitActionResponse("This is an action response!");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Action Submitted", "State expected not in 'Action Submitted'.  State currently:  " + studyWorkspacePage.GetStudyID());

            //Log in as assigned Coordinator and Review Required Actions (Were the actions completed as required? – YES)
            studyWorkspacePage.ReviewRequiredActions(true);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + studyWorkspacePage.GetStudyID());

            studyWorkspacePage.PrepareLetter("Review of New Information");
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + studyWorkspacePage.GetStudyID());

            studyWorkspacePage.SendLetter();
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Complete", "State expected not in 'Complete'.  State currently:  " + studyWorkspacePage.GetStudyID());

            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            studyWorkspacePage.SubmitRNICommitteeReview("2","0","0","0","0",SubmitRNICommitteeReviewPopup.Determinations.UnanticipatedProblem,SubmitRNICommitteeReviewPopup.Determinations.NoneOfTheAbove);
            Assert.IsTrue(studyWorkspacePage.GetStudyState() == "Acknowledged", "State expected not in 'Acknowledged'.  State currently:  " + studyWorkspacePage.GetStudyID());
        }

        [Test]
        public void SignificantRNISubmissionClarificationRequestedToDesignatedReviewToAcknowledged()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            
            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + workspace.GetStudyState());

            // Submit RNI as Pi
            workspace.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as IRBD
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBD
            workspace.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // As the assigned Coordinator (IRBD) and Submit RNI Pre-Review (One of the top four determinations)
            workspace.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyID());

            // Log in as the assigned IRB Coordinator and Assign Designated Reviewer.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");

            // Log in as the assigned designated reviewer and Request Clarification by Designated Reviewer.
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.RequestClarificationByDesignatedReviewer();
            Assert.IsTrue(workspace.GetStudyState() == "Clarification Requested (Designated Review)", "State expected not in 'Clarification Requested (Designated Review)'.  State currently:  " + workspace.GetStudyID());
           
            // Log in as the PI and Submit Response
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyID());
            
            // Log in as the asssigned Designated Reviewer, Submit RNI Designated Review (selecting one of the last four determinations – yes to submit).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.SubmitRNIDesignatedReview(SubmitDesignatedRNIReviewPopup.Determinations.UnanticipatedProblem, SubmitDesignatedRNIReviewPopup.Determinations.NoneOfTheAbove);
            Assert.IsTrue(workspace.GetStudyState() == "Acknowledged", "State expected not in 'Acknowledged'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        [Ignore]
        public void SignificantRNISubmissionThroughCommitteeReviewWithActionRequiredToComplete()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var ActivitiesNav = new ActivitiesNav();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + workspace.GetStudyState());

            // Submit RNI as Pi
            workspace.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            workspace.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // As the assigned Coordinator (IRBC) and Submit RNI Pre-Review (One of the top four determinations)
            workspace.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyID());

            // As IRBC, Assign to Meeting
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review");

            // As the Director (IRBD), Submit RNI Committee Review (further action required, add Responsible Party and Action Plan).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            ActivitiesNav.LnkSubmitRNICommitteeReview.Click();
            var SubmitRNICommitteeReview = new SubmitRNICommitteeReviewPopup(workspace.GetStudyID(), "Submit RNI Committee Review");
            SubmitRNICommitteeReview.SwitchTo();
            SubmitRNICommitteeReview.RdoFurtherActionReqYes.Click();
            SubmitRNICommitteeReview.BtnResponsibleParty.Click();
            var SelectPerson = new SelectPerson("Person");
            SelectPerson.SwitchTo();
            SelectPerson.SelectUser("Max (irbc)");
            SelectPerson.SwitchBackToParent();
            SubmitRNICommitteeReview.TxtActionPlan.Value = "This is the action plan:  TODO";
            SubmitRNICommitteeReview.TxtFor.Value = "2";
            SubmitRNICommitteeReview.TxtAgainst.Value = "0";
            SubmitRNICommitteeReview.TxtRecused.Value = "0";
            SubmitRNICommitteeReview.TxtAbsent.Value = "0";
            SubmitRNICommitteeReview.TxtAbstained.Value = "0";
            SubmitRNICommitteeReview.RdoRdyForSubmissionYes.Click();
            SubmitRNICommitteeReview.BtnOk.Click();
            SubmitRNICommitteeReview.SwitchBackToParent();
            Wait.Until(h => new Link(By.LinkText("Committee RNI Review Submitted")).Exists);

            workspace.PrepareLetter("Review of New Information");
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Action Required", "State expected not in 'Action Required'.  State currently:  " + workspace.GetStudyID());

            Store.LoginAsUser(Users.Irbc);
            // why does Action Required RNI not show up under all submissions?
            inboxPage.OpenStudy(RNITitle);
            workspace.SubmitActionResponse("This is an action response!");
            Assert.IsTrue(workspace.GetStudyState() == "Action Submitted", "State expected not in 'Action Submitted'.  State currently:  " + workspace.GetStudyID());

            // Log in as assigned Coordinator and Review Required Actions (Were the actions completed as required? – NO)
            workspace.ReviewRequiredActions(false);
            Assert.IsTrue(workspace.GetStudyState() == "Action Required", "State expected not in 'Action Required'.  State currently:  " + workspace.GetStudyID());

            // Log in as the Responsible Party and Submit Action Response.
            workspace.SubmitActionResponse("this is a response!");
            Assert.IsTrue(workspace.GetStudyState() == "Action Submitted", "State expected not in 'Action Submitted'.  State currently:  " + workspace.GetStudyID());

            // Need new Assign to Committee Review
            workspace.AssignToCommitteeReview();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());

            //As the assigned Coordinator (IRBC) Submit RNI Committee Review (Are further actions required - NO)
            // TODO MISSING THIS ACTIVITY
            workspace.SubmitRNICommitteeReview();  //-- need to update for NO  

            // As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter("Review New Information");
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Complete  ", "State expected not in 'Complete  '.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        [Ignore]
        public void SignificantRNISubmissionThroughDesignatedReviewToCommitteeReviewWithActionRequired()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var ActivitiesNav = new ActivitiesNav();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + workspace.GetStudyState());

            // Submit RNI as Pi
            workspace.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as IRBD, Assign coordinator as IRBD
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // As the assigned Coordinator (IRBD) and Submit RNI Pre-Review (One of the top four determinations)
            workspace.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyID());

            // As the IRB Director, Assign Designated Reviewer.  TODO
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected 'Non-Committee Review '.  State currently:  " + workspace.GetStudyID());

            // Log in as the Designated Reviewer and Submit RNI Designated Review (for significant RNI -one of the top 4 determinations). ??
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.SubmitRNIDesignatedReview();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());

            // Log in as the assigned coordinator and Assign to Meeting.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyID());

            // As the Director (IRBD), Submit RNI Committee Review (further action required, add Responsible Party and Action Plan).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            ActivitiesNav.LnkSubmitRNICommitteeReview.Click();
            var SubmitRNICommitteeReview = new SubmitRNICommitteeReviewPopup(workspace.GetStudyID(), "Submit RNI Committee Review");
            SubmitRNICommitteeReview.SwitchTo();
            SubmitRNICommitteeReview.RdoFurtherActionReqYes.Click();
            SubmitRNICommitteeReview.BtnResponsibleParty.Click();
            var SelectPerson = new SelectPerson("Person");
            SelectPerson.SwitchTo();
            SelectPerson.SelectUser("Max (irbc)");
            SelectPerson.SwitchBackToParent();
            SubmitRNICommitteeReview.TxtActionPlan.Value = "This is the action plan:  TODO";
            SubmitRNICommitteeReview.TxtFor.Value = "2";
            SubmitRNICommitteeReview.TxtAgainst.Value = "0";
            SubmitRNICommitteeReview.TxtRecused.Value = "0";
            SubmitRNICommitteeReview.TxtAbsent.Value = "0";
            SubmitRNICommitteeReview.TxtAbstained.Value = "0";
            SubmitRNICommitteeReview.RdoRdyForSubmissionYes.Click();
            SubmitRNICommitteeReview.BtnOk.Click();
            SubmitRNICommitteeReview.SwitchBackToParent();
            Wait.Until(h => new Link(By.LinkText("Committee RNI Review Submitted")).Exists);
            Wait.Until(h => workspace.GetStudyState() == "Post-Review");
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());
            
            //Log in as a Coordinator (IRBC), Prepare Letter.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.PrepareLetter("Review of New Information");
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyID());

            //As the Director (IRBD), locate the above RNI by clicking on the IRB tab/New Information Reports/ and select the RNI and Submit Action Response.
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            // TODO MISSING SUBMIT ACTION RESPONSE
            workspace.SubmitActionResponse();
            Assert.IsTrue(workspace.GetStudyState() == "Action Required", "State expected not in 'Action Required'.  State currently:  " + workspace.GetStudyID());

            // Log in as one of the Responsible Parties determined in Step 7 and Submit Action Response.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.SubmitActionResponse();
            Assert.IsTrue(workspace.GetStudyState() == "Action Submitted", "State expected not in 'Action Submitted'.  State currently:  " + workspace.GetStudyID());

            //Log in as assigned Coordinator and Review Required Actions (Were the actions completed as required? – YES)
            workspace.ReviewRequiredActions(true);
            Assert.IsTrue(workspace.GetStudyState() == "Post Review", "State expected not in 'Post Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned Coordinator, Prepare Letter.
            workspace.PrepareLetter("Review of New Information");
            Assert.IsTrue(workspace.GetStudyState() == "Post Review", "State expected not in 'Post Review'.  State currently:  " + workspace.GetStudyID());

            // As the assigned Coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Complete", "State expected not in 'Complete'.  State currently:  " + workspace.GetStudyID());
        }

        [Test]
        public void SignificantRNIThroughRNIPreReviewThenDiscard()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var inboxPage = new Inbox();
            var rniSmartformPage = new RNISmartform();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var ActivitiesNav = new ActivitiesNav();

            Store.LoginAsUser(Users.Pi);
            inboxPage.ImgCreateNewRNI.Click();
            // Fill in just required info
            rniSmartformPage.TxtRNIShortTitle.Value = RNITitle;
            rniSmartformPage.TxtDateAware.Value = "03/02/2014";
            rniSmartformPage.TxtDescriptionOfProblem.Value = "This is a RNI test for " + RNITitle;
            rniSmartformPage.RdoIsIncreasedRiskNo.Click();
            rniSmartformPage.RdoNeedRevisionNo.Click();
            rniSmartformPage.RdoConsentRequiresRevisionkNo.Click();
            rniSmartformPage.BtnContinue.Click();
            rniSmartformPage.BtnFinish.Click();
            Wait.Until(h => new Link(By.LinkText("Reportable Information Opened")).Exists);
            Assert.IsTrue(new Link(By.LinkText("Reportable Information Opened")).Exists, "'Reportable Information Opened' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Submission", "State expected not in 'Pre-Submission'.  State currently: " + workspace.GetStudyState());

            // Submit RNI as Pi
            workspace.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'Pre-Review' activity not found for:  " + RNITitle);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as IRBC
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);

            // Assign coordinator as IRBC
            workspace.AssignCoordinator("Orlando Max (irbc)");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // As the assigned Coordinator (IRBC) and Submit RNI Pre-Review (One of the top four determinations)
            workspace.SubmitRNIPreReview("", true, SubmitRNIPreReview.Determinations.UnanticipatedProblem);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyID());

            // Log in as PI and Discard RNI.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(RNITitle);
            workspace.Discard();
            Assert.IsTrue(new CCElement(By.LinkText("Discarded")).Exists, "Discarded link does not exist for:  " + RNITitle);
        }
        

        

    }
}
