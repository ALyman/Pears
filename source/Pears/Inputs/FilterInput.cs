using System;

namespace Pears.Inputs {
	public class FilterInput<TToken> : IInput<TToken> {
		private IInput<TToken> startInput;
		private IInput<TToken> finalInput;
		private readonly Func<TToken, bool> match;
		private IInput<TToken> next;

		public FilterInput(IInput<TToken> startInput, Func<TToken, bool> match) {
			this.startInput = startInput;
			this.match = match;
		}

		public bool IsEndOfStream {
			get {
				EnsureRead();
				return finalInput.IsEndOfStream;
			}
		}

		private void EnsureRead() {
			if (finalInput == null) {
				finalInput = startInput;
				while (!finalInput.IsEndOfStream && !match(finalInput.Token)) {
					finalInput = finalInput.Next;
				}
			}
		}

		public TToken Token {
			get {
				EnsureRead();
				return finalInput.Token;
			}
		}

		public IInput<TToken> Next {
			get {
				if (next != null) {
					return next;
				} else {
					EnsureRead();
					if (finalInput.IsEndOfStream) {
						return this;
					} else {
						return next = new FilterInput<TToken>(finalInput.Next, match);
					}
				}
			}
		}
	}
}