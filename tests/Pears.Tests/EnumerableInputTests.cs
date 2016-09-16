using NUnit.Framework;
using System.IO;
using System.Linq;

using Pears.Inputs;

namespace Pears.Tests {
	[TestFixture]
	public class EnumerableInputTests {

		[Test]
		public void ScanForward() {
			var expected = Enumerable.Range(0, 10);
			var input = expected.AsInput();
			Assert.AreEqual(expected, input.AsEnumerable());
			Assert.AreEqual(expected.Skip(2), input.Next.Next.AsEnumerable());
			IInput<int> a, b;

			for (a = input, b = input; !a.IsEndOfStream && !b.IsEndOfStream; a = a.Next, b = b.Next) {
				Assert.AreSame(a, b);
				Assert.AreEqual(a.Token, b.Token);
			}
		}

		[Test]
		public void EndOfStream() {
			var expected = Enumerable.Empty<int>();
			var input = expected.AsInput();
			Assert.AreEqual(expected, input.AsEnumerable());

			Assert.Throws<EndOfStreamException>(() => { var token = input.Token; });
			Assert.AreSame(input, input.Next);
		}
	}
}