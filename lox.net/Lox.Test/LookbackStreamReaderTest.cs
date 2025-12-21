using System.Text;
using Shouldly;

namespace Lox.Test;

public class LookbackStreamReaderTest {
    [Fact]
    public void Read() {
        const string expected = "hello world";
        using var reader = new LookbackStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(expected)));

        var result = new StringBuilder();
        var read = reader.Read();
        while (read != -1) {
            result.Append((char)read);
            read = reader.Read();
        }

        result.ToString().ShouldBe(expected);
    }

    [Fact]
    public void Peek() {
        const string str = "abcd";
        using var reader = new LookbackStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(str)));

        // peek
        var peek = reader.Peek();
        peek.ShouldBe('a');

        // peek should return the same value
        peek = reader.Peek();
        peek.ShouldBe('a');

        // read should also return that value
        var read = reader.Read();
        read.ShouldBe('a');

        // read will have advanced the cursor
        peek = reader.Peek();
        peek.ShouldBe('b');
    }

    [Fact]
    public void PeekNext() {
        const string str = "abcdef";
        using var reader = new LookbackStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(str)));

        // peek
        var peek = reader.Peek();
        peek.ShouldBe('a');

        // peek next
        peek = reader.PeekNext();
        peek.ShouldBe('b');

        // peek should return the same as before
        peek = reader.PeekNext();
        peek.ShouldBe('b');

        // read should return the first character
        var read = reader.Read();
        read.ShouldBe('a');
    }

    [Fact]
    public void EndOfStream() {
        const string expected = "hello world";
        using var reader = new LookbackStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(expected)));

        var result = new StringBuilder();
        while (!reader.EndOfStream) {
            var read = reader.Read();
            result.Append((char)read);
        }

        result.ToString().ShouldBe(expected);
    }
}
