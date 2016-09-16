using System;
using System.Collections.Generic;

using Pears.Inputs;
using Pears.Parsers;

namespace Pears {
	public static class Input {
		public static IInput<TToken> AsInput<TToken>(this IEnumerable<TToken> source) {
			return new EnumerableInput<TToken>(source);
		}

		public static IEnumerable<TToken> AsEnumerable<TToken>(this IInput<TToken> source) {
			while (!source.IsEndOfStream) {
				yield return source.Token;
				source = source.Next;
			}
		}

		public static IInput<TResult> Tokenize<TSource, TResult>(IInput<TSource> input, IParser<TSource, TResult> tokenizer) {
			return new TokenizerInput<TSource, TResult>(input, tokenizer);
		}

		public static IInput<TToken> Where<TToken>(this IInput<TToken> source, Func<TToken, bool> match) {
			return new FilterInput<TToken>(source, match);
		}
	}
}