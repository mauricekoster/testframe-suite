
namespace TestManagement.Module
{

    using System.Collections.Generic;
    using System;
    using General.Presenter;
    using Cluster.Presenter;
    using Configuration.Presenter;
    
    using System.IO;

    public class TestManagementActions
    {
        public static void CreateNewClusterWorkbook(string cluster_filename)
        {
            // get template path...
            var fn = cluster_filename;
            var path = Settings.GetTemplatePath();
            fn = Path.Combine(path, "Clusters", fn);

            TestFrame.CreateNewCluster(fn);

            //Globals.ShowMessage(String.Format("File: {0}", fn));
        }

        public static List<string> GetClusterFiles()
        {
            var ret = new List<string>();
            string clustersPath = Path.Combine( Settings.GetTemplatePath(), "Clusters" );

            if (clustersPath != null && Directory.Exists(clustersPath))
            {
                foreach (string file in Directory.GetFiles(clustersPath))
                {
                    var ext = Path.GetExtension(file);
                    if (ext.Equals(".xlt") || ext.Equals(".xltx"))
                        ret.Add(Path.GetFileName(file));
                }
            }

            return ret;

        }

        public static void ShowManageCategories()
        {
            var dlg = new ManageCategories();
            dlg.ShowDialog();
        }
    }
}
