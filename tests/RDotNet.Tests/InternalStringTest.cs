using Xunit;

namespace RDotNet
{
    public class InternalStringTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            var engine = Engine;
            var vector = engine.Evaluate("c('foo', NA, 'bar')").AsCharacter();
            Assert.Equal(3, vector.Length);
            Assert.Equal("foo", vector[0]);
            Assert.Null(vector[1]);
            Assert.Equal("bar", vector[2]);
        }
    }
}
