// -----------------------------------------------------------------------
// <copyright file="TestDesign.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TestDesign.Module
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using General.Presenter;
    using Cluster.Presenter;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestDesignActions
    {
        public static void SelectActionword()
        {
            if (!TestFrame.IsActiveTestdatasheet())
            {
                Globals.ShowMessage("No active Testcase sheet");
                return;
            }

            var dlg = new SelectActionword();
            dlg.ShowDialog();
        }
    }
}
