namespace TestFrameSuiteView
{
    using System;

    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Text;
    using System.IO;

    // NetOffice
    using Excel = NetOffice.ExcelApi;

    // Excel DNA
    using ExcelDna.Integration;
    using ExcelDna.Integration.CustomUI;

    using ActionWord.Presenter;
    using General.Presenter;
    using Cluster.Presenter;

    using ActionwordManagement.Module;
    using Configuration.Module;
    using TestDesign.Module;
    using TestManagement.Module;


    public class MyAddIn : IExcelAddIn
    {
        private static Excel.Application application;
        

        public void AutoOpen()
        {
            // Init stuff
            application = new Excel.Application(null, ExcelDnaUtil.Application);
            Debug.WriteLine("Excel.Application version: " + application.Version);

            try
            {
                // Add style combo for Office < 2007
                var sep = application.DecimalSeparator;
                string v = application.Version;
                var version = Convert.ToDouble(v.Replace(".", sep));

                Debug.WriteLine("version: {0}", version);

                if (version < 12.0)
                {
                    var cb = application.CommandBars["Formatting"];
                    var ctl = cb.FindControl(MsoControlType.msoControlComboBox, 1732);
                    Debug.WriteLine("Check style control");

                    if (ctl == null)
                    {
                        Debug.WriteLine("Add style control");
                        cb.Controls.Add(MsoControlType.msoControlComboBox, 1732, null, 3);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // Open the current TestFrame database
            TFDatabase.Open();

            while (!TFDatabase.IsActive)
            {
                Debug.WriteLine("No active TestFrame database. Select one...");
                TestFrameRibbon.tftShowDatabaseManager();
                TFDatabase.Open();
            }

            if (Globals.toolbar_version.CompareTo(HelpSupport.ShowWhatsNew) > 0)
            {
                HelpSupport.ShowHelp("whatsnew");
                HelpSupport.ShowWhatsNew = Globals.toolbar_version;
            }

            
        }

        public void AutoClose()
        {
            // nothing
        }
    }

    [ComVisible(true)]
    public class TestFrameRibbon : ExcelRibbon
    {

        #region Actionword Management Module
        
        public static void tftManageActionwords()
        {
            ActionwordManagementActions.ShowManageActionWords();
        }

        #endregion

        #region Configuration Module
        // Settings
        // Options
        // Database selection / management

        public static void tftShowGeneralOptions()
        {
            ConfigurationActions.ShowGeneralOptions();
        }

        /// <summary>
        /// Actionword database manager and selector
        /// </summary>
        public static void tftShowDatabaseManager()
        {
            ConfigurationActions.ShowDatabaseManager();
        }

        #endregion

        #region Test Design Module

        public static void tftSelectActionWord()
        {
            TestDesignActions.SelectActionword();
        }

        public static void tftInsertNewTestCondition()
        {
            TestFrame.InsertNewTestCondition();
        }

        public static void tftInsertNewTestCase()
        {
            TestFrame.InsertNewTestCase();
        }

        public static void tftInsertNewTag()
        {
            TestFrame.InsertNewTag();
        }

        #endregion

        #region Manual Execution Module
               
        #endregion

        #region Test Management Module

        public string getContent_Clusters(IRibbonControl control)
        {
            StringBuilder xml = new StringBuilder(@"<menu xmlns=""http://schemas.microsoft.com/office/2006/01/customui"" >");

            var file_list = TestManagementActions.GetClusterFiles();
            int i = 0;

            foreach (string file in file_list)
            {
                string nm = Path.GetFileNameWithoutExtension(file);
                xml.AppendFormat(@"<button id='cluster{0}' imageMso='MicrosoftExcel' label='{1}' tag='{2}' onAction='DoInsertCluster' />", ++i, nm, file);
            }
            //TODO: Refresh cluster list
            //xml.AppendFormat( @"<button id='refresh_clusters'  label='Refresh clusters' tag='refresh cluster list' onAction='RefreshClusterList' />" );
            xml.Append( @"</menu>" );

            return xml.ToString();
        } 
        public void DoInsertCluster(IRibbonControl control)
        {
            var fn = control.Tag;
            TestManagementActions.CreateNewClusterWorkbook(fn);
            
        }

        public static void tftShowManageCategories()
        {
            TestManagementActions.ShowManageCategories();
        }


        #endregion

        #region Help group

        public static void tftWhatsNew()
        {
            HelpSupport.ShowHelp("whatsnew");
        }

        

        public static void tftShowInfo()
        {
            var dlg = new About();
            dlg.ShowDialog();
        }

        #endregion

       
    }
}
