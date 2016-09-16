using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using Pears.Inputs;

namespace Pears.Tests {
	[TestFixture]
	public class LinqParsingTests {
		[Test]
		public void SuccessfulSelectMany() {
			var input = new MockInput<int>(3);

			var parser = from a in MockParser.Create(1, input)
						 from b in MockParser.Create(2, input.Next)
						 from c in MockParser.Create(3, input.Next.Next)
						 select new { a, b, c };

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(new { a = 1, b = 2, c = 3 }));
			Assert.That(finalInput, Is.SameAs(input.Next.Next.Next));
		}

		[Test]
		public void FailedSelectMany() {
			var input = new MockInput<int>(3);

			var parser = from a in MockParser.Create(1, input)
						 from b in MockParser.Create(2, input.Next)
						 from c in MockParser.Create(Maybe<int>.Empty, input.Next.Next)
						 select new { a, b, c };

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.False);
			Assert.That(finalInput, Is.SameAs(input));
		}
	}
}
