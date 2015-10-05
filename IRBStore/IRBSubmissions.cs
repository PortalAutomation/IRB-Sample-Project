using System;
using PortalSeleniumFramework;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages.BasePages;
using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;
using PortalSeleniumFramework.Pages.Components;

namespace IRBAutomation.IRBStore
{
    public class IRBSubmissions : LayoutInitialBasedPage
    {
        // Tabs:
        // In-Review, Active, Archived, New Information Reports, All Submissions
        public InReviewTab InReviewTab = new InReviewTab();
        public ActiveTab ActiveTab = new ActiveTab();
        public ArchivedTab ArchivedTab = new ArchivedTab();
        public NewInformationReportsTab NewInformationReportsTab = new NewInformationReportsTab();
        public AllSubmissionsTab AllSubmissionsTab = new AllSubmissionsTab();
        
        //public CCElement DivProjectListing = new CCElement(By.Id("component3A410C7E87F5F749BC8AD015360F38AB"));

        /// <summary>
        /// Opens a submission by navigating to all submissions tab
        /// </summary>
        /// <param name="name"></param>
        /// <param name="partialMatch"></param>
        public void OpenSubmission(string name, bool partialMatch = false)
        {
            if (partialMatch)
            {
                Wait.Until(h => new CCElement(By.PartialLinkText(name)).Exists);
                var targetLink = new CCElement(By.PartialLinkText(name));
                targetLink.Click();
            }
            else
            {
                Wait.Until(h => new CCElement(By.PartialLinkText(name)).Exists);
                var targetLink = new CCElement(By.LinkText(name));
                targetLink.Click();
            }
        }

        public void OpenSubmissionByAllSubmissions(string name, bool partialMatch = false)
        {
            this.AllSubmissionsTab.NavigateTo();
            this.AllSubmissionsTab.ProjectsComponent.LnkAdvanced.Click();
            this.AllSubmissionsTab.ProjectsComponent.SetCriteria("Name", name);
            if (partialMatch)
            {
                Wait.Until(h => new CCElement(By.PartialLinkText(name)).Exists);
                var targetLink = new CCElement(By.PartialLinkText(name));
                targetLink.Click();
                Wait.Until(h => Web.PortalDriver.Title == name);
            }
            else
            {
                Wait.Until(h => new CCElement(By.LinkText(name)).Exists);
                var targetLink = new CCElement(By.LinkText(name));
                targetLink.Click();
                Wait.Until(h => Web.PortalDriver.Title == name);
            }
        }

        public IRBSubmissions() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[75DEF2383B5DB042BC82D89A3BF4B589]]") {}
        
    }

    public class InReviewTab : LayoutInitialBasedPage
    {
        public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("In-Review");

        public InReviewTab()
            : base(
                "/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[C2FAFCE62F8DE6439FE29695A3FC6CDB]]&Tab2=com.webridge.entity.Entity[OID[EB6CC1BF392FF342A75754E2EC85E8DE]]"
                )
        {
            
        }
    }

    public class ActiveTab : LayoutInitialBasedPage
    {
        public ActiveTab() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[C2FAFCE62F8DE6439FE29695A3FC6CDB]]&Tab2=com.webridge.entity.Entity[OID[15134901AFB8BD408094C9444210948B]]") {}
        
        public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("Active");
    }

     public class ArchivedTab : LayoutInitialBasedPage
    {
         public ArchivedTab() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[C2FAFCE62F8DE6439FE29695A3FC6CDB]]&Tab2=com.webridge.entity.Entity[OID[5D712BD764CD1844A7C8206740575178]]") { }

         public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("Archived");

    }

     public class NewInformationReportsTab : LayoutInitialBasedPage
    {
         public NewInformationReportsTab() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[C2FAFCE62F8DE6439FE29695A3FC6CDB]]&Tab2=com.webridge.entity.Entity[OID[07F80D64DC86434A909CDD2BA7664AB7]]") { }

         public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("RNI Project Listing");

    }

     public class AllSubmissionsTab : LayoutInitialBasedPage
    {
         public AllSubmissionsTab() : base("/Rooms/DisplayPages/LayoutInitial?Container=com.webridge.entity.Entity[OID[C2FAFCE62F8DE6439FE29695A3FC6CDB]]&Tab2=com.webridge.entity.Entity[OID[B48751D97F8B2C448498A5A07A9F7CE9]]") { }

         public readonly ProjectListingComponent ProjectsComponent = new ProjectListingComponent("All Submissions Project Listing");

    }

    


}
