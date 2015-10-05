using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using IRBAutomation.IRBStore.Popups;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class IRBWorkspace : PageElement
    {
        // contains history, funding, project contacts, documents, reviews, snapshots

        public readonly CCElement StudyStateContainer = new CCElement(By.Id("__IRBSubmission.status.ID_container"));

        public readonly Inbox InboxPage = new Inbox();
        public readonly ActivitiesNav ActivitiesNav = new ActivitiesNav();
        public readonly ActionsNav ActionsNav = new ActionsNav();
        public readonly IRBProjectLog ProjectLogHistory = new IRBProjectLog("History");
        public readonly IRBProjectLog ProjectLogReviews = new IRBProjectLog("Reviews");
        
        // helper methods for frequently used operations

        public string GetStudyID()
        {
            CCElement container = new CCElement(By.Id("__IRBSubmission.ID_container"));
            CCElement ID = container.GetDescendant((".//span[3]"));
            return ID.Text;
        }

        public string GetStudyState()
        {
            CCElement target = StudyStateContainer.GetDescendant(".//span[3]");
            return target.Text;
        }

        /// <summary>
        /// Verify the first entry link in history log
        /// </summary>
        /// <param name="linkText"></param>
        /// <returns></returns>
        public bool VerifyFirstHistoryLogEntry(string linkText)
        {
            return ProjectLogHistory.VerifyFirstRowLink(linkText);
        }

        public void AddReviewComments(string comments)
        {
            ActivitiesNav.LnkAddReviewComments.Click();
            var addReviewComments = new AddReviewCommentsPopup(this.GetStudyID(), "Add Review Comments");
            addReviewComments.SwitchTo();
            addReviewComments.TxtReviewComments.Value = comments;
            addReviewComments.BtnOk.Click();
            addReviewComments.SwitchBackToParent();
            //ProjectLogReviews.ReviewsTab.Click();
            // hopefully no specific wait required...
        }

        public void AssignCoordinator(string user)
        {
            ActivitiesNav.LnkAssignCoordinator.Click();
            var AssignCoordinatorPopup = new AssignCoordinator(this.GetStudyID(), "Assign Coordinator");
            AssignCoordinatorPopup.SwitchTo();
            Radio irbCoordinator = new Radio(By.XPath(".//td[text()='" + user + "']/../td[1]/input[1]"));
            if (irbCoordinator.Exists)
            {
                irbCoordinator.Click();
            }
            else
            {
                Trace.WriteLine("User:  " + user + " could not be found.  Selecting first available user.");
                AssignCoordinatorPopup.FirstUser.Click();    
            }
            AssignCoordinatorPopup.BtnOk.Click();
            AssignCoordinatorPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("IRB Coordinator Assigned")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("IRB Coordinator Assigned") == true);
        }

        public void AssignDesignatedReviewer(string user)
        {
            ActivitiesNav.LnkAssignDesignatedReviewer.Click();
            var assignDesignatedReviewerPopup = new AssignDesignatedReviewer(this.GetStudyID(), "Assign Designated Reviewer");
            assignDesignatedReviewerPopup.SwitchTo();
            assignDesignatedReviewerPopup.CmbDesignatedReviewer.SelectByInnerText(user);
            assignDesignatedReviewerPopup.OkBtn.Click();
            assignDesignatedReviewerPopup.SwitchBackToParent();
            //Wait.Until((d) => new CCElement(By.LinkText("Assigned to Designated Reviewer")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Assigned to Designated Reviewer") == true);
        }

        public void AssignToCommitteeReview()
        {
            ActivitiesNav.LnkAssignToCommitteeReview.Click();
            var assignToCommitteeReview = new AssignToCommitteReview(this.GetStudyID(), "Assign To Committee Review");
            assignToCommitteeReview.SwitchTo();
            assignToCommitteeReview.BtnOk.Click();
            assignToCommitteeReview.SwitchBackToParent();
            //Wait.Until((d) => new CCElement(By.PartialLinkText("Assigned To Committee Review")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Assigned To Committee Review") == true);
        }

        public void AssignMeetingByFirstMeeting()
        {
            ActivitiesNav.LnkAssignToMeeting.Click();
            var assignToMeetingPage = new AssignToMeetingPopup(this.GetStudyID(), "Assign to Meeting");
            assignToMeetingPage.SwitchTo();
            assignToMeetingPage.RdoFirstMeeting.Click();
            assignToMeetingPage.BtnOk.Click();
            assignToMeetingPage.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("Assigned to Meeting")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Assigned to Meeting") == true);
        }

        public void ConfirmExternalIRB()
        {
            ActivitiesNav.LnkConfirmExternalIrb.Click();
            var confirmExternalPage = new ConfirmExternalIRBPopup(this.GetStudyID(), "Confirm External IRB");
            confirmExternalPage.SwitchTo();
            confirmExternalPage.BtnOk.Click();
            confirmExternalPage.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("External")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("External") == true);
        }

        public void AssignPrimaryContact(string user)
        {
            ActivitiesNav.LnkAssignPrimaryContact.Click();
            var assignPrimaryContact = new AssignPrimaryContact(this.GetStudyID(), "Assign Primary Contact");
            assignPrimaryContact.SwitchTo();
            assignPrimaryContact.SelectPrimaryContact(user);
            assignPrimaryContact.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Assigned Primary Contact")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Assigned Primary Contact") == true);
        }

        public void Discard()
        {
            ActivitiesNav.LnkDiscard.Click();
            var discardPopup = new DiscardPopup(this.GetStudyID(), "Discard");
            discardPopup.SwitchTo();
            discardPopup.BtnOk.Click();
            discardPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("Discarded")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Discarded") == true);
        }

        public void FinalizeDocuments()
        {
            ActivitiesNav.LnkFinalizeDocuments.Click();
            var finalizeDoc = new FinalizeDocumentsPopup(this.GetStudyID(), "Finalize Documents");
            finalizeDoc.SwitchTo();
            finalizeDoc.BtnOk.Click();
            finalizeDoc.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("Finalize")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Finalized Documents") == true);
        }


        public void ReviewRequiredActions(bool actionsCompleted = false)
        {
            ActivitiesNav.LnkReviewRequiredActions.Click();
            var reviewRequiredActions = new ReviewRequiredActionsPopup(this.GetStudyID(), "Review Required Actions");
            reviewRequiredActions.SwitchTo();
            if (actionsCompleted)
            {
                reviewRequiredActions.RdoActionsCompletedYes.Click();
            }
            else
            {
                reviewRequiredActions.RdoActionsCompletedNo.Click();
            }
            reviewRequiredActions.BtnOk.Click();
            reviewRequiredActions.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("Required Actions Reviewed")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Required Actions Reviewed") == true);
        }

        public void ReviewRequiredModifications(bool submitMod = true)
        {
            ActivitiesNav.LnkReviewRequiredModifications.Click();
            var reviewReqMod = new ReviewRequiredModificationsPopup(this.GetStudyID(), "Review Required Modifications");
            reviewReqMod.SwitchTo();
            if (submitMod)
            {
                reviewReqMod.RdoModsRequiredYes.Click();
            }
            else
            {
                reviewReqMod.RdoModsRequiredNo.Click();
            }
            reviewReqMod.BtnOk.Click();
            reviewReqMod.SwitchBackToParent();
            //Wait.Until(h => new Link(By.PartialLinkText("Required Modifications Reviewed")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Required Modifications Reviewed") == true);
            

        }

        public void Submit(string user, string password)
        {
            ActivitiesNav.LnkSubmit.Click();
            var submitPopup = new SubmitPopup(this.GetStudyID(),"Submit");
            submitPopup.SwitchTo();
            submitPopup.BtnOk.Click();
            submitPopup.ConfirmCredentials(user, password);
            // need to switch back to default content
            submitPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Submitted") == true);
        }

        public void SubmitRNI(string user, string password)
        {
            ActivitiesNav.LnkSubmitRNI.Click();
            var submitRniPopup = new SubmitRNIPopup(this.GetStudyID(), "Submit RNI");
            submitRniPopup.SwitchTo();
            submitRniPopup.BtnOk.Click();
            submitRniPopup.ConfirmCredentials(user, password);
            // need to switch back to default content
            submitRniPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("RNI Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("RNI Submitted") == true);
        }

        public void SubmitActionResponse(string notes = "")
        {
            ActivitiesNav.LnkSubmitActionResponse.Click();
            var submitActionResponse = new SubmitActionResponsePopup(this.GetStudyID(), "Submit Action Response");
            submitActionResponse.SwitchTo();
            submitActionResponse.TxtNotes.Value = notes;
            submitActionResponse.BtnOk.Click();
            submitActionResponse.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Action Response Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Action Response Submitted") == true);
        }

        /// <summary>
        /// Only covers required fields
        /// </summary>
        public void SubmitPreReviewForCR()
        {
            ActivitiesNav.LnkSubmitPreReview.Click();
            var submitPreReview = new SubmitPreReviewPopup(this.GetStudyID(), "Submit Pre-Review");
            submitPreReview.SwitchTo();
            submitPreReview.RadioBtnSubmitPreReviewYes.Click();
            submitPreReview.BtnOk.Click();
            submitPreReview.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Pre-Review Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Pre-Review Submitted") == true);
        }

        /// <summary>
        /// Only covers required fields
        /// </summary>
        public void SubmitPreReviewForStudy(params SubmitPreReviewPopup.TypeOfResearch[] researchTypes)
        {
            ActivitiesNav.LnkSubmitPreReview.Click();
            var submitPreReview = new SubmitPreReviewPopup(this.GetStudyID(), "Submit Pre-Review");
            submitPreReview.SwitchTo();
            submitPreReview.SpecifyRiskLevel(true);
            submitPreReview.SpecifyResearchTypes(researchTypes);
            submitPreReview.SubmitPreview();
            submitPreReview.BtnOk.Click();
            submitPreReview.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Pre-Review Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Pre-Review Submitted") == true);
        }

        public void SubmitRNIPreReview(string notes = "", bool readyToSubmit = true, params SubmitRNIPreReview.Determinations[] determinations)
        {
            ActivitiesNav.LnkSubmitRNIPreReview.Click();
            var submitRniPreReviewPopup = new SubmitRNIPreReview(this.GetStudyID(), "Submit RNI Pre-Review");
            submitRniPreReviewPopup.SwitchTo();
            foreach (SubmitRNIPreReview.Determinations det in determinations)
            {
                submitRniPreReviewPopup.SelectDetermination(det);
            }
            // TODO put in notes field
            if (readyToSubmit)
            {
                submitRniPreReviewPopup.rdoSubmitPreviewYes.Click();
            }
            else
            {
                submitRniPreReviewPopup.rdoSubmitPreviewNo.Click();
            }
            submitRniPreReviewPopup.BtnOk.Click();
            submitRniPreReviewPopup.SwitchBackToParent();
            if (readyToSubmit)
            {
                Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("RNI Pre-Review Submitted") == true);
            }
            else
            {
                Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("RNI Pre-Review Updated") == true);
            }
        }



        public void SubmitMod(string user, string password)
        {
            ActivitiesNav.LnkSubmit.Click();
            var submitModPopup = new SubmitRNIPopup(this.GetStudyID(), "Submit");
            submitModPopup.SwitchTo();
            submitModPopup.BtnOk.Click();
            submitModPopup.ConfirmCredentials(user, password);
            submitModPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Submitted") == true);
        }

        public void RequestClarificationByCommitteeMember(string notes = "")
        {
            ActivitiesNav.LnkRequestPreReviewClarificationByCommitteeMember.Click();
            var requestClarificationByCommitteeMemberPage = new RequestClarificationByCommitteeMemberPopup(this.GetStudyID(), "Request Clarification by Committee Member");
            requestClarificationByCommitteeMemberPage.SwitchTo();
            requestClarificationByCommitteeMemberPage.TxtInfo.Value = notes;
            requestClarificationByCommitteeMemberPage.BtnOk.Click();
            requestClarificationByCommitteeMemberPage.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Clarification Requested by Committee Member")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Clarification Requested by Committee Member") == true);
        }

        public void RequestClarificationByDesignatedReviewer(string notes = "")
        {
            ActivitiesNav.LnkRequestClarificationByDesignatedReviewer.Click();
            var clarificationRequestPopup = new ClarificationRequestPopup(this.GetStudyID(), "Request Clarification by Designated Reviewer");
            clarificationRequestPopup.SwitchTo();
            clarificationRequestPopup.BtnOk.Click();
            clarificationRequestPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Clarification Requested by Designated Reviewer")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Clarification Requested by Designated Reviewer") == true);
        }

        public void RequestPreReviewClarification()
        {
            ActivitiesNav.LnkRequestPreReviewClarification.Click();
            var clarificationRequestPopup = new RequestPreReviewClarificationPopup(this.GetStudyID(), "Request Pre-Review Clarification");
            clarificationRequestPopup.SwitchTo();
            clarificationRequestPopup.BtnOk.Click();
            clarificationRequestPopup.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Clarification Requested")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Clarification Requested") == true);
        }

        public void SubmitResponse(string user, string password, string notes = "")
        {
            ActivitiesNav.LnkSubmitResponse.Click();
            var responsePage = new SubmitResponsePopup(this.GetStudyID(), "Submit Response");
            responsePage.SwitchTo();
            responsePage.TxtInfo.Value = notes;
            responsePage.BtnOk.Click();
            responsePage.ConfirmCredentials(user, password);
            responsePage.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Response Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Response Submitted") == true);
        }

        public void SubmitRniCommitteeReviewPopupRequiredOnlyFields(string forVote = "0", string againstVote = "0", string recusedVote = "0", string absentVote = "0", string abstainedVote = "0")
        {
            ActivitiesNav.LnkSubmitRNICommitteeReview.Click();
            var submitPage = new SubmitRNICommitteeReviewPopup(this.GetStudyID(), "Submit RNI Committee Review");
            submitPage.SwitchTo();
            submitPage.TxtFor.Value = forVote;
            submitPage.TxtAgainst.Value = againstVote;
            submitPage.TxtRecused.Value = recusedVote;
            submitPage.TxtAbsent.Value = absentVote;
            submitPage.TxtAbstained.Value = abstainedVote;
            submitPage.RdoRdyForSubmissionYes.Click();
            submitPage.BtnOk.Click();
            submitPage.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Committee RNI Review Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Committee RNI Review Submitted") == true);
            
        }

        public void PrepareLetter(string draftLetterTemplateName = "")
        {
            ActivitiesNav.LnkPrepareLetter.Click();
            var prepareLetterPage = new PrepareLetterPopup(this.GetStudyID(), "Prepare Letter");
            prepareLetterPage.SwitchTo();
            if (draftLetterTemplateName == "")
            {
                prepareLetterPage.CmbDraftLetterTemplate.SelectByIndex(1);
            }
            else
            {
                prepareLetterPage.CmbDraftLetterTemplate.SelectByInnerText(draftLetterTemplateName);    
            }
            prepareLetterPage.BtnGenerate.Click();
            Wait.Until(d => new CCElement(By.PartialLinkText("Correspondence")).Exists);
            prepareLetterPage.BtnOk.Click();
            prepareLetterPage.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Prepared Letter") == true);
            //Wait.Until((d) => new CCElement(By.LinkText("Prepared Letter")).Exists);
        }

        public void SendLetter()
        {
            ActivitiesNav.LnkSendLetter.Click();
            var sendLetterPage = new SendLetterPopup(this.GetStudyID(), "Send Letter");
            sendLetterPage.SwitchTo();
            Wait.Until(h => sendLetterPage.IsDisplayed());
            sendLetterPage.BtnOk.Click();
            sendLetterPage.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Letter Sent") == true);
            //Wait.Until((d) => new CCElement(By.LinkText("Letter Sent")).Exists);
        }

        /// <summary>
        /// Verify this form
        /// </summary>
        /// <param name="determinations"></param>
        /// <param name="notes"></param>
        public void SubmitRNICommitteeReview(
            string forVote = "0", 
            string againstVote = "0", 
            string recusedVote = "0", 
            string absentVote = "0", 
            string abstainedVote = "0", 
            params SubmitRNICommitteeReviewPopup.Determinations[] determinations)
        {
            ActivitiesNav.LnkSubmitRNICommitteeReview.Click();
            var submitRniPreReviewPopup = new SubmitRNICommitteeReviewPopup(this.GetStudyID(), "Submit RNI Committee Review");
            submitRniPreReviewPopup.SwitchTo();
            foreach (SubmitRNICommitteeReviewPopup.Determinations det in determinations)
            {
                submitRniPreReviewPopup.SelectDetermination(det);
            }
            //submitRniPreReviewPopup.SelectDetermination(determinations);
            // TODO put in notes field
            submitRniPreReviewPopup.TxtFor.Value = forVote;
            submitRniPreReviewPopup.TxtAgainst.Value = againstVote;
            submitRniPreReviewPopup.TxtRecused.Value = recusedVote;
            submitRniPreReviewPopup.TxtAbsent.Value = absentVote;
            submitRniPreReviewPopup.TxtAbstained.Value = abstainedVote;
            submitRniPreReviewPopup.RdoRdyForSubmissionYes.Click();
            submitRniPreReviewPopup.BtnOk.Click();
            submitRniPreReviewPopup.SwitchBackToParent();
            Wait.Until(h => new Link(By.LinkText("Committee RNI Review Submitted")).Exists); 
        }

        public void SubmitRNIDesignatedReview(params SubmitDesignatedRNIReviewPopup.Determinations[] determinations)
        {
            ActivitiesNav.LnkSubmitRNIDesignatedReview.Click();
            var submitRNIDesignatedReview = new SubmitDesignatedRNIReviewPopup(this.GetStudyID(), "Submit RNI Designated Review");
            submitRNIDesignatedReview.SwitchTo();
            foreach (SubmitDesignatedRNIReviewPopup.Determinations det in determinations)
            {
                submitRNIDesignatedReview.SelectDetermination(det);
            }
            if (!submitRNIDesignatedReview.ChkConflictingInterest.Checked)
            {
                submitRNIDesignatedReview.ChkConflictingInterest.Click();
            }
            submitRNIDesignatedReview.RdoSubmitReviewYes.Click();
            submitRNIDesignatedReview.BtnOk.Click();
            submitRNIDesignatedReview.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("RNI Designated Review Submitted")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("RNI Designated Review Submitted") == true);
        }

        /// <summary>
        /// TODO only works with approved, ready to submit is only yes, review category choices, categories......
        /// </summary>
        /// <param name="determinations"></param>
        public void SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations determination, bool rdyToSubmit = true)
        {
            ActivitiesNav.LnkSubmitDesignatedReview.Click();
            var submitDesignatedReview = new SubmitDesignatedReview(this.GetStudyID(), "Submit Designated Review");
            submitDesignatedReview.SwitchTo();
            // TODO Select only approved
            submitDesignatedReview.SelectStudyDeterminations(determination);
            
            // workaround // TODO implement further categories
            submitDesignatedReview.ChkFirstExemptCategory.SetCheckBox(true);
            submitDesignatedReview.ChkConflictingInterest.SetCheckBox(true);
            // TODO HAck on review level
            submitDesignatedReview.RdoFirstReviewLevel.Click();
            if (rdyToSubmit)
            {
                submitDesignatedReview.RdoReadyToSubmitThisReviewYes.Click();
            }
            else
            {
                submitDesignatedReview.RdoReadyToSubmitThisReviewNo.Click();
            }
            submitDesignatedReview.BtnOk.Click();
            submitDesignatedReview.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Designated Review Submitted") == true);
        }

        /// <summary>
        /// Use this if submitting a designated review with modifications determination
        /// </summary>
        /// <param name="determination"></param>
        /// <param name="modificationString"></param>
        /// <param name="rdyToSubmit"></param>
        public void SubmitDesignatedReviewForStudy(SubmitDesignatedReview.StudyDeterminations determination, string modificationString, bool rdyToSubmit = true)
        {
            ActivitiesNav.LnkSubmitDesignatedReview.Click();
            var submitDesignatedReview = new SubmitDesignatedReview(this.GetStudyID(), "Submit Designated Review");
            submitDesignatedReview.SwitchTo();
            // TODO Select only approved
            submitDesignatedReview.SelectStudyDeterminations(determination);
            // workaround // TODO implement further categories
            submitDesignatedReview.ChkFirstExemptCategory.SetCheckBox(true);
            submitDesignatedReview.ChkConflictingInterest.SetCheckBox(true);
            // TODO HAck on review level
            submitDesignatedReview.RdoFirstReviewLevel.Click();
            submitDesignatedReview.TxtModification.Value = modificationString;
            if (rdyToSubmit)
            {
                submitDesignatedReview.RdoReadyToSubmitThisReviewYes.Click();
            }
            else
            {
                submitDesignatedReview.RdoReadyToSubmitThisReviewNo.Click();
            }
            submitDesignatedReview.BtnOk.Click();
            submitDesignatedReview.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Designated Review Submitted") == true);
        }

        public void SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations determination, string forVote = "", string againstVote = "", string recusedVote = "", string absentVote = "", string abstainedVote = "", string substitutions = "", bool readyToSubmit = true)
        {
            ActivitiesNav.LnkSubmitCommitteeReview.Click();
            var committReviewPage = new SubmitCommitteeReviewPopup(this.GetStudyID(), "Submit Committee Review");
            committReviewPage.SwitchTo();
            committReviewPage.SelectStudyDeterminations(determination);
            committReviewPage.SelectRiskLevel(true);
            committReviewPage.TxtLastApprovalDate.Value = "5/10/2050";
            committReviewPage.TxtFor.Value = forVote;
            committReviewPage.TxtAgainst.Value = againstVote;
            committReviewPage.TxtRecused.Value = recusedVote;
            committReviewPage.TxtAbsent.Value = absentVote;
            committReviewPage.TxtAbstained.Value = abstainedVote;
            committReviewPage.TxtSubstitutions.Value = substitutions;
            committReviewPage.SpecifyReadyToSubmit(true);
            committReviewPage.BtnOk.Click();
            committReviewPage.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Committee Review Submitted") == true);
        }

        public void SubmitCommitteeReview(SubmitCommitteeReviewPopup.Determinations determination, string reasons, string forVote = "", string againstVote = "", string recusedVote = "", string absentVote = "", string abstainedVote = "", string substitutions = "", bool readyToSubmit = true)
        {
            ActivitiesNav.LnkSubmitCommitteeReview.Click();
            var committReviewPage = new SubmitCommitteeReviewPopup(this.GetStudyID(), "Submit Committee Review");
            committReviewPage.SwitchTo();
            committReviewPage.SelectStudyDeterminations(determination);
            committReviewPage.SelectRiskLevel(true);
            committReviewPage.TxtLastApprovalDate.Value = "5/10/2050";
            committReviewPage.TxtFor.Value = forVote;
            committReviewPage.TxtAgainst.Value = againstVote;
            committReviewPage.TxtRecused.Value = recusedVote;
            committReviewPage.TxtAbsent.Value = absentVote;
            committReviewPage.TxtAbstained.Value = abstainedVote;
            committReviewPage.TxtSubstitutions.Value = substitutions;
            committReviewPage.TxtRecommendedChangesReasons.Value = reasons;
            committReviewPage.SpecifyReadyToSubmit(true);
            committReviewPage.BtnOk.Click();
            committReviewPage.SwitchBackToParent();
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Committee Review Submitted") == true);
        }

        public void UpdateExternalIRBStatus(bool externalIRBClosed = true)
        {
            ActivitiesNav.LnkUpdateExternalIRBStatus.Click();
            var updateExternalIRB = new UpdateExternalIRBStatusPopup(this.GetStudyID(), "Update External IRB Status");
            updateExternalIRB.SwitchTo();
            if (externalIRBClosed)
            {
                updateExternalIRB.RdoExternalIRBClosedYes.Click();
            }
            else
            {
                updateExternalIRB.RdoExternalIRBClosedNo.Click();
            }
            updateExternalIRB.BtnOk.Click();
            updateExternalIRB.SwitchBackToParent();
            //Wait.Until(h => new Link(By.LinkText("Updated External IRB Status")).Exists);
            Wait.Until(h => ProjectLogHistory.VerifyFirstRowLink("Updated External IRB Status") == true);
        }


        public void AssignProxy(string personLastName, string userConfirmation, string userConfirmationPassword)
        {
            throw new NotImplementedException();
        }

    }
}
