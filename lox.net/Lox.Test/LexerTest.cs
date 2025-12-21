using System.Text;
using FakeItEasy;
using Shouldly;

namespace Lox.Test;

public class LexerTest {
    public class LexerTestData : TheoryData<string, Token[]> {
        public LexerTestData() {
            #region TestCases

            this.Add(
            "print \"Hello, World!\";\n",
            new Token[] {
                new(TokenType.Print, "print", null, 1),
                new(TokenType.String, "\"Hello, World!\"", "Hello, World!", 1),
                new(TokenType.Semicolon, ";", null, 1),
                new(TokenType.Eof, string.Empty, null, 2)
            }
            );

            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(LexerTestData))]
    public void TestGetTokens(string source, Token[] expected) {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var tokens = Lexer.GetTokens(stream, A.Fake<IErrorContext>()).ToList();

        tokens.ShouldBe(expected);
    }
}
