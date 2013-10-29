using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Microsoft.Win32;

namespace ActionWord.Presenter.Tests
{
    /// <summary>
    /// Summary description for TFDBManager_Test
    /// </summary>
    [TestFixture]
    public class TFDBManager_Test
    {
        private RegistryKey __test_tf;
        private RegistryKey __databaseKey;
        //private const string registry_entrypoint = "Software\\VB and VBA Program Settings\\TestFrame Toolbar v2";
        private const string registry_entrypoint = "Software\\CGI\\TestFrame Suite";

        public TFDBManager_Test()
        {
            //
            // TODO: Add constructor logic here
            //
            __test_tf = Registry.CurrentUser.CreateSubKey(registry_entrypoint);
            __databaseKey = __test_tf.CreateSubKey("Database");
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void GetDatabase_returns_a_dictionary()
        {
            IDictionary<string, string> l = TFDBManager.GetDatabases();            
            Assert.IsInstanceOfType(
                    typeof(IDictionary<string, string>) ,
                    l
                );
        }

        [Test]
        public void AddDatabase_will_store_in_registry()
        {
            TFDBManager.AddDatabase("__unittest", @"e:\unittest.mdb");

            // get reg key
            Assert.IsTrue(__databaseKey.GetValueNames().Any(s => s.Equals("__unittest")));
        
            // remove regkey
            __databaseKey.DeleteValue("__unittest");
            
        }

        [Test]
        public void RemoveDatabase_will_remove_entry_from_registry()
        {
            // prepare
            TFDBManager.AddDatabase("__unittest", "C:\\unittest.mdb");

            // the action
            TFDBManager.RemoveDatabase("__unittest");
            
            // the check
            Assert.IsFalse(__databaseKey.GetValueNames().Any(s => s.Equals("__unittest")));

            // cleanup (if needed)
            if (__databaseKey.GetValueNames().Any(s => s.Equals("__unittest")))
                __databaseKey.DeleteValue("__unittest");
            
        }

        [Test]
        [ExpectedException(typeof(TFDBManager.DatabaseNotFoundException))]
        public void Current_while_not_exists()
        {
            TFDBManager.Current = "__notexisting";
        }

        [Test]
        public void Current_set_existing_will_work()
        {
            TFDBManager.AddDatabase("__unittest", "C:\\unittest.mdb");
            TFDBManager.Current = "__unittest";

            Assert.IsTrue(TFDBManager.Current.Equals("__unittest"));

            TFDBManager.RemoveDatabase("__unittest");
        }

        [Test]
        public void DatabaseFilename_get_current()
        {
            TFDBManager.AddDatabase("__unittest", "C:\\unittest.mdb");
            TFDBManager.Current = "__unittest";

            Assert.IsTrue(TFDBManager.DatabaseFilename.Equals("C:\\unittest.mdb"));

            TFDBManager.RemoveDatabase("__unittest");
        }

        [Test]
        public void GetDatabase_returns_all_dbs()
        {
            TFDBManager.AddDatabase("__unittest1", "C:\\unittest1.mdb");
            TFDBManager.AddDatabase("__unittest2", "C:\\unittest2.mdb");
            TFDBManager.Current = "__unittest1";

            var list = TFDBManager.GetDatabases();

            Assert.IsTrue(list.Keys.Any(k => k.Equals("__unittest1")));
            Assert.IsTrue(list.Keys.Any(k => k.Equals("__unittest2")));

            TFDBManager.RemoveDatabase("__unittest1");
            TFDBManager.RemoveDatabase("__unittest2");
        }


        [Test]
        public void RemoveDatabase_removing_current_will_unset_current()
        {
            TFDBManager.AddDatabase("__unittest1", "C:\\unittest1.mdb");
            TFDBManager.Current = "__unittest1";

            TFDBManager.RemoveDatabase("__unittest1");

            Assert.IsTrue(TFDBManager.Current == "");
        }

        [Test]
        public void NewDatabase_mdb_is_created()
        {
            TFDBManager.NewDatabase("__unittest", @"e:\tfdb\unittest.mdb");
            TFDBManager.NewDatabase("aap", @"e:\tfdb\aap.mdb");
        }
    }
}
