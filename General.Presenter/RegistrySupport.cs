// -----------------------------------------------------------------------
// <copyright file="RegistrySupport.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Win32;

namespace General.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interaction with Registry. These are support functions.
    /// </summary>
    public static class RegistrySupport
    {
        private static RegistryKey __test_tf;
        private static RegistryKey __databaseKey;
        private static RegistryKey __generalKey;

        //TODO CGI rebranding
        private const string registry_entrypoint = "Software\\CGI\\TestFrame Suite";
        //private const string registry_entrypoint = "Software\\VB and VBA Program Settings\\TestFrame Toolbar v2";

        static RegistrySupport()
        {
            __test_tf = Registry.CurrentUser.CreateSubKey(registry_entrypoint);
            __generalKey = __test_tf.CreateSubKey("General");
            __databaseKey = __test_tf.CreateSubKey("Database");
        }

        static public string EntryPoint {
            get { return registry_entrypoint; } 
        }

        static public RegistryKey SubKey(string keyname)
        {
            return __test_tf.CreateSubKey(keyname);
        }
        static public string GetRegistrySetting(string keyname, string valuename, string defaultvalue="")
        {
            return SubKey(keyname).GetValue(valuename, defaultvalue).ToString();
        }
    }
}
