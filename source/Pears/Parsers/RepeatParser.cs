using System;
using System.Collections.Generic;

using Pears.Inputs;

namespace Pears.Parsers {
	public class RepeatParser<TToken, TResult> : Parser<TToken, IEnumerable<TResult>> {
		private IParser<TToken, TResult> parser;
		private int minimum;
		private int maximum;

		public RepeatParser(IParser<TToken, TResult> parser, int minimum, int maximum) {
			this.parser = parser;
			this.minimum = minimum;
			this.maximum = maximum;
		}

		public override Maybe<IEnumerable<TResult>> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			var results = new List<TResult>();

			var currentInput = finalInput = input;
			var i = 0;

			for (; i < minimum; i++) {
				var result = parser.TryParse(currentInput, out finalInput);
				if (!result.HasValue) {
					return Maybe<IEnumerable<TResult>>.Empty;
				}
				results.Add(result.Value);
				currentInput = finalInput;
			}

			for (; i < maximum; i++) {
				var result = parser.TryParse(currentInput, out finalInput);
				if (!result.HasValue) {
					finalInput = currentInput;
					return results.AsReadOnly();
				}
				results.Add(result.Value);
				currentInput = finalInput;
			}

			return results;
		}
    }
}
