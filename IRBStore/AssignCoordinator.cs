using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AssignCoordinator : ActivityPopup
    {
        public CCElement
            //BtnOk = new CCElement(By.Id("okBtn")),
            FirstUser = new CCElement(By.CssSelector("input[name='_IRBSubmission_AssignOwner.loggedFor.owner']"));


        public AssignCoordinator(string projectId, string activityName) : base(projectId, activityName)
        {
        }


        //public void SelectUser(string lastName)
        //{
            
        //}
    }
}
