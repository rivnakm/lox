using Lox.Expressions;
using Shouldly;

namespace Lox.Test.Expressions;

public class ExpressionHashTest {
    [Fact]
    public void TestExpressionHash() {
        var dict = new Dictionary<IExpression, string>();

        // Two distinct objects should return the same hash code if they have the same values
        var expr1 = new VariableExpression(new Token(TokenType.Identifier, "a", "one", 0));
        var expr2 = new VariableExpression(new Token(TokenType.Identifier, "a", "one", 0));

        dict[expr1] = "expr1";
        dict[expr2] = "expr2";

        var actual = dict[expr1];
        var expected = "expr2";

        actual.ShouldBe(expected);
    }
}
