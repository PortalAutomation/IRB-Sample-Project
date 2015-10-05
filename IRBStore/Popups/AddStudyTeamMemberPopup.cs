using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore.Popups
{
    public class AddStudyTeamMemberPopup : IPopup
    {
        public string Title { get { return "Add Study Team Member"; } }

        public Button
            BtnSelectTeamMember = new Button(By.CssSelector("input[value='Select...']")),
            BtnOk = new Button(By.CssSelector("input[value='OK']"));

        public ChooserPopup selectPersonPopup = new ChooserPopup("Person");

        public Radio
            RadioConsentProcessNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_StudyTeamMemberInfo.customAttributes.involvedInConsentProcess'][value='no']")),
            RadioConsentProcessYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_StudyTeamMemberInfo.customAttributes.involvedInConsentProcess'][value='yes']")),
            RadioFinancialInterestNo =
                new Radio(
                    By.CssSelector(
                        "input[name='_StudyTeamMemberInfo.customAttributes.hasFinancialInterest'][value='no']")),
            RadioFinancialInterestYes =
                new Radio(
                    By.CssSelector(
                        "input[name='_StudyTeamMemberInfo.customAttributes.hasFinancialInterest'][value='yes']"));

        public void SelectTeamMember(string user = "")
        {
            BtnSelectTeamMember.Click();
            selectPersonPopup.SwitchTo();
            if (user == "")
            {
                Radio firstChoice = new Radio(By.CssSelector("input[type='radio']"));
                firstChoice.Click();
            }
            else
            {
                selectPersonPopup.SelectValue(user);    
            }
            selectPersonPopup.BtnOk.Click();
            selectPersonPopup.SwitchBackToParent();
        }

        public void SelectRoles(params Roles[] roles)
        {
            string name = "";

            foreach (var role in roles)
            {
                switch (role)
                {
                    case Roles.CoInvestigator:
                    {
                        name = "Co-investigator";
                        break;
                    }
                    case Roles.DataAnalyst:
                    {
                        name = "Data Analyst";
                        break;
                    }
                    case Roles.ResearchAssistant:
                    {
                        name = "Research Assistant";
                        break;
                    }
                    case Roles.Statistician:
                    {
                        name = "Statistician";
                        break;
                    }
                    case Roles.LayObserver:
                    {
                        name = "Lay Observer";
                        break;
                    }
                }
                var chkbox = new Checkbox(By.XPath(".//td[contains(.,'" + name + "')]/../td/table/tbody/tr/td/input[1]"));
                chkbox.Click();
                Trace.WriteLine("Checking option: " + role);
            }
        }

        public void SpecifyConsentProcessInvolvement(bool isInvolved = false)
        {
            if (isInvolved)
            {
                RadioConsentProcessYes.Click();
            }
            else
            {
                RadioConsentProcessNo.Click();
            }
        }

        public void SpecifyFinancialInterest(bool hasFinancialInterest = false)
        {
            if (hasFinancialInterest)
            {
                RadioFinancialInterestYes.Click();
            }
            else
            {
                RadioFinancialInterestNo.Click();
            }
        }

        public enum Roles
        {
            CoInvestigator,
            DataAnalyst,
            ResearchAssistant,
            Statistician,
            LayObserver
        }

    }
}
