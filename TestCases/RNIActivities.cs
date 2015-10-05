using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore;
using NUnit.Framework;
using OpenQA.Selenium;
using IRBAutomation.Helpers;

namespace IRBAutomation.TestCases
{
    [TestFixture]
    public class RNIActivities : BaseTest
    {
        
        [Test(Description = "Insignificant RNI submission")]
        public void SubmitRNI()
        {
            string RNITitle = "AutoTestRNI-" + DataGen.String(5);
            var Inbox = new Inbox();
            var RNISmartformPage = new RNISmartform();
            var StudyWorkspacePage = new IRBWorkspace();
            
            Store.LoginAsUser(Users.Pi);
            Inbox.ImgCreateNewRNI.Click();
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
            // SubmitRNI
            StudyWorkspacePage.SubmitRNI(Users.Pi.UserName, Users.Pi.Password);
            Assert.IsTrue(new Link(By.LinkText("RNI Submitted")).Exists, "'RNI Submitted' activity not found for:  " + RNITitle);
            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Pre-Review", "State of RNI: Not in pre-review state");
        }

        //[Test]
        //[Ignore("Duplicated by System Test InsignificantRniWithClarificationRequestedToAcknowledged")]
        public void SubmitRNIPreReview()
        {
            // clone a RNI in pre-review state

            string targetStudy = "RNI-" + DataGen.String(5);
            // select something new
            EntityClonerUtil.CloneEntity("RNI00000005", targetStudy);

            var Inbox = new Inbox();
            var LeftActionNav = new ActivitiesNav();
            var StudyWorkspacePage = new IRBWorkspace();
            
            Store.LoginAsUser(Users.Irbd);

            Inbox.LnkAdvanced.Click();
            Wait.Until(h => Inbox.QueryField1.Displayed);
            Inbox.QueryField1.SelectByInnerText("State");
            Wait.Until(h => Inbox.QueryCriteria1.Enabled);
            Inbox.QueryCriteria1.Text = "Pre-Review";
            Wait.Until(d => Inbox.BtnGo.Enabled);
            Inbox.BtnGo.Click();
            Wait.Until(d => Inbox.BtnGo.Enabled);

            // change this
            Inbox.OpenStudy(targetStudy);
            
            string id = StudyWorkspacePage.GetStudyID();
            var AssignCordPopup = new AssignCoordinator(id, "Assign Coordinator");

            // Assign coordinator
            LeftActionNav.LnkAssignCoordinator.Click();
            AssignCordPopup.SwitchTo();
            AssignCordPopup.FirstUser.Click();
            AssignCordPopup.SwitchBackToParent();
            
            // Submit RNI-SubmitPreReviewPopup

            LeftActionNav.LnkSubmitRNIPreReview.Click();
            var SubmitRNIPreReviewPage = new SubmitRNIPreReview(id, "Submit RNI Pre-Review");
            SubmitRNIPreReviewPage.SwitchTo();
            SubmitRNIPreReviewPage.SelectDetermination(IRBStore.SubmitRNIPreReview.Determinations.NoneOfTheAbove);
            SubmitRNIPreReviewPage.rdoSubmitPreviewYes.Click();
            SubmitRNIPreReviewPage.BtnOk.Click();
            SubmitRNIPreReviewPage.SwitchBackToParent();

            Wait.Until(h => new Link(By.LinkText("RNI Pre-Review Submitted")).Exists);
            Assert.IsTrue(new Link(By.LinkText("RNI Pre-Review Submitted")).Exists, "'RNI Submitted Pre-Review' activity not found for:  " + targetStudy);

            Assert.IsTrue(StudyWorkspacePage.GetStudyState() == "Acknowledged", "State of RNI: Not in acknowledged state");
        }
    }
}
