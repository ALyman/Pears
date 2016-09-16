using System;

using Pears.Inputs;

namespace Pears.Parsers {
	public class RuleParser<TToken, TResult> : IParser<TToken, TResult> {
		private IParser<TToken, TResult> boundParser;

		public void Bind(IParser<TToken, TResult> parser) {
			if (boundParser != null && !object.ReferenceEquals(boundParser, parser)) {
				throw new InvalidOperationException("RuleParser already bound");
			}

			boundParser = parser;
		}

		public Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			if (boundParser == null) {
				throw new InvalidOperationException("RuleParser not bound");
			}

			return boundParser.TryParse(input, out finalInput);
		}
	}
}