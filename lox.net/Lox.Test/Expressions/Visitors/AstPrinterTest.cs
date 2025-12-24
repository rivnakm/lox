using Lox.Expressions;
using Lox.Expressions.Visitors;
using Shouldly;

namespace Lox.Test.Expressions.Visitors;

public class AstPrinterTest
{
    public class AstPrinterTestData : TheoryData<Expression, string>
    {
        public AstPrinterTestData()
        {
            #region TestCases

            this.Add(new Literal("hello"), "hello");
            this.Add(new Literal(1234), "1234");
            this.Add(new Literal(null), "nil");
            this.Add(new UnaryExpression(new Token(TokenType.Minus, "-", null, 1), new Literal(123)), "(- 123)");
            this.Add(new BinaryExpression(new Literal(1), new Token(TokenType.Plus, "+", null, 1), new Literal(2)),
                     "(+ 1 2)");
            this.Add(new Grouping(new BinaryExpression(new Literal(1),
                                                       new Token(TokenType.Plus, "+", null, 1),
                                                       new Literal(2))),
                     "(group (+ 1 2))");

            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(AstPrinterTestData))]
    public void TestPrint(Expression expression, string expected)
    {
        var printer = new AstPrinter();
        var result = printer.Print(expression);

        result.ShouldBe(expected);
    }
}
