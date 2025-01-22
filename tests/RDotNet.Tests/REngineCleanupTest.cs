using Xunit;
using System.Linq;

namespace RDotNet
{
    public class REngineCleanupTest : RDotNetTestFixture
    {
        [Fact]
        public void TestDefaultClearGlobalEnv()
        {
            SetUpTest();
            var engine = Engine;
            engine.ClearGlobalEnvironment();
            var s = engine.Evaluate("ls()").AsCharacter().ToArray();
            Assert.True(s.Length == 0);
        }

        [Fact]
        public void TestDetachPackagesDefault()
        {
            SetUpTest();
            var engine = Engine;

            var s = engine.Evaluate("search()").AsCharacter().ToArray();
            Assert.DoesNotContain("package:lattice", s);
            Assert.DoesNotContain("package:Matrix", s);
            Assert.DoesNotContain("package:MASS", s);
            Assert.DoesNotContain("biopsy", s);

            engine.ClearGlobalEnvironment();
            engine.Evaluate("library(lattice)");
            engine.Evaluate("library(Matrix)");
            engine.Evaluate("library(MASS)");
            engine.Evaluate("data(biopsy, package='MASS')");
            engine.Evaluate("attach(biopsy)");
            s = engine.Evaluate("search()").AsCharacter().ToArray();

            Assert.Contains("package:lattice", s);
            Assert.Contains("package:Matrix", s);
            Assert.Contains("package:MASS", s);
            Assert.Contains("biopsy", s);

            engine.ClearGlobalEnvironment(detachPackages: true);

            s = engine.Evaluate("search()").AsCharacter().ToArray();
            Assert.DoesNotContain("package:lattice", s);
            Assert.DoesNotContain("package:Matrix", s);
            Assert.DoesNotContain("package:MASS", s);
            Assert.DoesNotContain("biopsy", s);
        }
    }
}
