using System.Text;

namespace Lox.Test;

public class LoxTest {
    [Fact]
    public void Test() {
        var executor = new LoxExecutor();
        const string program = """
var nan = 0/0;

print nan == 0; // expect: false
print nan != 1; // expect: true

// NaN is not equal to self.
print nan == nan; // expect: false
print nan != nan; // expect: true
""";

        executor.Exec(CreateStream(program), false);
    }

    private static Stream CreateStream(string text) {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
