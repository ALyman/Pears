using System;
using System.Collections.Generic;
using System.IO;

namespace Pears
{
    public class EnumeratorInputSource<TToken> : IInputSource<TToken>
    {
        private static int count = 0;

        private int index;
        private readonly IEnumerator<TToken> enumerator;
        private bool haveRead = false;
        private bool endOfInput;
        private TToken token;
        private EnumeratorInputSource<TToken> next;

        public EnumeratorInputSource(IEnumerator<TToken> enumerator)
        {
            this.index = count++;
            this.enumerator = enumerator;
        }

        private void EnsureRead()
        {
            if (haveRead) return;
            haveRead = true;
            if (enumerator.MoveNext()) {
                token = enumerator.Current;
            } else {
                endOfInput = true;
            }
        }

        public TToken Value
        {
            get
            {
                EnsureRead();
                if (endOfInput) throw new EndOfStreamException();
                return token;
            }
        }

        public IInputSource<TToken> Next
        {
            get
            {
                EnsureRead();
                if (endOfInput)
                {
                    return this;
                }
                if (next == null)
                {
                    next = new EnumeratorInputSource<TToken>(enumerator);
                }
                return next;
            }
        }

        public bool EndOfInput
        {
            get
            {
                EnsureRead();
                return endOfInput;
            }
        }
    }
}