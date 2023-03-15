using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Pears.Inputs
{
    public class StringInput : IInput<char>
    {
        private readonly string _text;
        private readonly int _position;
        private StringInput _next;

        public StringInput(string text, int position = 0)
        {
            _text = text;
            _position = position;
        }

        public bool IsEndOfStream => _position >= _text.Length;
        public char Token
        {
            get
            {
                if (_position < _text.Length)
                    return _text[_position];
                throw new EndOfStreamException();
            }
        }


        public IInput<char> Next
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

                return _next = new StringInput(_text, _position + 1);
            }
        }

        IInput IInput.Next => this.Next;
    }
}
