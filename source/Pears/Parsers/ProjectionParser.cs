using System;

using Pears.Inputs;

namespace Pears.Parsers {
	public class ProjectionParser<TToken, TSource, TResult> : IParser<TToken, TResult> {
		private readonly IParser<TToken, TSource> parser;
		private readonly Func<TSource, TResult> selector;

		public ProjectionParser(IParser<TToken, TSource> parser, Func<TSource, TResult> selector) {
			this.parser = parser;
			this.selector = selector;
		}

		public Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			var result = parser.TryParse(input, out finalInput);
			if (result.HasValue) {
				return selector(result.Value);
			} else {
				return Maybe<TResult>.Empty;
			}
		}
    }
}