using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class CreateNewMeeting : SmartFormPage
    {
        public TextBox 
            TxtLocation = new TextBox(By.CssSelector("input[name='_Meeting.customAttributes.location']")),
            TxtMeetingDate = new TextBox(By.Id("webr_uniqueID_0"));

        public Button BtnOk = new Button(By.Id("ok_btn_Top"));
        /// <summary>
        /// Creates a date
        /// </summary>
        /// <param name="committeeName"></param>
        /// <param name="location"></param>
        public void CreateMeeting(string committeeName, string location)
        {
            SelectCommittee(committeeName);
            DateTime utcTime = DateTime.UtcNow;
            string datePatt = @"M/d/yyyy hh:mm tt";
            string meetingDate = utcTime.AddDays(20).ToString(datePatt);
            Trace.WriteLine("Setting committee meeting time to:  " + meetingDate);
            TxtMeetingDate.Value = meetingDate;
            TxtLocation.Value = location;
            BtnOk.Click();
        }

        public void SelectCommittee(string name)
        {
            Radio RdoCommittee = new Radio(By.XPath(".//td[text()='" + name + "']/../td[1]/input[1]"));
            RdoCommittee.Click();
        }
    }
}
