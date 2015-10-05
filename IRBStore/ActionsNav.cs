using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Pages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class ActionsNav : PageElement
    {
        public readonly Button
            ImgEditStudy = new Button(By.CssSelector("img[alt='Click here to edit the protocol.']")),
            ImgCreateModCr = new Button(By.CssSelector("img[alt='New Modification / Continuing Review']")),
            ImgCreateNewStudyLink = new Button(By.CssSelector("img[alt='Create New Study']")),
            ImgCreateNewRNI = new Button(By.CssSelector("img[alt='Report New Information']")),
            ImgCreateNewMeeting = new Button(By.CssSelector("img[alt='Create Meeting']")),
            ImgCreateNewCommittee = new Button(By.CssSelector("img[alt='Create Committee']"));
    }
}
