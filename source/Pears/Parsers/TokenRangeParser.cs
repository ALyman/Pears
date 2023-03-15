using System;
using System.Collections.Generic;
using System.Linq;
using Pears.Inputs;

namespace Pears.Parsers;

public class TokenRangeParser<TToken, TResult> : IParser<TToken, IEnumerable<TToken>> {
    private IParser<TToken, TResult> parser;

    public TokenRangeParser(IParser<TToken, TResult> parser)
    {
        this.parser = parser;
    }

    public Maybe<IEnumerable<TToken>> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
        var result = parser.TryParse(input, out finalInput);

        if (result.HasValue)
        {
            var fin = finalInput;
            return new Maybe<IEnumerable<TToken>>(
                input.AsInputEnumerable().TakeWhile(i => i != fin)
                .Select(i => i.Token));
        }
        else
        {
            return Maybe<IEnumerable<TToken>>.Empty;
        }
    }
}