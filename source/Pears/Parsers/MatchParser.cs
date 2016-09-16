using System;

using Pears.Inputs;

namespace Pears.Parsers {
	public class MatchParser<TToken, TResult> : Parser<TToken, TResult> {
		private readonly Func<TToken, bool> match;
		private readonly Func<TToken, TResult> selector;

		public MatchParser(Func<TToken, bool> match, Func<TToken, TResult> selector) {
			this.match = match;
			this.selector = selector;
		}

		public override Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			if (!input.IsEndOfStream && match(input.Token)) {
				finalInput = input.Next;
				return selector(input.Token);
			} else {
				finalInput = input;
				return Maybe<TResult>.Empty;
			}
		}
	}
}