namespace ActionwordManagement.Module
{
    using ActionWord.Presenter;
    using ActionWordManagement.Module;
    using System;

    public static class ActionwordManagementActions
    {
        public static void ShowManageActionWords()
        {
            var dlg = new ManageActionwords_TopLevel();
            dlg.ShowDialog();
        }

        internal static Nullable<bool> ShowEditActionword(Actionword aw)
        {
            var dlg = new EditActionword();
            dlg.CurrentActionword = aw;
            return dlg.ShowDialog();
        }
    }

}
