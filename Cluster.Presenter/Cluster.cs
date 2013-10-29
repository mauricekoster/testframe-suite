namespace Cluster.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // NetOffice
    using NetOffice.ExcelApi.Enums;
    using Excel = NetOffice.ExcelApi;

    // Excel DNA
    using ExcelDna.Integration;
    
    using System.Diagnostics;

    using General.Presenter;
    using ActionWord.Presenter;

    public static class TestFrame
    {
        private static Excel.Application application;

        static TestFrame()
        {
            application = new Excel.Application(null, ExcelDnaUtil.Application);
            
        }

        public static bool IsActiveTestdatasheet()
        {
            var sh = (Excel.Worksheet)application.ActiveSheet;

            return true;
        }
        public static bool IsTestdataSheet(Excel.Worksheet sh)
        {
            Excel.Range rng = sh.UsedRange;
            Debug.WriteLine(rng.Rows.Count);
            for (int r = 1; r < rng.Rows.Count; r++)
            {
                var style = (Excel.Style)sh.Cells[r, 1].Style;
                var s = style.Name;

                if (s.Equals("actionword"))
                    return true;
            }

            return false;
        }
        public static string Language()
        {
            var shnm = new Dictionary<string, string>() {
                {"Clusterkaart", "NL"}, {"Cluster chard","EN"}, {"Clustercard","EN"}, {"Cluster chart","EN"},
            };

            var query = from nm in shnm
                        where ExcelSupport.SheetExists(nm.Key)
                        select nm.Value;

            if (query.Count() == 0) return "EN";
            return query.First();
        }

        private static string TestConditionFormula(string lang)
        {
            string dummy;
            switch (lang)
            {
                case "NL":
                    dummy = "conditie";
                    break;
                default:
                    dummy = "condition";
                    break;
            }
            return "=CONCATENATE(VLOOKUP(\"*ID\",C:D,2,FALSE),\"C\",COUNTIF(OFFSET(A$1,0,0,ROW(),1), \"" 
                + dummy + "\")*10)";
        }

        private static string TestCaseFormula(string lang)
        {
            var tc = TestConditionFormula(lang);
            return tc + " & \"T\" & (COUNTIF(OFFSET(B$1,0,0,ROW()-1,1)," + tc.Substring(2) + "& \"T*\") +1) * 10";
        }

        private static string GetPriorityRange(Excel.Worksheet sh, long columnnr)
        {
            long r;
            var ur = (Excel.Range)sh.UsedRange;

            for ( r = 2; r < ur.Rows.Count; r++)
            {
                Excel.Range rng = sh.Cells[r, columnnr];
                //return rng.Value.ToString();
                if (rng.Value == null || rng.Value.ToString().StartsWith("&") ) // .ToString().Equals("")
                    return "=" + sh.Cells[2, columnnr].Address + ":" + sh.Cells[ (r - 1), columnnr].Address;
            }
            return "";
        }

        private static void SetValidationRange(Excel.Worksheet sh, long currow, long columnnr)
        {
            try
            {
                //sh.Cells[currow, x].Value = GetPriorityRange(sh, x);
                sh.Cells[currow, columnnr].Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation,
                                                   null, GetPriorityRange(sh, columnnr));
            }
            catch (Exception)
            {

                sh.Cells[currow, columnnr].Validation.Modify(XlDVType.xlValidateList,
                                                      XlDVAlertStyle.xlValidAlertInformation,
                                                      null, GetPriorityRange(sh, columnnr));
            }	
        }
        
        private static void InsertNewTestThingy(string thingy)
        {
            var sh = (Excel.Worksheet)application.ActiveSheet;
            int currow = application.ActiveCell.Row;
            long maxcol = 8;

            Excel.Range rng = sh.UsedRange;

            string dummy = sh.Cells[1, 9].Value == null ? "" : sh.Cells[1, 9].Value.ToString();

            if ( dummy.Equals("Test Status", StringComparison.CurrentCultureIgnoreCase ) )
                maxcol = 9;
            
            if (!TestFrame.IsActiveTestdatasheet())
            {
                Globals.ShowMessage("No active Testcase sheet");

                return;
            }

            string tc = GetText(thingy);

            application.ScreenUpdating = false;
            var r = ExcelSupport.FindRow( sh, tc );
            sh.Cells[currow, 1].Value = tc;
            if (r == 0)
            {
                string f;
                switch (thingy)
                {
                    case "testcase":
                        f = TestCaseFormula(Language());
                        break;
                    case "testcondition":
                        f = TestConditionFormula(Language());
                        break;
                    default:
                        f = "=\"No formula!\"";
                        break;
                }
                sh.Cells[currow, 2].Formula = f;
            }
            else
            {
                sh.Cells[currow, 2].Formula = sh.Cells[r, 2].Formula; // r.Formula;
            }
            

            // merge description cells
            sh.Range(sh.Cells[currow, 3], sh.Cells[currow, 5]).Merge();
            sh.Cells[currow, 3].Value = "<<description>>";
            sh.Cells[currow, 3].Activate();

            sh.Range(sh.Cells[currow, 1], sh.Cells[currow, maxcol]).Style = thingy;
            
            if (thingy.Equals("testcondition") )
                {
                // insert combo boxes and validations
                for (var x = 6; x <= 8; x++)
                {
                    SetValidationRange( sh, currow, x );
                }
    
                sh.Cells[currow, 6].Value = "Must have";
                sh.Cells[currow, 7].Value = "Final";
                sh.Cells[currow, 8].Value = "Must test";
            }
            else
            {
                if (maxcol == 9)
                {
                    sh.Cells[currow, 8].FormulaR1C1 = "=IF(RC[1]<>\"\",\"Test status:\",\"\")";
                    sh.Cells[currow, 8].HorizontalAlignment = XlHAlign.xlHAlignRight;
                    SetValidationRange(sh, currow, 9);
                    sh.Cells[currow, 9].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                }
            }
            application.ScreenUpdating = true;
        }

        private static string GetText(string p)
        {
            var sh = (Excel.Worksheet)application.ActiveSheet;
            var ur = (Excel.Range)sh.UsedRange;

            for (long r = 1; r < ur.Rows.Count; r++)
            {
                Excel.Range rng = sh.Cells[r, 1];
                var s = (Excel.Style)rng.Style;
                
                if (s.Name.Equals(p))
                    return rng.Value.ToString();
            }

            return p;
        }

        public static void InsertNewTestCondition()
        {
            InsertNewTestThingy("testcondition");
        }

        public static void InsertNewTestCase()
        {
            InsertNewTestThingy("testcase");
        }

        public static void InsertNewTag()
        {
            var sh = (Excel.Worksheet)application.ActiveSheet;
            int currow = application.ActiveCell.Row;

            Excel.Range rng = sh.UsedRange;

            if (!TestFrame.IsTestdataSheet(sh))
            {
                Globals.ShowMessage("No active Testcase sheet");
                return;
            }

            application.ScreenUpdating = false;
           
            sh.Rows[currow].Insert();

            try
            {
                sh.Cells[currow, 2].Style = "tag";
                sh.Cells[currow, 3].Style = "tag-value";
            } 
            catch(Exception)
            {
                
            }
            sh.Cells[currow, 1].Value = "#tag";
            sh.Cells[currow, 1].Style = "actionword";
            sh.Cells[currow, 2].Activate();
            
            application.ScreenUpdating = true;
        }

        public static void InsertActionwordCurrentRow(Actionword aw)
        {
            var sh = (Excel.Worksheet)application.ActiveSheet;
            var cur = (Excel.Range)application.ActiveCell;
            long c;

            if (!IsTestdataSheet(sh)) return;

            long row = cur.Row;
            string cont_str = "&Cont";

            sh.Rows[row].Insert();
            sh.Rows[row].Insert();
            row++;
            sh.Cells[row-1, 1].Style = "actionword";
            sh.Cells[row, 1].Style = "actionword";

            sh.Cells[row, 1].Value = aw.Name;
            sh.Cells[row, 1].Activate();

            long maxnr = TFDatabase.Active.Settings.ArgumentsPerRow + 1;

            c = 2;
            foreach (var arg in aw.Arguments)
            {
                
                sh.Cells[row - 1, c].Value = arg.Name;
                sh.Cells[row - 1, c].Style = "argument";

                sh.Cells[row, c].Style = "testdata";
                sh.Cells[row, c].Value = arg.DefaultValue;

                c++;
                if (c == maxnr)
                {
                    sh.Cells[row, c].Value = cont_str;
                    sh.Cells[row, c].Style = "testdata";

                    c = 2;

                    row++;
                    sh.Rows[row].Insert();
                    sh.Cells[row, 1].Style = "actionword";
                    row++;
                    sh.Rows[row].Insert();
                    sh.Cells[row, 1].Style = "actionword";
                    sh.Cells[row, 1].Value = cont_str;
                }
            }
        }

       
        public static void CreateNewCluster(string template_filename)
        {
            application.Workbooks.Add(template_filename);
        }
    }
}
