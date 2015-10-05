using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AddAncillaryReview : IPopup
    {
        public Button
            //BtnSelectOrg = new Button(By.CssSelector("com.webridge.entity.Entity[OID[211CBE62C7544C49A2586E72129CB8DB]]_selectBtn")),
            //BtnSelectPerson = new Button(By.Id("com.webridge.entity.Entity[OID[91D44370A2B7AB438EBC8A77E5FD6626]]_selectBtn")),
            BtnOk = new Button(By.CssSelector("input[value='OK']"));

        public Select
            SelReviewType = new Select(By.CssSelector("select[name='_ancillaryReview.customAttributes.ancillaryReviewSelection.customAttributes.reviewType']"));

        public Radio
            RdoResponseRequiredYes = new Radio(By.CssSelector("input[name='_ancillaryReview.customAttributes.ancillaryReviewSelection.customAttributes.required'][value='yes']")),
            RdoResponseRequiredNo = new Radio(By.CssSelector("input[name='_ancillaryReview.customAttributes.ancillaryReviewSelection.customAttributes.required'][value='no']"));

        public Container
            ReviewOrgContainer = new Container(By.Id("__ancillaryReview.customAttributes.ancillaryReviewSelection.customAttributes.reviewOrganization_container")),
            ReviewPersonContainer = new Container(By.Id("__ancillaryReview.customAttributes.ancillaryReviewSelection.customAttributes.reviewPerson_container"));
            
        public string Title { get { return "Add Ancillary Review"; } }

        public SelectPerson SelectPersonPage = new SelectPerson("Person");
        //public IPopup SelectOrgPage = new ("Organization");
        public ChooserPopup SelectOrgPage = new ChooserPopup("Organization");

        public void SelectPerson(string lastName)
        {
            IEnumerable<CCElement> buttons = ReviewPersonContainer.GetDescendants(".//td[3]/span/input");
            CCElement BtnSelectPerson = buttons.FirstOrDefault(h => h.GetAttributeValue("type") == "button");
            if (BtnSelectPerson != null)
            {
                Trace.WriteLine("Clicking on Select Person...");
                BtnSelectPerson.Click();
            }
            SelectPersonPage.SwitchTo();
            SelectPersonPage.SelectValue(lastName);
            SelectPersonPage.BtnOk.Click();
            SelectPersonPage.SwitchBackToParent();
        }

        public void SelectOrganization(string orgName)
        {
            IEnumerable<CCElement> buttons = ReviewPersonContainer.GetDescendants(".//td[3]/span/input");
            CCElement BtnSelectOrg = buttons.FirstOrDefault(h => h.GetAttributeValue("type") == "button");
            if (BtnSelectOrg != null)
            {
                Trace.WriteLine("Clicking on Select Organization...");
                BtnSelectOrg.Click();
            }
            SelectOrgPage.SwitchTo();
            SelectOrgPage.SelectValue("Immunology");
            SelectOrgPage.BtnOk.Click();
            SelectOrgPage.SwitchBackToParent();
        }
    }
}
