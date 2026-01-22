using FakeItEasy;
using Lox.Exceptions;
using Lox.Utility;
using Shouldly;

namespace Lox.Test.Utility;

public class InterpreterValueUtilityTest {
    [Theory]
    [InlineData(null, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(0.0, true)]
    [InlineData(1.0, true)]
    [InlineData("abc", true)]
    [InlineData("", true)]
    public void IsTruthy(object? value, bool expected) {
        InterpreterValueUtility.IsTruthy(value).ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, null, true)]
    [InlineData(true, null, false)]
    [InlineData(null, true, false)]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, false)]
    [InlineData(1, "1", false)]
    [InlineData("abc", "abc", true)]
    [InlineData("abc", "xyz", false)]
    public void AreEqual(object? left, object? right, bool expected) {
        InterpreterValueUtility.AreEqual(left, right).ShouldBe(expected);
    }

    [Fact]
    public void CheckNumber_Valid() {
        var check = () => InterpreterValueUtility.CheckNumber(A.Dummy<Token>(), 1.0);
        check.ShouldNotThrow();
    }

    [Fact]
    public void CheckNumber_Invalid() {
        var check = () => InterpreterValueUtility.CheckNumber(A.Dummy<Token>(), "1.0");
        check.ShouldThrow<InvalidTypeException>();
    }

    [Fact]
    public void CheckNumbers_Valid() {
        var check = () => InterpreterValueUtility.CheckNumbers(A.Dummy<Token>(), 1.0, 2.0);
        check.ShouldNotThrow();
    }

    [Theory]
    [InlineData(1.0, "2.0")]
    [InlineData("1.0", 2.0)]
    [InlineData("1.0", "2.0")]
    public void CheckNumbers_Invalid(object? left, object? right) {
        var check = () => InterpreterValueUtility.CheckNumbers(A.Dummy<Token>(), left, right);
        check.ShouldThrow<InvalidTypeException>();
    }

    [Theory]
    [InlineData(null, "nil")]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Format(object? value, string expected) {
        InterpreterValueUtility.Format(value).ShouldBe(expected);
    }
}
