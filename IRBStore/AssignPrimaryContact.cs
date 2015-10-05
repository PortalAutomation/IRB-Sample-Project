using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AssignPrimaryContact : ActivityPopup
    {
        public Button
            BtnSelectUser = new Button(By.CssSelector("input[value='Select...']"));
            //BtnOk = new Button(By.Id("okBtn"));

        public AssignPrimaryContact(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public ChooserPopup PrimaryContactPopup = new ChooserPopup("Person");

        public void SelectPrimaryContact(string user)
        {
            BtnSelectUser.Click();
            PrimaryContactPopup.SwitchTo();
            PrimaryContactPopup.SelectValue(user);
            PrimaryContactPopup.BtnOk.Click();
            PrimaryContactPopup.SwitchBackToParent();
            BtnOk.Click();
        }
    }
}
