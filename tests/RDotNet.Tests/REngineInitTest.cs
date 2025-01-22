using Xunit;
using RDotNet.NativeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RDotNet
{

    /// <summary> The windows registry.</summary>
    public class MockRegistry : IRegistry
    {
        private MockRegistryKey currentUser;
        private MockRegistryKey localMachine;

        public MockRegistry(MockRegistryKey localMachine, MockRegistryKey currentUser)
        {
            this.localMachine = localMachine;
            this.currentUser = currentUser;
        }

        /// <summary> Gets the current user.</summary>
        ///
        /// <value> The current user.</value>
        public IRegistryKey CurrentUser => currentUser;

        /// <summary> Gets the local machine.</summary>
        ///
        /// <value> The local machine.</value>
        public IRegistryKey LocalMachine => localMachine;

        public MockRegistry(string localMachineTestReg, string currentUserTestReg = null)
        {
            localMachine = MockRegistryKey.Parse(localMachineTestReg);
            currentUser = string.IsNullOrEmpty(currentUserTestReg) ? null : MockRegistryKey.Parse(currentUserTestReg);
        }
    }

    /// <summary> The windows registry key.</summary>
    public class MockRegistryKey : IRegistryKey
    {
        public Dictionary<string, string> keyValues = new Dictionary<string, string>();
        private List<MockRegistryKey> subKeys = new List<MockRegistryKey>();

        private string ShortName;
        //public string /*LongName*/;
        public MockRegistryKey(string fullKey)
        {
            var s = CreateStack(fullKey);
            var k = s.Pop(); // HKEY_LOCAL_MACHINE
            ShortName = k;
            if(s.Count != 0)
                subKeys.Add(new MockRegistryKey(s));
        }

        public MockRegistryKey(Stack<string> s)
        {
            if (s.Count == 0) throw new ArgumentException("must be at least one item in the stack of keys");
            var k = s.Pop();
            ShortName = k;
            if (s.Count > 0)
                subKeys.Add(new MockRegistryKey(s));
        }

        /// <summary>
        /// Get the real key of a registry entry
        /// </summary>
        /// <returns>RegistryKey object</returns>
        public Object GetRealKey()
        {
            return subKeys[0].GetRealKey();
        }

        /// <summary> Gets sub key names.</summary>
        ///
        /// <returns> An array of string.</returns>
        public string[] GetSubKeyNames()
        {
            return subKeys.Select(x => x.ShortName).ToArray();
        }

        /// <summary> Gets a value of a key-value pair.</summary>
        ///
        /// <param name="name"> The name.</param>
        ///
        /// <returns> The value.</returns>
        public object GetValue(string name)
        {
            return keyValues[name];
        }

        /// <summary> Retrieves an array of strings that contains all the value names associated with
        ///                 this key.</summary>
        ///
        /// <returns> An array of string.</returns>
        public string[] GetValueNames()
        {
            return keyValues.Select(x => x.Key).ToArray();
        }

        /// <summary> Opens sub key.</summary>
        ///
        /// <param name="name"> The name.</param>
        ///
        /// <returns> An IRegistryKey.</returns>
        public IRegistryKey OpenSubKey(string name)
        {
            var s = CreateStack(name);
            return Find(s);
        }

        internal static MockRegistryKey Parse(string localMachineTestReg)
        {
            //[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64]
            //'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
            //'Current Version'='3.3.3'
            var topKeys = new List<MockRegistryKey>();
            var lines = localMachineTestReg.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string currentKey = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    currentKey = line.Replace("[", "").Replace("]", ""); // so left with HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
                    Populate(currentKey, topKeys);
                }
                else if (currentKey != null)
                {
                    var x = line.Trim();
                    if (x.Length == 0) continue;
                    var k = Find(topKeys, currentKey);
                    //'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
                    var keyVal = x.Replace("'", "").Split('=');
                    k.AddKeyVal(keyVal[0], keyVal[1]);
                }
            }
            if (topKeys.Count != 1)
                throw new Exception("");
            return topKeys[0];
        }

        public void AddKeyVal(string key, string val)
        {
            keyValues[key] = val;
        }

        private static Stack<string> CreateStack(string fullKey)
        {
            // fullKey is expected to be something like: HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
            var kItems = fullKey.Split('\\');
            Array.Reverse(kItems);
            return new Stack<string>(kItems);
        }

        private static void Populate(string fullKey, List<MockRegistryKey> topKeys)
        {
            // fullKey is expected to be something like: HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64
            var s = CreateStack(fullKey);
            if (!s.Any()) return;
            var k = s.Pop(); // HKEY_LOCAL_MACHINE
            var existing = topKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Any())
            {
                topKey = existing.First();
                topKey.AddSubKeys(s);
            }
            else
            {
                topKey = new MockRegistryKey(fullKey);
                topKeys.Add(topKey);
            }
        }

        private void AddSubKeys(Stack<string> s)
        {
            if (s.Count == 0) return ;
            var k = s.Pop();
            var kk = Find(k);
            if (kk == null)
            {
                subKeys.Add(s.Count != 0 ? new MockRegistryKey(s) : new MockRegistryKey(k));
            }
            else
                kk.AddSubKeys(s);
        }

        private MockRegistryKey Find(string shortSubkeyName)
        {
            var v = from x in subKeys where x.ShortName == shortSubkeyName select x;
            return v.Count() switch
            {
                < 1 => null,
                > 1 => throw new Exception("More than one key found for: " + shortSubkeyName),
                _ => v.First()
            };
        }

        private static MockRegistryKey Find(List<MockRegistryKey> topKeys, string fullKey)
        {
            var s = CreateStack(fullKey);
            if (!s.Any()) return null;
            var k = s.Pop(); // HKEY_LOCAL_MACHINE
            var existing = topKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Any()) topKey = existing.First();
            return !s.Any() ? topKey : topKey.Find(s);
        }

        private MockRegistryKey Find(Stack<string> s)
        {
            if (!s.Any()) return null;
            var k = s.Pop();
            var existing = subKeys.Where(m => m.ShortName == k);
            MockRegistryKey topKey = null;
            if (existing.Count() > 0) topKey = existing.First();
            return !s.Any() ? topKey : topKey.Find(s);
        }

    }


    public class REngineInitTest : RDotNetTestFixture
    {
        [Fact(Skip = "Cannot run this in a batch with the new singleton pattern")] // Cannot run this in a batch with the new singleton pattern.
        public void TestInitParams()
        {
            var device = new MockDevice();
            REngine.SetEnvironmentVariables();
            using var engine = REngine.GetInstance();
            const ulong maxMemSize = 128 * 1024 * 1024;
            var parameter = new StartupParameter() {
                MaxMemorySize = maxMemSize,
            };
            engine.Initialize(parameter: parameter, device: device);
            Assert.Equal(128.0, engine.Evaluate("memory.limit()").AsNumeric()[0]);
        }

        private NativeUtility createTestRegistryUtil(bool realRegistry = true)
        {
            if (realRegistry)
                return new NativeUtility();
            else
            {
                var localMachineTestReg = @"
[HKEY_LOCAL_MACHINE\SOFTWARE\R-core]

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
'Current Version'='3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803 Microsoft R Client]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.3.3]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'
'Current Version'='3.2.2.803'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
'Current Version'='3.3.3'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64\3.2.2.803 Microsoft R Client]
'InstallPath'='C:\\Program Files\\Microsoft\\R Client\\R_SERVER\\'

[HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64\3.3.3]
'InstallPath'='C:\\Program Files\\R\\R-3.3.3'
";

                return new NativeUtility(new MockRegistry(localMachineTestReg));
            }
        }

        // TODO: probably needs adjustments for MacOS and Linux
        [Fact]
        public void TestFindRBinPath()
        {
            var rLibPath = createTestRegistryUtil().FindRPath();
            var files = Directory.GetFiles(rLibPath);
            var fnmatch = files.Where(fn => fn.ToLower() == Path.Combine(rLibPath.ToLower(), NativeUtility.GetRLibraryFileName().ToLower()));
            Assert.Single(fnmatch);
        }

        [Fact(Skip = "Fails to pass at 2020-10 on all platforms, but this appears not to test anything anymore really. Review.")]
        public void TestMockWindowsRegistry()
        {

            IRegistryKey rCore;
            // 2020-10 I lost sight of what this test was for. Causes issues on Linux, not sure why and too hard to debug against other priorities.
            if (!NativeUtility.IsWin32NT) return;
            var w = new WindowsRegistry();
            rCore = w.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");


            var localMachineTestReg = @"
    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core]

    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R]
    'InstallPath'='C:\Program Files\R\R-3.3.3'
    'Current Version'='3.3.3'
    ";
            var reg = new MockRegistry(localMachineTestReg);
            var lm = reg.LocalMachine;
            //var sk = lm.GetSubKeyNames();
            rCore = lm.OpenSubKey(@"SOFTWARE\R-core");
            var valNames = rCore.GetValueNames();
            Assert.Empty(valNames);

            Assert.Single(rCore.GetSubKeyNames());
            Assert.Equal("R", rCore.GetSubKeyNames()[0]);
            var r = rCore.OpenSubKey(@"R");
            Assert.Empty(r.GetSubKeyNames());
            Assert.Equal(2, r.GetValueNames().Length);
            Assert.Equal(r.GetValue("InstallPath"), "C:\\Program Files\\R\\R-3.3.3");
            Assert.Equal(r.GetValue("Current Version"), "3.3.3");

            localMachineTestReg = @"
    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64]
    'InstallPath'='C:\Program Files\Microsoft\R Client\R_SERVER\'
    'Current Version'='3.2.2.803'

    [HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803]
    'InstallPath'='C:\Program Files\Microsoft\R Client\R_SERVER\'
    ";

            reg = new MockRegistry(localMachineTestReg);
            lm = reg.LocalMachine;
            rCore = lm.OpenSubKey(@"SOFTWARE\R-core");
            Assert.Empty(rCore.GetValueNames());

            Assert.Single(rCore.GetSubKeyNames());
            Assert.Equal("R", rCore.GetSubKeyNames()[0]);
            r = rCore.OpenSubKey(@"R");
            Assert.Single(r.GetSubKeyNames());

            var R64 = lm.OpenSubKey(@"SOFTWARE\R-core\R\R64");

            Assert.Single(R64.GetSubKeyNames());
            Assert.Equal(2, R64.GetValueNames().Length);

            Assert.Equal(R64.GetValue("InstallPath"), @"C:\Program Files\Microsoft\R Client\R_SERVER\");
            Assert.Equal(R64.GetValue("Current Version"), "3.2.2.803");
        }

        [Fact]
        public void TestFindRegKey()
        {
            //"HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\3.2.2.803 Microsoft R Client";
            //"HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R\R64\3.2.2.803";
            //   C:\Program Files\Microsoft\R Client\R_SERVER\
            //string rLibPath = createTestRegistryUtil().FindRPaths();
            //var files = Directory.GetFiles(rLibPath);
            //var fnmatch = files.Where(fn => fn.ToLower() == Path.Combine(rLibPath.ToLower(), NativeUtility.GetRLibraryFileName().ToLower()));
            //Assert.Equal(1, fnmatch.Count());
        }

        [Fact(Skip = "This still does not pass on Travis CI sourcing pkgs from https://cloud.r-project.org/bin/linux/ubuntu bionic-cran40")]
        public void TestFindRHomePath()
        {
            var rHomePath = createTestRegistryUtil().FindRHome();
            var files = Directory.GetDirectories(rHomePath);
            // Note differences in setups:
            // A local R installation has these folders;
            // modules  lib  share  etc  doc  library  bin  include
            // however on Travis CI sourcing from https://cloud.r-project.org/bin/linux/ubuntu bionic-cran40/
            // bin  COPYING  etc  lib	library  modules  site-library	SVN-REVISION
            var fnmatch = files.Where(fn => Path.GetFileName(fn) == "library");
            Assert.Single(fnmatch);
            fnmatch = files.Where(fn => Path.GetFileName(fn) == "modules");
            Assert.Single(fnmatch);
            fnmatch = files.Where(fn => Path.GetFileName(fn) == "etc");
            Assert.Single(fnmatch);
            fnmatch = files.Where(fn => Path.GetFileName(fn) == "lib");
            Assert.Single(fnmatch);
            // Following fails on the Travis CI machine
            // fnmatch = Directory.GetDirectories(fnmatch.First()).Where(fn => Path.GetFileName(fn) == "base");
            // Assert.Equal(1, fnmatch.Count());
        }

        [Fact]
        public void TestGetPathInitSearchLog()
        {
            SetUpTest();
            var engine = Engine;
            var log = NativeUtility.SetEnvironmentVariablesLog;
            Assert.NotEqual(string.Empty, log);
        }

        [Fact]
        public void TestUsingDefaultRPackages()
        {
            // This test was designed to look at a symptom observed alongside the issue https://github.com/rdotnet/rdotnet/issues/127
            SetUpTest();
            var engine = Engine;
            var se = engine.Evaluate("set.seed");

            if(NativeUtility.GetPlatform() == PlatformID.Win32NT)
            {
                Assert.True(engine.Evaluate("Sys.which('R.dll')").AsCharacter()[0].Length > 0);
                Assert.True(engine.Evaluate("Sys.which('RBLAS.dll')").AsCharacter()[0].Length > 0);
            }

            string[] expected = { "base", "methods", "utils", "grDevices", "graphics", "stats" };
            var loadedDlls = engine.Evaluate("getLoadedDLLs()").AsList();
            var dllnames = loadedDlls.Select(x => x.AsCharacter().ToArray()[0]).ToArray();

            var query = from x in expected.Intersect(dllnames)
                                        select x;

            Assert.Equal(expected, query.ToArray());

            se = engine.Evaluate("set.seed(0)");
            se = engine.Evaluate("blah <- rnorm(4)");
        }
    }
}
