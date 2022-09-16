using System;

namespace Pears.Inputs {
	public class WhereInput<TToken> : IInput<TToken>
	{
		private readonly IInput<TToken> startInput;
		private IInput<TToken> finalInput;
		private readonly Func<TToken, bool> predicate;
	    private IInput<TToken> next;

		public WhereInput(IInput<TToken> startInput, Func<TToken, bool> predicate) {
            while (!startInput.IsEndOfStream && !predicate(startInput.Token))
            {
                startInput = startInput.Next;
            }

            this.startInput = startInput;
			this.predicate = predicate;
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
				while (!finalInput.IsEndOfStream && !predicate(finalInput.Token)) {
                    finalInput = finalInput.Next;
				}
			}
		}

		public TToken Token {
			get {
				EnsureRead();
				return startInput.Token;
			}
		}

		public IInput<TToken> Next {
			get {
				if (next != null) {
					return next;
				}

                EnsureRead();

			    if (finalInput.IsEndOfStream) {
			        return this;
			    }

                return next = new WhereInput<TToken>(finalInput.Next, predicate);
			}
		}
	}
}