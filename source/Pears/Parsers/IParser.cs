using System;

using Pears.Inputs;

namespace Pears.Parsers {
	public interface IParser<TToken, TResult> {
		Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput);
	}

	public abstract class Parser<TToken, TResult> : IParser<TToken, TResult> {
		public abstract Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput);
	}
}