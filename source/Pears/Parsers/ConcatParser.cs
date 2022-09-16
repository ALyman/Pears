using System;

using Pears.Inputs;

namespace Pears.Parsers {
	public class ConcatParser<TToken, T1, T2, TResult> : Parser<TToken, TResult> {
		private readonly IParser<TToken, T1> first;
		private readonly IParser<TToken, T2> second;
		private readonly Func<T1, T2, TResult> selector;

		public ConcatParser(IParser<TToken, T1> first, IParser<TToken, T2> second, Func<T1, T2, TResult> selector) {
			this.first = first;
			this.second = second;
			this.selector = selector;
		}

		public override Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			Maybe<T1> firstResult;
			Maybe<T2> secondResult;
			finalInput = input;

			if ((firstResult = first.TryParse(finalInput, out finalInput)).HasValue) {
				if ((secondResult = second.TryParse(finalInput, out finalInput)).HasValue) {
					return selector(firstResult, secondResult);
				}
			}

			finalInput = input;
			return Maybe<TResult>.Empty;
		}
    }
}