using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CommonUtilities;
using IRB.Helpers;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.ScreenCapture;
using NUnit.Framework;
using OpenQA.Selenium;
using WebAutomation;

namespace IRB
{
    public class BaseTest
    {
        // create a user to login as for the test
        public readonly UserAccount admin = Store.ExpectUserAccount("Administrator", "1234");
        private ScreenCaptureJob scj;
        public string _testScreenCaptureFileName = string.Empty;

        [SetUp]
        public void Setup()
        {
            const String pathname = @"..\..\autoConfig.xml";
            try
            {
               ClickPortalUI.AutoConfig.read(pathname);
               if (ClickPortalUI.AutoConfig.ContainsKey("EnableVideoRecording"))
                {
                    _testScreenCaptureFileName = @"C:\TempLogs\VideoRecordings\" + TestContext.CurrentContext.Test.FullName + ".wmv";
                    if (ClickPortalUI.AutoConfig["EnableVideoRecording"].ToLower() == "true")
                    {
                        if (File.Exists(_testScreenCaptureFileName))
                        {
                            File.Delete(_testScreenCaptureFileName);
                        }
                        scj = new ScreenCaptureJob();
                        scj.OutputScreenCaptureFileName = _testScreenCaptureFileName;
                        scj.Start();
                        Trace.WriteLine(String.Format("Starting recording for test: {0}", TestContext.CurrentContext.Test.FullName));
                    }
                }
                ClickPortalUI.Initialize();
                ((IJavaScriptExecutor)CCWebUIAuto.webDriver).ExecuteScript("window.showModalDialog = window.openWindow;");
                Trace.WriteLine(String.Format("Executing test: {0}", TestContext.CurrentContext.Test.FullName));
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }
        }

        [TearDown]
        public void TearDown()
        {
            bool deleteRecording = true;

            try
            {
                if (TestContext.CurrentContext.Result.Status == TestStatus.Failed ||
                    TestContext.CurrentContext.Result.Status == TestStatus.Inconclusive)
                {
                    deleteRecording = false;
                    // Want to pause to view problem on video recording before shutting down browser
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                CCWebUIAuto.HandleException(ex);
            }
            finally
            {
                Trace.WriteLine("Closing browser.");
                CCWebUIAuto.webDriver.Quit();
                if (ClickPortalUI.AutoConfig.ContainsKey("EnableVideoRecording"))
                {
                    if (ClickPortalUI.AutoConfig["EnableVideoRecording"].ToLower() == "true")
                        scj.Stop();
                        scj.Dispose();
                        if (deleteRecording == true)
                    {
                        Trace.WriteLine("Removing unnecessary test recording:  " + _testScreenCaptureFileName);
                        File.Delete(_testScreenCaptureFileName);
                    }
                }
                // kill any remaining IE or IEDriverServer processes
                var procCollection = Process.GetProcesses();
                foreach (var proc in procCollection)
                {
                    if (proc.ProcessName.StartsWith("iexplore") || (proc.ProcessName.StartsWith("IEDriverServer")))
                    {
                        try
                        {
                            //proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            CCWebUIAuto.HandleException(ex);
                        }
                    }
                }
            }
        }
    }
}
