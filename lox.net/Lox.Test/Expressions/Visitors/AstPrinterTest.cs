using Lox.Expressions;
using Shouldly;

namespace Lox.Test.Expressions.Visitors;

public class AstPrinterTest {

    [Theory]
    [ClassData(typeof(AstPrinterTestData))]
    public void TestPrint(IExpression expression, string expected) {
        var printer = new AstPrinter();
        var result = printer.Print(expression);

        result.ShouldBe(expected);
    }

    public class AstPrinterTestData : TheoryData<IExpression, string> {
        public AstPrinterTestData() {
            #region TestCases

            this.Add(new LiteralExpression("hello"), "hello");
            this.Add(new LiteralExpression(1234), "1234");
            this.Add(new LiteralExpression(null), "nil");
            this.Add(new UnaryExpression(new Token(TokenType.Minus, "-", null, 1), new LiteralExpression(123)),
                     "(- 123)");
            this.Add(new BinaryExpression(new LiteralExpression(1),
                                          new Token(TokenType.Plus, "+", null, 1),
                                          new LiteralExpression(2)),
                     "(+ 1 2)");
            this.Add(new GroupingExpression(new BinaryExpression(new LiteralExpression(1),
                                                                 new Token(TokenType.Plus, "+", null, 1),
                                                                 new LiteralExpression(2))),
                     "(group (+ 1 2))");

            #endregion
        }
    }
}
