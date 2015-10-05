using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class SubmitPopup : ActivityPopup
    {
        public SubmitPopup(string projectId, string activityName) : base(projectId, activityName)
        {
        }
    }

    public class SubmitRNIPopup : ActivityPopup
    {
        public SubmitRNIPopup(string projectId, string activityName)
            : base(projectId, activityName)
        {
        }
    }

    public class SubmitModPopup : ActivityPopup
    {
        public SubmitModPopup(string projectId, string activityName)
            : base(projectId, activityName)
        {
        }
    }
}
