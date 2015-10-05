using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    public class AddAttachmentPopup : IPopup
    {
        public string Title { get { return "Add Attachment"; } }

        public Button
            BtnBrowse = new Button(By.CssSelector("input[name='_Attachment.customAttributes.draftVersion.targetURL']")),
            BtnOk = new Button(By.CssSelector("input[value='OK']"));
    }
}
