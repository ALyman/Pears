using System;

using Pears.Inputs;

namespace Pears.Parsers
{
    public class AlternativeParser<TToken, T1, T2, TResult> : Parser<TToken, TResult>
    {
        private readonly IParser<TToken, T1> first;
        private readonly Func<T1, TResult> firstSelector;
        private readonly IParser<TToken, T2> second;
        private readonly Func<T2, TResult> secondSelector;

        public AlternativeParser(IParser<TToken, T1> first, Func<T1, TResult> firstSelector, IParser<TToken, T2> second, Func<T2, TResult> secondSelector)
        {
            this.first = first;
            this.firstSelector = firstSelector;
            this.second = second;
            this.secondSelector = secondSelector;
        }

        public override Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput)
        {
            Maybe<T1> firstResult;
            Maybe<T2> secondResult;

            if ((firstResult = first.TryParse(input, out finalInput)).HasValue)
            {
                return firstSelector(firstResult);
            }

            if ((secondResult = second.TryParse(input, out finalInput)).HasValue)
            {
                return secondSelector(secondResult);
            }

            return Maybe<TResult>.Empty;
        }
    }
}