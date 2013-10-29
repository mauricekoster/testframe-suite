
namespace Configuration.Module
{

    using ActionWord.Presenter;
    using System.IO;
    using General.Presenter;

    public class ConfigurationActions 
    {
        static ConfigurationActions()
        {
        }
        
        public static void ShowDatabaseManager()
        {
            var dlg = new DatabaseManager();

            if (TFDatabase.IsActive)
            {
                TFDatabase.Active.Close();
            }

            dlg.ShowDialog();

            TFDatabase.Open();
        }
        
        public static void ShowGeneralOptions()
        {
            var dlg = new GeneralOptions();
            dlg.ShowDialog();
        }

       
    }
}
