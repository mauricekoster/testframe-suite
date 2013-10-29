
namespace Cluster.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    // NetOffice
    using NetOffice;
    using Excel = NetOffice.ExcelApi;
    using ExcelConst = NetOffice.ExcelApi.Enums;

    // Excel DNA
    using ExcelDna.Integration;
    using ExcelDna.Integration.CustomUI;
    using System.Diagnostics;

    public class ExcelSupport
    {
        private static Excel.Application application;

        static ExcelSupport()
        {
            application = new Excel.Application(null, ExcelDnaUtil.Application);
            
        }

        public static bool SheetExists(string p)
        {
            var wb = (Excel.Workbook)application.ActiveWorkbook;

            for (int i = 1; i < wb.Sheets.Count; i++)
            {
                var sh = (Excel.Worksheet)wb.Sheets[i];
                if (sh.Name == p) return true;
            }
            return false;
        }

        public static long FindRow(Excel.Worksheet sh, string searchtext, long columntosearch=1)
        {
            Excel.Range ur = (Excel.Range)sh.UsedRange;

            for (long r = 1; r < ur.Rows.Count; r++)
            {
                Excel.Range rng = sh.Cells[r, columntosearch];
                var v = rng.Value;
                if (v == null) continue;
                if (v.Equals(searchtext) )
                    return r;
            }

            return 0;
        }
    }
}
