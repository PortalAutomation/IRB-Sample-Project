using System;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.Components;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore.Popups;
using OpenQA.Selenium;
using Container = PortalSeleniumFramework.PrimitiveElements.Container;

namespace IRBAutomation.IRBStore
{
    // should inherit from component (activities)
    public class ActivitiesNav : CCPage
    {
        public Link
            LnkSubmit = new Link(By.LinkText("Submit")),
            LnkDiscard = new Link(By.LinkText("Discard")),
            LnkAddReviewComments = new Link(By.LinkText("Add Review Comments")),
            LnkAssignCoordinator = new Link(By.LinkText("Assign Coordinator")),
            LnkAssignDesignatedReviewer = new Link(By.LinkText("Assign Designated Reviewer")),
            LnkAssignPrimaryContact = new Link(By.LinkText("Assign Primary Contact")),
            LnkAssignToCommitteeReview = new Link(By.LinkText("Assign To Committee Review")),
            LnkConfirmExternalIrb = new Link(By.LinkText("Confirm External IRB")),
            LnkSubmitDesignatedReview = new Link(By.LinkText("Submit Designated Review")),
            LnkSubmitPreReview = new Link(By.LinkText("Submit Pre-Review")),
            LnkSubmitRNIPreReview = new Link(By.LinkText("Submit RNI Pre-Review")),
            LnkSubmitRNICommitteeReview = new Link(By.LinkText("Submit RNI Committee Review")),
            LnkSubmitRNIDesignatedReview = new Link(By.LinkText("Submit RNI Designated Review")),
            LnkSubmitRNI = new Link(By.LinkText("Submit RNI")),
            LnkFinalizeDocuments = new Link(By.LinkText("Finalize Documents")),
            LnkPrepareLetter = new Link(By.LinkText("Prepare Letter")),
            LnkSendLetter = new Link(By.LinkText("Send Letter")),
            LnkManageAncillaryReviews = new Link(By.LinkText("Manage Ancillary Reviews")),
            LnkManageGuestList = new Link(By.LinkText("Manage Guest List")),
            LnkCopySubmission = new Link(By.LinkText("Copy Submission")),
            LnkAddComment = new Link(By.LinkText("Add Comment")),
            LnkAddPrivateComment = new Link(By.LinkText("Add Private Comment")),
            LnkAssignIRB = new Link(By.LinkText("Assign IRB")),
            LnkAddRelatedGrant = new Link(By.LinkText("Add Related Grant")),
            LnkTerminate = new Link(By.LinkText("Terminate")),
            LnkUpdateExternalIRBStatus = new Link(By.LinkText("Update External IRB Status")),
            LnkRequestClarificationByDesignatedReviewer = new Link(
                    By.CssSelector(
                        "a[title='Designated reviewer sends the submission back to the study team for clarifications.']")),
            LnkRequestPreReviewClarification = new Link(By.LinkText("Request Pre-Review Clarification")),
            LnkRequestPreReviewClarificationByCommitteeMember = new Link(By.LinkText("Request Clarification by Committee Member")),
            LnkReviewRequiredActions = new Link(By.LinkText("Review Required Actions")),
            LnkReviewRequiredModifications = new Link(By.LinkText("Review Required Modifications")),
            LnkSubmitResponse = new Link(By.LinkText("Submit Response")),
            LnkSubmitActionResponse = new Link(By.LinkText("Submit Action Response")),
            LnkSubmitCommitteeReview = new Link(By.LinkText("Submit Committee Review")),
            LnkAssignPIProxy = new Link(By.LinkText("Assign PI Proxy")),
            LnkAssignToMeeting = new Link(By.LinkText("Assign to Meeting")),
            LnkWithdraw = new Link(By.LinkText("Withdraw")),
            // shortcut links
            LnkSubmissions = new Link(By.LinkText("Submissions")),
            LnkMeetings = new Link(By.LinkText("Meetings")),
            LnkReports = new Link(By.LinkText("Reports")),
            LnkLibrary = new Link(By.LinkText("Library")),
            LnkHelpCenter = new Link(By.LinkText("Help Center"));

        public Button
            ImgEditStudy = new Button(By.CssSelector("img[alt='Click here to edit the protocol.']")),
            ImgCreateModCr = new Button(By.CssSelector("img[alt='New Modification / Continuing Review']")),
            ImgCreateNewStudyLink = new Button(By.CssSelector("img[alt='Create New Study']")),
            ImgCreateNewRNI = new Button(By.CssSelector("img[alt='Report New Information']"));

        public Container
            ContainerIRBState = new Container(By.Id("readonly"));

        public override void NavigateTo()
        {
            throw new NotImplementedException();
        }
    }
}
