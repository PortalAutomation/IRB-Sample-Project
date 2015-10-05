using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    class ManageAncillaryReviews : ActivityPopup
    {
        public Button
            BtnAdd = new Button(By.CssSelector("input[value='Add']"));
        //    BtnOk = new Button(By.Id("okBtn"));

        public ManageAncillaryReviews(string projectId, string activityName) : base(projectId, activityName)
        {
          
        }

        //public string Title {
        //    get { return "Add Ancillary Review"; }
        //}
    }
}
