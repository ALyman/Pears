using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pears.Inputs;
using Pears.Parsers;

namespace Pears
{
    public static class Input
    {
        public static IEnumerable<TToken> AsEnumerable<TToken>(this IInput<TToken> source)
        {
            while (!source.IsEndOfStream)
            {
                yield return source.Token;
                source = source.Next;
            }
        }

        public static IInput<TToken> AsInput<TToken>(this IEnumerable<TToken> source)
        {
            return new EnumerableInput<TToken>(source);
        }

        public static IInput<TToken> Concat<TToken>(this IInput<TToken> first, params IInput<TToken>[] rest)
        {
            return Concat(new[] { first }.Concat(rest));
        }

        public static IInput<TToken> Concat<TToken>(IEnumerable<IInput<TToken>> inputs)
        {
            return inputs.SelectMany(input => input.AsEnumerable()).AsInput();
        }

        public static IInput<TAccumulate> Scan<TSource, TAccumulate>(this IInput<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return source.Select(item => seed = func(seed, item));
        }

        public static IInput<TResult> Scan<TSource, TAccumulate, TResult>(this IInput<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return source.Select(item => seed = func(seed, item)).Select(resultSelector);
        }

        public static IInput<TResult> Select<TSource, TResult>(this IInput<TSource> source, Func<TSource, TResult> selector)
        {
            return new SelectInput<TSource, TResult>(source, selector);
        }

        public static IInput<TResult> SelectMany<TSource, TResult>(this IInput<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return source.AsEnumerable().SelectMany(selector).AsInput();
        }

        public static IInput<TResult> Tokenize<TSource, TResult>(IInput<TSource> input, IParser<TSource, TResult> tokenizer)
        {
            return new TokenizerInput<TSource, TResult>(input, tokenizer);
        }

        public static IInput<TToken> Where<TToken>(this IInput<TToken> source, Func<TToken, bool> predicate)
        {
            return new WhereInput<TToken>(source, predicate);
        }
    }
}