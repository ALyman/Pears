using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Pears.Inputs
{
    public class ListInput<T> : IInput<T>
    {
        private readonly IList<T> _text;
        private readonly int _position;
        private ListInput<T> _next;

        public ListInput(IList<T> text, int position = 0)
        {
            _text = text;
            _position = position;
        }

        public bool IsEndOfStream => _position >= _text.Count;
        public T Token
        {
            get
            {
                if (_position < _text.Count)
                    return _text[_position];
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

                return _next = new ListInput<T>(_text, _position + 1);
            }
        }
    }
}
