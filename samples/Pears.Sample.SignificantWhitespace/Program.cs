using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pears;

namespace Pears.Sample.SignificantWhitespace
{
    [TestFixture]
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
        }

        [Test]
        public static void Run()
        {
            var charStream = "if a\n\ta()\nelse\n\tb()\n\t c()\nd()".AsInput();

            var tokenizer = Parse.Alternatives(
                /*whitespace*/ Parse.Match<char>(char.IsWhiteSpace).OneOrMore().Select(string.Concat),
                /*number*/ Parse.Match<char>(char.IsDigit).OneOrMore().Select(string.Concat),
                Parse.Match<char>(char.IsLetter).SelectMany(initial => Parse.Match<char>(char.IsLetterOrDigit).ZeroOrMore().Select(string.Concat), (initial, rest) => string.Concat(initial, rest)),
                Parse.Match('+').Select(ch => "+"),
                Parse.Match('-').Select(ch => "-"),
                Parse.Match('*').Select(ch => "*"),
                Parse.Match('/').Select(ch => "/"),
                Parse.Match('(').Select(ch => "("),
                Parse.Match(')').Select(ch => ")")
            );


            var trackedChars = charStream
                .Scan(
                    new { Line = 0, Column = 0, InWhitespace = true, InitialWhitespace = "", Char = Maybe<char>.Empty },
                    (context, ch) =>
                    {
                        if (!context.Char.HasValue)
                        {
                            return new
                            {
                                Line = 1,
                                Column = 0,
                                InWhitespace = true,
                                InitialWhitespace = "",
                                Char = (Maybe<char>)ch
                            };
                        }
                        else if (context.Char == '\n')
                        {
                            return new
                            {
                                Line = context.Line + 1,
                                Column = 0,
                                InWhitespace = true,
                                InitialWhitespace = "",
                                Char = (Maybe<char>)ch
                            };
                        }
                        else if (char.IsWhiteSpace(context.Char) && context.InWhitespace)
                        {
                            return new
                            {
                                context.Line,
                                Column = context.Column + 1,
                                InWhitespace = char.IsWhiteSpace(ch),
                                InitialWhitespace = "" + context.Char.Value,
                                Char = (Maybe<char>)ch
                            };
                        }
                        else
                        {
                            return new
                            {
                                context.Line,
                                Column = context.Column + 1,
                                InWhitespace = true,
                                context.InitialWhitespace,
                                Char = (Maybe<char>)ch
                            };
                        }
                    },
                    (context) => new { context.Line, context.Column, context.InitialWhitespace, Char = context.Char.Value });

            var all = trackedChars.AsEnumerable().ToArray();
        }
    }
}
