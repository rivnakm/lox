using Lox.Collections;
using Shouldly;

namespace Lox.Test.Collections;

public class DequeTest {
    [Fact]
    public void Push() {
        var expected = new[] { 1, 2, 3 };
        var deque = new Deque<int>();
        deque.Push(1);
        deque.Push(2);
        deque.Push(3);

        deque.ToList().ShouldBe(expected);
    }

    [Fact]
    public void Unpop() {
        var expected = new[] { 3, 2, 1 };
        var deque = new Deque<int>();
        deque.Unpop(1);
        deque.Unpop(2);
        deque.Unpop(3);

        deque.ToList().ShouldBe(expected);
    }

    [Fact]
    public void Peek() {
        var items = new[] { 1, 2, 3 };
        var deque = new Deque<int>(items);

        var peek = deque.Peek();
        peek.ShouldBe(1);
        deque.Count.ShouldBe(3);
    }

    [Fact]
    public void Peek_Empty() {
        var deque = new Deque<int>();

        var peek = deque.Peek();
        peek.ShouldBe(default);
        deque.Count.ShouldBe(0);
    }

    [Fact]
    public void Pop() {
        var items = new[] { 1, 2, 3 };
        var deque = new Deque<int>(items);

        var peek = deque.Pop();
        peek.ShouldBe(1);
        deque.Count.ShouldBe(2);
    }

    [Fact]
    public void Pop_Empty() {
        var deque = new Deque<int>();

        var peek = deque.Pop();
        peek.ShouldBe(default);
        deque.Count.ShouldBe(0);
    }

    [Fact]
    public void TryPeek() {
        var items = new[] { 1, 2, 3 };
        var deque = new Deque<int>(items);

        var success = deque.TryPeek(out var peek);
        success.ShouldBe(true);
        peek.ShouldBe(1);
        deque.Count.ShouldBe(3);
    }

    [Fact]
    public void TryPeek_Empty() {
        var deque = new Deque<int>();

        var success = deque.TryPeek(out _);
        success.ShouldBe(false);
        deque.Count.ShouldBe(0);
    }

    [Fact]
    public void TryPop() {
        var items = new[] { 1, 2, 3 };
        var deque = new Deque<int>(items);

        var success = deque.TryPop(out var peek);
        success.ShouldBe(true);
        peek.ShouldBe(1);
        deque.Count.ShouldBe(2);
    }

    [Fact]
    public void TryPop_Empty() {
        var deque = new Deque<int>();

        var success = deque.TryPop(out _);
        success.ShouldBe(false);
        deque.Count.ShouldBe(0);
    }
}
