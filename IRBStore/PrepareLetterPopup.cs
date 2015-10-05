using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class PrepareLetterPopup : ActivityPopup
    {
        public CCElement
            CmbDraftLetterTemplate =
                new CCElement(
                    By.CssSelector("select[name='_IRBSubmission_PrepareLetter.customAttributes.selectedFileTemplate']")),
            BtnGenerate = new CCElement(By.CssSelector("input[value='Generate']"));
            //BtnOk = new CCElement(By.Id("okBtn"));

        public PrepareLetterPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }
}
