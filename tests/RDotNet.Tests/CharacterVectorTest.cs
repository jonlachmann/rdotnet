using Xunit;

namespace RDotNet
{
    public class CharacterVectorTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            var engine = Engine;
            var vector = engine.Evaluate("x <- c('foo', NA, 'bar')").AsCharacter();
            Assert.Equal(3, vector.Length);
            Assert.Equal("foo", vector[0]);
            Assert.Null(vector[1]);
            Assert.Equal("bar", vector[2]);
            vector[0] = null;
            Assert.Null(vector[0]);
            var logical = engine.Evaluate("is.na(x)").AsLogical();
            Assert.True(logical[0]);
            Assert.True(logical[1]);
            Assert.False(logical[2]);
        }

        [Fact]
        public void TestUnicodeCharacter()
        {
            SetUpTest();
            var engine = Engine;
            var vector = engine.Evaluate("x <- c('красавица Наталья', 'Un apôtre')").AsCharacter();
            var encoding = engine.Evaluate("Encoding(x)").AsCharacter();
            Assert.Equal("UTF-8", encoding[0]);
            Assert.Equal("UTF-8", encoding[1]);

            Assert.Equal(2, vector.Length);
            Assert.Equal("красавица Наталья", vector[0]);
            Assert.Equal("Un apôtre", vector[1]);
        }

        [Fact]
        public void TestDotnetToR()
        {
            SetUpTest();
            var engine = Engine;
            var vector = engine.Evaluate("x <- character(100)").AsCharacter();
            Assert.Equal(100, vector.Length);
            Assert.Equal("", vector[0]);
            vector[1] = "foo";
            vector[2] = "bar";
            var second = engine.Evaluate("x[2]").AsCharacter().ToArray();
            Assert.Single(second);
            Assert.Equal("foo", second[0]);
            Assert.Equal("foo", vector[1]);

            var third = engine.Evaluate("x[3]").AsCharacter().ToArray();
            Assert.Single(third);
            Assert.Equal("bar", third[0]);
            Assert.Equal("foo", vector[1]);
        }
    }
}
