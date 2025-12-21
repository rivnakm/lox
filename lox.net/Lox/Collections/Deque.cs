using System.Collections;

namespace Lox.Collections;

public class Deque<T> : IEnumerable<T> {
    // TODO: test perf of list vs doubly linked list vs array with head/tail pointers
    private readonly List<T> _items;

    public Deque() : this(new List<T>()) { }

    public Deque(IEnumerable<T> items) {
        this._items = new List<T>(items);
    }

    public int Capacity => this._items.Capacity;
    public int Count => this._items.Count;

    public void Push(T item) {
        this._items.Add(item);
    }

    public T? Pop() {
        if (this._items.Count == 0) {
            return default;
        }

        var item = this._items[0];
        this._items.RemoveAt(0);
        return item;
    }

    public void Unpop(T item) {
        this._items.Insert(0, item);
    }

    public bool TryPop(out T item) {
        if (this._items.Count == 0) {
            item = default!;
            return false;
        }

        item = this._items[0];
        this._items.RemoveAt(0);
        return true;
    }

    public T? Peek() {
        return this._items.Count == 0 ? default : this._items[0];
    }

    public bool TryPeek(out T item) {
        if (this._items.Count == 0) {
            item = default!;
            return false;
        }

        item = this._items[0];
        return true;
    }

    public IEnumerator<T> GetEnumerator() {
        return this._items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}
