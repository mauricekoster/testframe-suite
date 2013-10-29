// -----------------------------------------------------------------------
// <copyright file="Globals.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace General.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Microsoft.VisualBasic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Globals
    {
        public const string AppName = "TestFrame Suite";
        public const string toolbar_version = "2013.1";

        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static string InputBox(string prompt, string defaultvalue="")
        {
            return Interaction.InputBox(prompt, Globals.AppName, defaultvalue, -1, -1);
        }
        public static bool YN(string question)
        {
            return ( MessageBox.Show(question,
                     Globals.AppName, MessageBoxButton.YesNo, 
                     MessageBoxImage.Question) == MessageBoxResult.Yes
                     );
        }
    }


}
