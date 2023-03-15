using System.Collections.Generic;
using Pears.Inputs;

namespace Pears.Parsers;

public class DelimitedRepeatParser<TToken, TDelimiter, TResult> : Parser<TToken, IEnumerable<TResult>> {
    private readonly IParser<TToken, TResult> parser;
    private readonly IParser<TToken, TDelimiter> delimiterParser;
    private readonly int minimum;
    private readonly int maximum;

    public DelimitedRepeatParser(IParser<TToken, TResult> parser, IParser<TToken, TDelimiter> delimiterParser, int minimum, int maximum) {
        this.parser = parser;
        this.delimiterParser = delimiterParser;
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public override Maybe<IEnumerable<TResult>> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
        var results = new List<TResult>();

        var currentInput = finalInput = input;
        var i = 0;

        for (; i < minimum; i++) {
            if (i > 0)
            {
                var delimiter = delimiterParser.TryParse(currentInput, out currentInput);
                if (!delimiter.HasValue)
                {
                    return Maybe<IEnumerable<TResult>>.Empty;
                }
            }
            var result = parser.TryParse(currentInput, out finalInput);
            if (!result.HasValue) {
                return Maybe<IEnumerable<TResult>>.Empty;
            }
            results.Add(result.Value);
            currentInput = finalInput;
        }

        for (; i < maximum; i++)
        {
            var priorInput = currentInput;
            if (i > 0)
            {
                var delimiter = delimiterParser.TryParse(currentInput, out currentInput);
                if (!delimiter.HasValue)
                {
                    finalInput = priorInput;
                    return results.AsReadOnly();
                }
            }

            var result = parser.TryParse(currentInput, out finalInput);
            if (!result.HasValue) {
                finalInput = priorInput;
                return results.AsReadOnly();
            }
            results.Add(result.Value);
            currentInput = finalInput;
        }

        return results;
    }
}