using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class AssignToMeetingPopup : ActivityPopup
    {
        public Radio RdoFirstMeeting = new Radio(By.CssSelector("input[name='_IRBSubmission_ScheduleForMeeting.loggedFor.customAttributes.currentAgendaItem.customAttributes.meeting'][type='radio']"));

        public AssignToMeetingPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }
}
