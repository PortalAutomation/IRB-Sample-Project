using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCWebUIAuto.Helpers;
using CCWebUIAuto.Pages;
using CCWebUIAuto.Pages.Components;
using CCWebUIAuto.PrimitiveElements;
using IRBAutomation.Helpers;
using IRBAutomation.IRBStore;
using IRBAutomation.IRBStore.Popups;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.TestCases
{
    [TestFixture]
    public class ModSystemTests : BaseTest
    {
        [Test]
        public void ModificationOtherPartsOfStudyCreationThroughCommitteeReviewWithModificationRequiredToApproved()
        {
            string targetStudy = "AutoTest-" + DataGen.String(5);
            EntityClonerUtil.CloneEntity("STUDY00000024", targetStudy, true);

            var ActionsNav = new ActivitiesNav();
            var submissionPage = new IRBSubmissions();
            var ModificationsPage = new InitialModCrSmartForm();
            var ModificationInfoPage = new ModificationInformation();
            var Workspace = new IRBWorkspace();
            var StudySF = new InitialStudySmartForm();

            Store.LoginAsUser(Users.Pi);
            submissionPage.OpenSubmissionByAllSubmissions(targetStudy);


            // Log in as the PI and go to the IRB/Active tab and select an Approved study, 
            // Create Modification/CR , select “Modification” and “Other parts of the study”.  
            // Make changes to one or more views of the study.
            ActionsNav.ImgCreateModCr.Click();
            ModificationsPage.RdoModification.Click();
            ModificationsPage.ChooseModificationScope(Scope.OtherPartsOfTheStudy);
            
            //ModificationsPage.BtnContinue
            ModificationsPage.BtnContinue.Click();
            ModificationInfoPage.TxtSummary.Value = "This is a test summary for modification.";
            ModificationsPage.BtnContinue.Click();

            //StudySF.StudyTeamMembersPage.AddStudyTeamMember("Elmira (comm1)", false, false, AddStudyTeamMemberPopup.Roles.CoInvestigator, AddStudyTeamMemberPopup.Roles.ResearchAssistant);
            //StudySF.StudyTeamMembersPage.BtnContinue.Click();

            // save and exit smartform
            StudySF.LnkSave.Click();
            StudySF.LnkExit.Click();

            // As the PI, Submit the modification.
            Workspace.SubmitMod(Users.Pi.UserName, Users.Pi.Password);
            PopUpWindow.SwitchTo(targetStudy, true);

            // why does Exists not retry?
            Wait.Until(h => new CCElement(By.LinkText("Submitted")).Exists);
            Assert.IsTrue(new CCElement(By.LinkText("Submitted")).Exists, "Attempted submitted mod does not exist for:  " + targetStudy);

            // Log in as the assigned IRB Coordinator and Submit Pre-Review.

            // Log in as assigned IRB Coordinator, Assign to Meeting.

            // As the assigned IRB Coordinator,  Submit Committee Review (modifications required to secure “approved” determination)

            // Log in as assigned IRB Coordinator and Finalize Documents.
            
            // As the assigned IRB Coordinator, Prepare Letter.

            // As the assigned IRB Coordinator, Send Letter.

            
        }
    }
}
