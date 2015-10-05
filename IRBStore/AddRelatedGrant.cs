using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using NUnit.Framework;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AddRelatedGrant : ActivityPopup
    {
        public AddRelatedGrant(string projectId, string activityName) : base(projectId, activityName)
        {
        }

        public Button BtnAdd = new Button(By.CssSelector("input[value='Add']"));
        
    }

    public class AddClickIRBGrantInfo : IPopup
    {
        public string Title { get { return "Add ClickIRBGrantInfo"; } }

        public TextBox TxtGrantId = new TextBox(By.CssSelector("input[name='_ClickIRBGrantInfo.customAttributes.grantID']"));
        public TextBox TxtGrantTitle = new TextBox(By.CssSelector("input[name='_ClickIRBGrantInfo.customAttributes.grantTitle']"));

        public Button BtnOk = new Button(By.CssSelector("input[value='OK']"));

    }
}
