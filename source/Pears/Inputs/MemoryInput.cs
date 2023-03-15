#if NET5_0_OR_GREATER

using System;
using System.IO;

namespace Pears.Inputs;

public class MemoryInput<T> : IInput<T>
{
    private readonly Memory<T> _text;
    private readonly int _position;
    private MemoryInput<T> _next;

    public MemoryInput(Memory<T> text, int position = 0)
    {
        _text = text;
        _position = position;
    }

    public bool IsEndOfStream => _position >= _text.Length;
    public T Token
    {
        get
        {
            if (_position < _text.Length)
                return _text.Span[_position];
            throw new EndOfStreamException();
        }
    }

    public IInput<T> Next
    {
        get
        {
            if (_next != null)
            {
                return _next;
            }

            if (this.IsEndOfStream)
            {
                return this;
            }

            return _next = new MemoryInput<T>(_text, _position + 1);
        }
    }

    IInput IInput.Next => this.Next;
}

#endif