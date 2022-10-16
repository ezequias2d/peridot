using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

namespace Peridot;

public class Batcher<TTexture> where TTexture : notnull
{
    private TTexture[] _textures;
    private BatchItem[] _items;
    private int _count;
    private Slice[] _slices;
    private int _slicesCount;

    public Batcher()
    {
        _slices = new Slice[8];
        _textures = new TTexture[8];
        _items = new BatchItem[8];
        _count = 0;
        _slicesCount = 0;
    }

    public ref BatchItem Add(TTexture texture)
    {
        if (_count >= _items.Length)
        {
            var lastSize = _items.Length;
            var newSize = (lastSize + lastSize / 2 + 7) & (~7);
            Array.Resize(ref _items, newSize);
            Array.Resize(ref _textures, newSize);
        }

        var i = _count++;
        _textures[i] = texture;
        return ref _items[i];
    }

    public void Clear()
    {
        Array.Clear(_items);
        Array.Clear(_textures);
        Array.Clear(_slices);

        _count = 0;
        _slicesCount = 0;
    }

    public void Build(SortMode mode)
    {
        if (_count == 0)
            return;

        switch (mode)
        {
            case SortMode.FrontToBack:
                Array.Sort(_items, _textures, 0, _count, FrontToBackComparer.Instance);
                break;
            case SortMode.BackToFront:
                Array.Sort(_items, _textures, 0, _count, BackToFrontComparer.Instance);
                break;
        }

        TTexture currentTexture = _textures[0];

        int index = 0;
        int start = 0;
        _slicesCount = 0;
        do
        {
            TTexture texture = _textures[index];
            if (!currentTexture.Equals(texture))
            {
                if (_slicesCount >= _slices.Length)
                {
                    var lastSize = _slices.Length;
                    var newSize = (lastSize + lastSize / 2 + 7) & (~7);
                    Array.Resize(ref _slices, newSize);
                }
                _slices[_slicesCount++] = new(start, index - start);
                Debug.Assert(index - start >= 0);
                start = index;
                currentTexture = texture;
            }
            index++;
        }
        while (index < _count);

        if (_slicesCount >= _slices.Length)
        {
            var lastSize = _slices.Length;
            var newSize = (lastSize + lastSize / 2 + 7) & (~7);
            Array.Resize(ref _slices, newSize);
        }
        _slices[_slicesCount++] = new(start, index - start);
    }

    public ReadOnlySpan<BatchItem> Items => new ReadOnlySpan<BatchItem>(_items, 0, _count);
    public ReadOnlySpan<Slice> Slices => new ReadOnlySpan<Slice>(_slices, 0, _slicesCount);
    public Enumerator GetEnumerator() => new Enumerator(_textures, _slices, _slicesCount);
    public TTexture GetSliceTexture(Slice slice)
    {
        return _textures[slice.Start];
    }

    private class FrontToBackComparer : IComparer<BatchItem>
    {
        public readonly static IComparer<BatchItem> Instance = new FrontToBackComparer();
        private FrontToBackComparer() { }

        public int Compare(BatchItem x, BatchItem y)
        {
            return x.Location.Z.CompareTo(y.Location.Z);
        }
    }

    private class BackToFrontComparer : IComparer<BatchItem>
    {
        public readonly static IComparer<BatchItem> Instance = new BackToFrontComparer();
        private BackToFrontComparer() { }
        public int Compare(BatchItem x, BatchItem y)
        {
            return y.Location.Z.CompareTo(x.Location.Z);
        }
    }

    public struct Slice : IEquatable<Slice>
    {
        public Slice(int start, int length)
        {
            Start = start;
            Length = length;
        }
        public readonly int Start;
        public readonly int Length;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, Length);
        }

        public bool Equals(Batcher<TTexture>.Slice other)
        {
            return Start == other.Start &&
                Length == other.Length;
        }
    }

    public struct Enumerator
    {
        private readonly TTexture[] _textures;
        private Slice[] _slices;
        private int _slicesCount;
        private int _index;

        internal Enumerator(TTexture[] textures, Slice[] slices, int count)
        {
            _textures = textures;
            _slices = slices;
            _slicesCount = count;
            _index = -1;
        }

        public (TTexture Key, Slice Slice) Current
        {
            get
            {
                var slice = _slices[_index];
                var texture = _textures[slice.Start];
                return (texture, slice);
            }
        }

        public bool MoveNext()
        {
            if (_index >= _slicesCount)
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
