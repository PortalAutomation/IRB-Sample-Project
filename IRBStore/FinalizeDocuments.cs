using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class FinalizeDocuments
    {
        public CCElement 
            ChkApprove = new CCElement(By.CssSelector("input[type='checkbox'][name*='attachment']")),
            BtnOk = new CCElement(By.Id("okBtn"));
    }
}
