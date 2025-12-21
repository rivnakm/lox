using Lox.Extensions;
using Shouldly;

namespace Lox.Test.Extensions;

public class StringExtensionsTest {
    [Theory]
    [InlineData("", 'a', 1, "")]
    [InlineData("aba", 'a', 1, "b")]
    [InlineData("aabaa", 'a', 1, "aba")]
    [InlineData("aabaa", 'a', 2, "b")]
    [InlineData("aaaaaa", 'a', 2, "aa")]
    [InlineData("\"Hello, World!\"", '"', 1, "Hello, World!")]
    public void TrimExact(string str, char trimChar, int count, string expected) {
        var actual = str.TrimExact(trimChar, count);

        actual.ShouldBe(expected);
    }

    [Fact]
    public void TrimExact_NullString_Throws() {
        var exec = () => _ = StringExtensions.TrimExact(null!, 'c', 1);
        exec.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TrimExact_CountOutOfRange_Throws(int count) {
        var exec = () => _ = "abc".TrimExact('c', count);
        exec.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
