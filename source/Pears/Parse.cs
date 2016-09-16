using System;
using System.Linq;

using Pears.Parsers;
using System.Collections.Generic;

namespace Pears {
	public static class Parse {
		public static IParser<TToken, TToken> Match<TToken>(TToken token) {
			return Match<TToken>(token, EqualityComparer<TToken>.Default);
		}

		public static IParser<TToken, TToken> Match<TToken>(TToken token, IEqualityComparer<TToken> comparer) {
			return Match<TToken, TToken>(token, comparer, result => result);
		}

		public static IParser<TToken, TToken> Match<TToken>(Func<TToken, bool> match) {
			return Match<TToken, TToken>(match, result => result);
		}

		public static IParser<TToken, TResult> Match<TToken, TResult>(TToken token, Func<TToken, TResult> selector) {
			return Match<TToken, TResult>(token, EqualityComparer<TToken>.Default, selector);
		}

		public static IParser<TToken, TResult> Match<TToken, TResult>(TToken token, IEqualityComparer<TToken> comparer, Func<TToken, TResult> selector) {
			return Match<TToken, TResult>(match => comparer.Equals(token, match), selector);
		}

		public static IParser<TToken, TResult> Match<TToken, TResult>(Func<TToken, bool> match, Func<TToken, TResult> selector) {
			return new MatchParser<TToken, TResult>(match, selector);
		}

		public static IParser<TToken, TResult> Concat<TToken, T1, T2, TResult>(Func<T1, T2, TResult> selector, IParser<TToken, T1> first, IParser<TToken, T2> second) {
			return new ConcatParser<TToken, T1, T2, TResult>(first, second, selector);
		}

		public static IParser<TToken, IEnumerable<TResult>> Concat<TToken, TResult>(params IParser<TToken, TResult>[] parsers) {
			return parsers.Aggregate(
				Parse.Constant<TToken, IEnumerable<TResult>>(Enumerable.Empty<TResult>()),
				(first, second) => Concat((priorResults, secondResult) => priorResults.Concat(new[] { secondResult }), first, second)
			);
		}

		private static IParser<TToken, TResult> Constant<TToken, TResult>(TResult result) {
			return new ConstantParser<TToken, TResult>(result);
		}

		public static IParser<TToken, TResult> Alternatives<TToken, T1, T2, TResult>(Func<T1, TResult> firstSelector, IParser<TToken, T1> first, Func<T2, TResult> secondSelector, IParser<TToken, T2> second) {
			return new AlternativeParser<TToken, T1, T2, TResult>(first, firstSelector, second, secondSelector);
		}

		public static IParser<TToken, TResult> Alternatives<TToken, TResult>(params IParser<TToken, TResult>[] parsers) {
			return parsers.Aggregate((first, second) => new AlternativeParser<TToken, TResult, TResult, TResult>(first, _ => _, second, _ => _));
		}

		public static IParser<TToken, TResult> SelectMany<TToken, T1, T2, TResult>(this IParser<TToken, T1> first, Func<T1, IParser<TToken, T2>> second, Func<T1, T2, TResult> selector) {
			return Concat(selector, first, second(default(T1)));
		}

		public static IParser<TToken, IEnumerable<TResult>> Repeat<TToken, TResult>(this IParser<TToken, TResult> parser, int minimum = 0, int maximum = int.MaxValue) {
			return new RepeatParser<TToken, TResult>(parser, minimum, maximum);
		}

		public static IParser<TToken, IEnumerable<TResult>> ZeroOrMore<TToken, TResult>(this IParser<TToken, TResult> parser) {
			return Repeat(parser, minimum: 0, maximum: int.MaxValue);
		}

		public static IParser<TToken, IEnumerable<TResult>> OneOrMore<TToken, TResult>(this IParser<TToken, TResult> parser) {
			return Repeat(parser, minimum: 1, maximum: int.MaxValue);
		}

		public static RuleParser<TToken, TResult> Rule<TToken, TResult>() {
			return new RuleParser<TToken, TResult>();
		}

		public static IParser<TToken, TResult> Select<TToken, TSource, TResult>(this IParser<TToken, TSource> parser, Func<TSource, TResult> selector) {
			return new ProjectionParser<TToken, TSource, TResult>(parser, selector);
		}
	}
}