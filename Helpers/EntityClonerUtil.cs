using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalSeleniumFramework.Helpers;
using PortalSeleniumFramework.Pages.BasePages;
using IRBAutomation.IRBStore;
using NUnit.Framework;

namespace IRBAutomation.Helpers
{
    public static class EntityClonerUtil
    {
        public static void CloneEntity(string EntityToClone, string uniqueStudyID = "", bool isMod = false)
        {
            string uniqueStudyName = DataGen.String(5);
            Store.LoginAsUser(Users.Admin);
            var cmd = new CommandWindow();

            if (isMod)
            {
                var script = @"                
                var proj = getResultSet('_IRBSubmission').query(""ID='" + EntityToClone + @"'"").elements.item(1);
                var cloneJob = EntityCloner.createRequest(proj, proj, 'j');
                cloneJob.customizeHandling('_IRBSubmission.resourceContainer', 'cloneEntity');
                cloneJob.customizeHandling('_IRBSubmission.customAttributes.draftStudy', 'cloneEntity');
                cloneJob.startRequestNow();
                var clone = cloneJob.rootEntity;
                clone.ID = 'STUDY-" + uniqueStudyID + @"'
                clone.Name = '" + uniqueStudyID + @"'
                clone.ResourceContainer.Name = '" + uniqueStudyID + @"'
                clone.customAttributes.draftStudy.Name = 'DRAFTSTUDY" + uniqueStudyID + @"'
                 ";
                Assert.AreEqual("", cmd.Run(script));
            }

            else
            {
                var script = @"                
                var proj = getResultSet('_IRBSubmission').query(""ID='" + EntityToClone + @"'"").elements.item(1);
                var cloneJob = EntityCloner.createRequest(proj, proj, 'j');
                cloneJob.customizeHandling('_IRBSubmission.resourceContainer', 'cloneEntity');
                cloneJob.customizeHandling('_IRBSubmission.customAttributes.draftStudy', 'cloneEntity');
                cloneJob.startRequestNow();
                var clone = cloneJob.rootEntity;
                clone.ID = 'STUDY-" + uniqueStudyID + @"'
                clone.Name = '" + uniqueStudyID + @"'
                clone.ResourceContainer.Name = '" + uniqueStudyID + @"'
                ";
                Assert.AreEqual("", cmd.Run(script));
            }
        }
    }
}
