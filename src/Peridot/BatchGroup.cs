// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

namespace Peridot;

public class BatchGroup
{
    private BatchItem[] _items;

    public BatchGroup()
    {
        _items = new BatchItem[64];
    }
    public int Count { get; private set; }

    public ref BatchItem Add()
    {
        if (Count >= _items.Length)
        {
            var lastSize = _items.Length;
            var newSize = (lastSize + lastSize / 2 + 63) & (~63);
            Array.Resize(ref _items, newSize);
        }

        return ref _items[Count++];
    }

    public void Clear()
    {
        Count = 0;
    }

    public ReadOnlySpan<BatchItem> GetSpan() => new ReadOnlySpan<BatchItem>(_items, 0, Count);

    public Enumerator GetEnumerator() => new(_items, Count);

    public struct Enumerator
    {
        private readonly BatchItem[] _items;
        private readonly int _count;
        private int _index;

        internal Enumerator(BatchItem[] items, int count)
        {
            _items = items;
            _count = count;
            _index = -1;
        }

        public BatchItem Current => _items[_index];

        public bool MoveNext()
        {
            if (_index >= _count)
                return false;
            _index++;
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
