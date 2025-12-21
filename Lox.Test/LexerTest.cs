using System.Text;
using FakeItEasy;
using Shouldly;

namespace Lox.Test;

public class LexerTest {
    public static IEnumerable<object?[]> LexerTestCases() {
        #region TestCases

        yield return new object[] {
            "print \"Hello, World!\";\n", new List<Token> {
                new(TokenType.Print, "print", null, 1),
                new(TokenType.String, "\"Hello, World!\"", "Hello, World!", 1),
                new(TokenType.Semicolon, ";", null, 1),
                new(TokenType.Eof, string.Empty, null, 2)
            }
        };

        #endregion
    }

    [Theory]
    [MemberData(nameof(LexerTestCases))]
    public void TestGetTokens(string source, List<Token> expected) {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var tokens = Lexer.GetTokens(stream, A.Fake<IErrorContext>()).ToList();

        tokens.ShouldBe(expected);
    }

}
