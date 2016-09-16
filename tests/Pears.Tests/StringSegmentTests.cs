using NUnit.Framework;
using System;
using System.Linq;

namespace Pears.Tests {
	[TestFixture]
	public class StringSegmentTests {

		[Test]
		[TestCase("a", 0, 1, "a")]
		[TestCase("abc", 0, 3, "abc")]
		[TestCase("abc", 0, 1, "a")]
		[TestCase("abc", 2, 1, "c")]
		[TestCase("abc", 0, 2, "ab")]
		[TestCase("abc", 1, 2, "bc")]
		[TestCase("abc", 1, 1, "b")]
		[TestCase(null, 0, 0, null)]
		public void EqualityTests(string source, int startIndex, int length, string expected) {
			var segment = new StringSegment(source, startIndex, length);
			Assert.AreEqual(expected, (string)segment);
			Assert.AreEqual(expected, segment.ToString());
			Assert.IsTrue(segment.Equals(expected));
		}

		[Test]
		[TestCase("a", 0, 1, "b")]
		[TestCase("abc", 0, 3, "xyz")]
		[TestCase("abc", 0, 1, "c")]
		[TestCase("abc", 2, 1, "a")]
		[TestCase("abc", 0, 2, "bc")]
		[TestCase("abc", 1, 2, "ab")]
		[TestCase("abc", 1, 1, "x")]
		[TestCase("abc", 0, 1, "abc")]
		[TestCase("abc", 1, 1, "abc")]
		[TestCase("abc", 2, 1, "abc")]
		public void InequalityTests(string source, int startIndex, int length, string expected) {
			var segment = new StringSegment(source, startIndex, length);
			Assert.AreNotEqual(expected, (string)segment);
			Assert.AreNotEqual(expected, segment.ToString());
			Assert.IsFalse(segment.Equals(expected));
		}

		[Test]
		[TestCase("abc", -1, 1, "startIndex")]
		[TestCase("abc", 0, 4, "length")]
		[TestCase("abc", 1, 3, "length")]
		[TestCase("abc", 1, -1, "length")]
		[TestCase("abc", 2, 2, "length")]
		[TestCase(null, 0, 2, "length")]
		[TestCase(null, 1, 0, "startIndex")]
		public void InvalidConstructorArguments(string source, int startIndex, int length, string paramName) {
			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new StringSegment(source, startIndex, length));
			Assert.AreEqual(paramName, ex.ParamName);
		}

		[Test]
		[TestCase("abc", 0, 3, "b", "a", "c")]
		[TestCase("abc", 0, 3, "ab", "", "", "c")]
		[TestCase("abc", 0, 3, "ac", "", "b", "")]
		[TestCase("xyzabcdef", 3, 3, "ab", "", "", "c")]
		[TestCase("xyzabcdef", 3, 3, "n", "abc")]
		[TestCase("", 0, 0, "n", "")]
		public void SplitByChars(string source, int startIndex, int length, string seps, params string[] expected) {
			var segment = new StringSegment(source, startIndex, length);
			var actual = segment.Split(seps.ToCharArray(), StringSplitOptions.None).ToArray();
			Assert.AreEqual(expected, actual.Select(s => (string)s).ToArray());
		}

		[Test]
		[TestCase("abc", 0, 3, "b", "a", "c")]
		[TestCase("abc", 0, 3, "ab", "c")]
		[TestCase("abc", 0, 3, "ac", "b")]
		[TestCase("xyzabcdef", 3, 3, "ab", "c")]
		[TestCase("xyzabcdef", 3, 3, "b", "a", "c")]
		[TestCase("xyzabcdef", 3, 3, "n", "abc")]
		[TestCase("", 0, 0, "n")]
		public void SplitByCharsWithNoEmpties(string source, int startIndex, int length, string seps, params string[] expected) {
			var segment = new StringSegment(source, startIndex, length);
			var actual = segment.Split(seps.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
			Assert.AreEqual(expected, actual.Select(s => (string)s).ToArray());
		}

		[Test]
		[TestCase("a", 0, 1, "a")]
		[TestCase("abc", 0, 3, "abc")]
		[TestCase("abc", 0, 1, "a")]
		[TestCase("abc", 2, 1, "c")]
		[TestCase("abc", 0, 2, "ab")]
		[TestCase("abc", 1, 2, "bc")]
		[TestCase("abc", 1, 1, "b")]
		[TestCase(null, 0, 0, null)]

		[TestCase("a", 0, null, "a")]
		[TestCase("a", 1, null, "")]
		[TestCase("abc", 0, null, "abc")]
		[TestCase("abc", 1, null, "bc")]
		[TestCase("abc", 2, null, "c")]
		[TestCase("abc", 3, null, "")]
		[TestCase(null, 0, null, null)]
		public void SubstringTests(string source, int startIndex, int? length, string expected) {
			var segment = new StringSegment(source);
			if (length.HasValue) {
				segment = segment.Substring(startIndex, length.Value);
			} else {
				segment = segment.Substring(startIndex);
			}
			Assert.AreEqual(expected, (string)segment);
			Assert.AreEqual(expected, segment.ToString());
			Assert.IsTrue(segment.Equals(expected));
		}

		[Test]
		[TestCase("abc", -1, 1, "startIndex")]
		[TestCase("abc", 0, 4, "length")]
		[TestCase("abc", 1, 3, "length")]
		[TestCase("abc", 1, -1, "length")]
		[TestCase("abc", 2, 2, "length")]
		[TestCase(null, 0, 2, "length")]
		[TestCase(null, 1, 0, "startIndex")]
		public void InvalidSubstringArguments(string source, int startIndex, int length, string paramName) {
			var seg = new StringSegment(source);
			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => seg.Substring(startIndex, length));
			Assert.AreEqual(paramName, ex.ParamName);
		}
	}
}