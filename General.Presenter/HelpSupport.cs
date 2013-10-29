


namespace General.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;
    using System.Diagnostics;

    public static class HelpSupport
    {

        public static string ShowWhatsNew
        {
            get { return RegistrySupport.SubKey("General").GetValue("Showed What's new", "").ToString();  }
            set { RegistrySupport.SubKey("General").SetValue("Showed What's new", value, RegistryValueKind.String); }
        }
        
        [DllImport("hhctrl.ocx", CharSet = CharSet.Unicode, EntryPoint = "HtmlHelpW")]
        private static extern int HtmlHelp(
           int caller,
           String file,
           uint command,
           String str
           );

        public static void ShowHelp(string topic = "", int caller = 0 )
        {
            Debug.Print("Show help");

            string filename;
            string helptopic = "";

            filename = GetHelpFilename();

            if (topic == "")
                helptopic = "welcome";
            else
                helptopic = topic;

            helptopic = "html/" + helptopic + ".html";

            HtmlHelp(caller , filename, 0, helptopic);
        }

        #region Private parts

        private static string GetHelpFilename()
        {
            return GetAssemblyPath() + "\\Documentation\\" + Globals.AppName + ".chm";
        }

        private static string GetAssemblyPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion
    }
}
