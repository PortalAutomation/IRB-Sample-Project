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
    [TestFixture]
    public class ModificationSystemTests : BaseTest
    {
        [Test]
        public void ModificationOtherPartsOfStudyCreationThroughCommitteeReviewWithModificationRequiredToApproved()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();
            
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            CRSmartForm.LnkSave.Click();
            CRSmartForm.LnkExit.Click();

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
        public void ModificationOtherPartsOfStudyCreationThroughCommitteeReviewApproved()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            StudySF.NewSubmissionSmartformPage.TxtDescription.Value = "This is a modified description for this test.";
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
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Approved, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        public void ModificationOtherPartsOfStudyCreationThroughCommitteeReviewDeferred()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            StudySF.NewSubmissionSmartformPage.TxtDescription.Value = "This is a modified description for this test.";
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
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Deferred, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        public void ModificationOtherPartsOfStudyCreationThroughNCRApproved()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            StudySF.NewSubmissionSmartformPage.TxtDescription.Value = "This is a modified description for this test.";
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var ModName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Orlando Max (irbc)");
            
            // Log in as IRBC, Request Pre-Review Clarification
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.RequestPreReviewClarification();
            Assert.IsTrue(workspace.GetStudyState() == "Clarification Requested (Pre-Review)", "State expected not in 'Clarification Requested (Pre-Review)'.  State currently: " + workspace.GetStudyState());

            // Log in as PI, Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password, "This is my response!");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            // Log in as IRBC and Assign Coordinator who is different than the IRBC in the initial study.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Ira Stein (irbd)");
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned coordinator, Submit Committee Review (approved determination).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved);
           Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
           
        }

        [Test]
        public void ModificationOtherPartsOfTheStudyCreationThroughCommitteeReviewDisapproved()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();

            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();

            StudySF.NewSubmissionSmartformPage.TxtDescription.Value = "This is a modified description for this test.";
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var modName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(modName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(modName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            // need determination, risk level
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Disapproved, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        public void ModificationOtherPartsOfTheStudyCreation_NCR_Mods_Required_Approved()
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
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            StudySF.NewSubmissionSmartformPage.TxtDescription.Value = "This is a modified description for this test.";
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var ModName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (modifications required to secure “approved” determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "These are modification notes: ", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());
            
            // As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved, "These are more modification notes.", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void ModificationStudyTeamOnlyCreationThroughNCR_Approved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select 
            // “Modification” and “Study team member information”.  Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var ModName = Web.PortalDriver.Title;
            
            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned coordinator, Submit Committee Review (approved determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.Approved);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned IRBC and Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.FinalizeDocuments();

            //As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
            
        }

        [Test]
        public void ModificationStudyTeamOnlyCreationThroughCommitteeReviewApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select 
            // “Modification” and “Study team member information”.  Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
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
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Approved, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        public void ModificationStudyTeamOnlyCreationThroughCommitteeReview_Deferred()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select 
            // “Modification” and “Study team member information”.  Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
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
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Deferred, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        public void ModificationStudyTeamOnlyCreationThroughCRWithModsRequired_Approved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select 
            // “Modification” and “Study team member information”.  Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var ModName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            // As the IRBC¸ Submit Committee Review (modifications required to secure “approved”).
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.ModificationsRequiredToSecureApproved, "modification comments", "2", "0", "0", "0", "0", "", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // Log in as the assigned IRBC, Review Required Modifications (Yes to the question “Were the modifications completed as required?”)
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void ModificationStudyTeamOnlyCreation_NCR_ModsRequired_Approved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select “Modification” and “Study team member information”.  
            // Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var ModName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRBC, Assign Designated Reviewer. 
            workspace.AssignDesignatedReviewer("Harry Smith (comm4)");
            Assert.IsTrue(workspace.GetStudyState() == "Non-Committee Review", "State expected not in 'Non-Committee Review'.  State currently:  " + workspace.GetStudyState());

            // Log in as the Designated Reviewer and Submit Designated Review (modifications required to secure “approved” determination).
            Store.LoginAsUser(Users.Comm4);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations.ModificationsRequiredToSecureApproved, "These are modification notes: ", true);
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned IRB Coordinator, Finalize Documents.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.FinalizeDocuments();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Prepare Letter.
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Required", "State expected not in 'Modifications Required'.  State currently:  " + workspace.GetStudyState());

            // Log in as the PI and Submit Response.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.SubmitResponse(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(workspace.GetStudyState() == "Modifications Submitted", "State expected not in 'Modifications Submitted'.  State currently:  " + workspace.GetStudyState());

            // Log in as IRBC, Review Required Modifications.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.ReviewRequiredModifications();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Prepare Letter.
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(ModName);
            workspace.PrepareLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Post-Review", "State expected not in 'Post-Review'.  State currently:  " + workspace.GetStudyState());

            // As the assigned coordinator, Send Letter.
            workspace.SendLetter();
            Assert.IsTrue(workspace.GetStudyState() == "Approved", "State expected not in 'Approved'.  State currently:  " + workspace.GetStudyState());
        }

        [Test]
        public void ModificationStudyTeamOnlyCreationThroughCR_Disapproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var CRSmartForm = new InitialModCrSmartForm();
            var StudySF = new InitialStudySmartForm();
            var workspace = new IRBWorkspace();
            var irbSubmissionsPage = new IRBSubmissions();
            var actionsNav = new ActionsNav();

            // Log in as the PI and go to the IRB/Active tab and select an Approved study, Create Modification/CR , select “Modification” and “Study team member information”.  
            // Change some aspect of the Study Team member form.
            Store.LoginAsUser(Users.Pi);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(targetStudy);
            actionsNav.ImgCreateModCr.Click();

            // Fill in just required info
            CRSmartForm.ChooseModCRPurpose(SubmissionPurpose.Modification);
            CRSmartForm.ChooseModificationScope(Scope.StudyTeamMemberInformation);
            CRSmartForm.BtnContinue.Click();
            // Modification Page 
            CRSmartForm.TxtSummarizeModifications.Value = "These are the new modifications!  :/ :) :P";
            CRSmartForm.BtnContinue.Click();
            // Add a study team member, save, exit
            StudySF.StudyTeamMembersPage.AddStudyTeamMember(false, false, "", AddStudyTeamMemberPopup.Roles.CoInvestigator);
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            //As the PI, Submit.
            workspace.Submit(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("Submitted")).Exists, "'Submitted' activity not found for:  " + targetStudy);
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review", "State expected not in 'Pre-Review'.  State currently: " + workspace.GetStudyState());
            var modName = Web.PortalDriver.Title;

            // Log in as IRBD and assign Coordinator (skip this this step if the copy option is enabled).
            Store.LoginAsUser(Users.Irbd);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(modName);
            workspace.AssignCoordinator("Orlando Max (irbc)");

            //Log in as the assigned coordinator and Submit Pre-Review
            Store.LoginAsUser(Users.Irbc);
            irbSubmissionsPage.OpenSubmissionByAllSubmissions(modName);
            workspace.SubmitPreReviewForCR();
            Assert.IsTrue(workspace.GetStudyState() == "Pre-Review Completed", "State expected not in 'Pre-Review Completed'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Assign Meeting.
            workspace.AssignMeetingByFirstMeeting();
            Assert.IsTrue(workspace.GetStudyState() == "Committee Review", "State expected not in 'Committee Review'.  State currently:  " + workspace.GetStudyState());

            //As the assigned coordinator, Submit Committee Review (approved determination).
            workspace.SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations.Disapproved, "modification comments", "2", "0", "0", "0", "0", "", true);
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
        [Ignore("Under Construction")]
        public void SuspendedStudyWithModificationStudyTeamOnlyCreationThroughCRtoApproved()
        {
            
        }

    }
}
