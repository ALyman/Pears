using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Pears.Inputs;

namespace Pears.Sample.Calculator {
	[TestFixture]
	class Program {
		[Test]
		public static void Main(/*string[] args*/) {
			var tokenizer = Parse.Alternatives<char, string>(
					/*whitespace*/ Parse.Match<char>(char.IsWhiteSpace).OneOrMore().Select(string.Concat),
					/*number*/ Parse.Match<char>(char.IsDigit).OneOrMore().Select(string.Concat),
					Parse.Match('+').Select(ch => "+"),
					Parse.Match('-').Select(ch => "-"),
					Parse.Match('*').Select(ch => "*"),
					Parse.Match('/').Select(ch => "/"),
					Parse.Match('(').Select(ch => "("),
					Parse.Match(')').Select(ch => ")")
				);

			var number = Parse.Match<string>(s => s.All(char.IsDigit)).Select(double.Parse);
			var expr = Parse.Rule<string, Expression>();
			var sum = Parse.Rule<string, Expression>();
			var product = Parse.Rule<string, Expression>();
			var atom = Parse.Rule<string, Expression>();
			sum.Bind(Parse.Alternatives(
				from l in product
				from op in Parse.Match("+")
				from r in sum
				select (Expression)Expression.Add(l, r),

				from l in product
				from op in Parse.Match("-")
				from r in sum
				select (Expression)Expression.Subtract(l, r),

				product
			));
			product.Bind(Parse.Alternatives(
				from l in atom
				from op in Parse.Match("*")
				from r in product
				select (Expression)Expression.Multiply(l, r),

				from l in atom
				from op in Parse.Match("/")
				from r in product
				select (Expression)Expression.Divide(l, r),

				atom
			));
			atom.Bind(Parse.Alternatives(
				from lparen in Parse.Match("(")
				from e in expr
				from rparen in Parse.Match(")")
				select e,

				number.Select(v => (Expression)Expression.Constant(v))
			));
			expr.Bind(sum);

			var charStream = "0 * 1 + 2 + 3 + 4 + 5 + (6 - 7) * 8 / 9".AsInput();
			var tokenStream = Input.Tokenize(charStream, tokenizer).Where(s => !string.IsNullOrWhiteSpace(s));

			IInput<string> finalInput;
			var result = expr.TryParse(tokenStream, out finalInput);
			Trace.WriteLine(result.Value, "Expression");
		}
	}
}
