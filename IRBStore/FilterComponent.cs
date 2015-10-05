using PortalSeleniumFramework.PrimitiveElements;
using OpenQA.Selenium;

namespace IRBAutomation.IRBStore
{
    public class FilterComponent
    {
        // need to implement filter by criteria
        public Button GoButton = new Button(By.CssSelector("input[value='Go']")),
            ClearButton = new Button(By.CssSelector("input[value='Clear']"));

        public Link 
            LnkAdvanced = new Link(By.LinkText("Advanced")),
            LnkBasic = new Link(By.LinkText("Basic"));

        public TextBox
            QueryCriteria1 = new TextBox(By.CssSelector("input[id*='queryCriteria1")),
            QueryCriteria2 = new TextBox(By.CssSelector("input[id*='queryCriteria2")),
            QueryCriteria3 = new TextBox(By.CssSelector("input[id*='queryCriteria3")),
            QueryCriteria4 = new TextBox(By.CssSelector("input[id*='queryCriteria3")),
            QueryCriteria5 = new TextBox(By.CssSelector("input[id*='queryCriteria3"));

        public Select
            QueryField1 = new Select(By.CssSelector("select[id*='queryField1']")),
            QueryField2 = new Select(By.CssSelector("select[id*='queryField2']")),
            QueryField3 = new Select(By.CssSelector("select[id*='queryField3']")),
            QueryField4 = new Select(By.CssSelector("select[id*='queryField4']")),
            QueryField5 = new Select(By.CssSelector("select[id*='queryField5']"));

        //LnkAdvanced = new CCElement(By.CssSelector("a[id*='advancedLink']")),
        //    QueryCriteria1 = new CCElement(By.CssSelector("input[id*='queryCriteria1']")),
        //    QueryCriteria2 = new CCElement(By.CssSelector("input[id*='queryCriteria2']")),
        //    QueryCriteria3 = new CCElement(By.CssSelector("input[id*='queryCriteria3']")),
        //    QueryField1 = new CCElement(By.CssSelector("select[id*='queryField1']")),
        //    QueryField2 = new CCElement(By.CssSelector("select[id*='queryField2']")),
        //    QueryField3 = new CCElement(By.CssSelector("select[id*='queryField3']")),

        //Inbox.LnkAdvanced.Click();
        //    Wait.Until(h => Inbox.QueryField1.Displayed);
        //    Inbox.QueryField1.SelectByInnerText("Name");
        //    Inbox.QueryCriteria1.Text = "AutoTest";
        //    Inbox.QueryField2.SelectByInnerText("State");
        //    Inbox.QueryCriteria2.Text = "Post-Review";
        //    Wait.Until(d => Inbox.BtnGo.Enabled);
        //    Inbox.BtnGo.Click();

        public void EnterCriteria(string queryCriteria, string queryFieldValue)
        {
            
        }

    }
}
