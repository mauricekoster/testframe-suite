// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="CGI">
// Copyrights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Configuration.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using ActionWord.Presenter;
    using General.Presenter;

    /// <summary>
    /// Wrapper for easy access to setiings in the database settings table.
    /// </summary>
    public static class Settings
    {
        public static string GetTemplatePath()
        {
            var path = TFDatabase.Active.Settings.TemplateDirectory;
            if (path == null || path.Equals(""))
            {

                path = RegistrySupport.GetRegistrySetting("General", "TemplatePath", @"C:\TestFrame\Template");
                // this will be the Excel executable. :(
                // path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
            return path;
        }
    }
}
