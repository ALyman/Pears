using Pears.Inputs;
using System;

namespace Pears.Parsers {
	internal class ConstantParser<TToken, TResult> : Parser<TToken, TResult> {
		private readonly TResult result;

		public ConstantParser(TResult result) {
			this.result = result;
		}

		public override Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			finalInput = input;
			return result;
		}
    }
}