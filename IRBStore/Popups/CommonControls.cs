using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    public class CommonPopupControls : PageElement
    {
        public void SelectStudyDeterminations(Determinations value)
        {
            string name = "";
            switch (value)
            {
                case Determinations.Approved: { name = "Approved"; break; }
                case Determinations.ModificationsRequiredToSecureApproved: { name = @"Modifications Required to Secure ""Approved"""; break; }
                case Determinations.NotHumanResearch: { name = "Not Human Research"; break; }
                case Determinations.ModificationsRequiredToSecureNotHumanResearch: { name = @"Modifications Required to Secure ""Not Human Research"""; break; }
                case Determinations.HumanResearchNotEngaged: { name = @"Human Research, Not Engaged"; break; }
                case Determinations.ModificationsRequiredToSecureHumanResearchNotEngaged: { name = @"Modifications Required to Secure ""Human Research, Not Engaged"""; break; }
                case Determinations.Deferred: { name = @"Deferred"; break; }
                case Determinations.Disapproved: { name = @"Disapproved"; break; }
            }
            var rdo = new Radio(By.XPath(".//td[text()='" + name + "']/../td/input[1]"));
            rdo.Click();
            Trace.WriteLine("Checking option: " + value);
        }

        public enum Determinations
        {
            Approved,
            ModificationsRequiredToSecureApproved,
            NotHumanResearch,
            ModificationsRequiredToSecureNotHumanResearch,
            HumanResearchNotEngaged,
            ModificationsRequiredToSecureHumanResearchNotEngaged,
            Deferred,
            Disapproved
        }


    }
}
