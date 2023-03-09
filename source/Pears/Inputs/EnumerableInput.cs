using System.Collections.Generic;
using System.IO;

namespace Pears.Inputs {
	public sealed class EnumerableInput<TToken> : IInput<TToken> {
		private readonly IEnumerator<TToken> enumerator;
		private bool haveRead, endOfStream;
		private TToken token;
		private EnumerableInput<TToken> next;

		public EnumerableInput(IEnumerable<TToken> source)
			: this(source.GetEnumerator()) {
		}

		private EnumerableInput(IEnumerator<TToken> enumerator) {
			this.enumerator = enumerator;
		}

		public bool IsEndOfStream {
			get {
				EnsureRead();
				return endOfStream;
			}
		}

		public TToken Token {
			get {
				EnsureRead();
				if (endOfStream) {
					throw new EndOfStreamException();
				}
				return token;
			}
		}

		public IInput<TToken> Next {
			get
            {
                if (next != null) {
					return next;
				}

                EnsureRead();
                if (endOfStream) {
                    return this;
                }

                return next = new EnumerableInput<TToken>(enumerator);
            }
		}

		private void EnsureRead() {
			if (!haveRead) {
				if (enumerator.MoveNext()) {
					endOfStream = false;
					token = enumerator.Current;
				} else {
					endOfStream = true;
				}
				haveRead = true;
			}
		}
	}
}