using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using ActionWord.Presenter;

namespace ActionWord.Presenter.Tests
{
    [TestFixture]
    public class TFDatabaseTest
    {
        [Test]
        public void TestMethod1()
        {
            string oldcur = TFDBManager.Current;
            TFDBManager.NewDatabase("unittest", @"e:\unittest.mdb");
            TFDBManager.Current = "unittest";

            TFDatabase.Open();

            Assert.IsTrue(TFDatabase.Active.Settings["database version"] == "2009.07");
            Assert.IsTrue(TFDatabase.Active.Settings["xxx"] == "");

            TFDatabase.Active.Settings["xxx"] = "Test me";
            Assert.IsTrue(TFDatabase.Active.Settings["xxx"] == "Test me");

            TFDatabase.Active.Settings["xxx"] = "Something else";
            Assert.IsTrue(TFDatabase.Active.Settings["xxx"] == "Something else");

            TFDBManager.Current = oldcur;
            TFDBManager.RemoveDatabase("unittest");
        }
    }
}
