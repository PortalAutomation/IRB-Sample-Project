using PortalSeleniumFramework.Helpers;

namespace IRBAutomation.IRBStore
{
    public class Users
    {
        public static readonly UserAccount Admin = new UserAccount("administrator", "xxxx");
        public static readonly UserAccount TestRead = new UserAccount("testRead", "xxxx");
        public static readonly UserAccount TestWrite = new UserAccount("testWrite", "xxxx");
        public static readonly UserAccount TestRespond = new UserAccount("testRespond", "xxxx");
        public static readonly UserAccount Pi = new UserAccount("pi", "xxxx");
        public static readonly UserAccount Irbd = new UserAccount("irbd", "xxxx");
        public static readonly UserAccount Comm4 = new UserAccount("comm4", "xxxx");
        public static readonly UserAccount Irbc = new UserAccount("irbc", "xxxx");
    }
}
