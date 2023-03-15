using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Pears.Inputs;

namespace Pears.Tests {
	[TestFixture]
	public class BasicParsingTests {

		[TestCaseSource(nameof(IntsTestData))]
		[TestCase(int.MinValue)]
		[TestCase(int.MaxValue)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(-1)]
		public void SuccessfulMatch(int actual) {
			var parser = Parse.Match(actual);
			var input = new[] { actual }.AsInput();
			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(actual));
			Assert.That(finalInput, Is.SameAs(input.Next));
		}

		[TestCaseSource(nameof(IntsTestData))]
		[TestCase(int.MinValue)]
		[TestCase(int.MaxValue)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(-1)]
		public void FailedMatch(int actual) {
			var parser = Parse.Match(actual);
			var input = new[] { actual + 1 }.AsInput();
			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.False);
			Assert.That(finalInput, Is.SameAs(input));
		}

		protected static IEnumerable<int> IntsTestData() {
			var r = new Random(0);
			return Enumerable.Range(0, 5).Select(i => r.Next());
		}

		[Test]
		public void SuccessfulConcat() {
			var input = new MockInput<int>(3);
			var first = MockParser.Create(result: 1, expectedInput: input);
			var second = MockParser.Create(result: 2, expectedInput: input.Skip(1));

			var parser = Parse.Concat(Tuple.Create, first, second);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(finalInput, Is.SameAs(input.Skip(2)));
			Assert.That(result.Value, Is.EqualTo(Tuple.Create(1, 2)));
		}

		[Test]
		public void SuccessfulMultiConcat() {
			var input = new MockInput<int>(3);
			var first = MockParser.Create(result: 1, expectedInput: input);
			var second = MockParser.Create(result: 2, expectedInput: input.Skip(1));
			var third = MockParser.Create(result: 3, expectedInput: input.Skip(2));

			var parser = Parse.Concat(first, second, third);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(finalInput, Is.SameAs(input.Skip(3)));
			Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }));
		}

		[Test]
		[TestCase("failFirst")]
		[TestCase("failSecond")]
		[TestCase("failBoth")]
		public void FailedConcat(string whichFailed) {
			var input = new MockInput<int>(3);

			MockParser<int, int> first, second;
			IInput<int> expectedFinalInput;

			switch (whichFailed) {
				case "failFirst":
					expectedFinalInput = input;
					first = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					second = MockParser.Create(result: 2, expectedInput: input.Skip(1));
					break;

				case "failSecond":
					expectedFinalInput = input;
					first = MockParser.Create(result: 1, expectedInput: input);
					second = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input.Skip(1));
					break;

				case "failBoth":
					expectedFinalInput = input;
					first = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					second = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input.Skip(1));
					break;

				default:
					throw new NotImplementedException("Missing code for test case");
			}

			var parser = Parse.Concat(Tuple.Create, first, second);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.False);
			Assert.That(finalInput, Is.SameAs(expectedFinalInput));
			if (expectedFinalInput == input.Skip(1)) {
				Assert.That(second.CallCount, Is.EqualTo(0));
			}
		}

		[Test]
		[TestCase("success")]
		[TestCase("failFirst")]
		[TestCase("failSecond")]
		[TestCase("failBoth")]
		public void Alternatives(string whichFailed) {
			var input = new MockInput<int>(3);

			MockParser<int, int> first, second;
			IInput<int> expectedFinalInput;
			int expectedResult = -1;

			switch (whichFailed) {
				case "success":
					expectedFinalInput = input.Skip(1);
					expectedResult = 1;
					first = MockParser.Create(result: 1, expectedInput: input);
					second = MockParser.Create(result: 2, expectedInput: input);
					break;

				case "failFirst":
					expectedFinalInput = input.Skip(1);
					expectedResult = 2;
					first = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					second = MockParser.Create(result: 2, expectedInput: input);
					break;

				case "failSecond":
					expectedResult = 1;
					expectedFinalInput = input.Skip(1);
					first = MockParser.Create(result: 1, expectedInput: input);
					second = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					break;

				case "failBoth":
					expectedResult = -1;
					expectedFinalInput = input;
					first = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					second = MockParser.Create(result: Maybe<int>.Empty, expectedInput: input);
					break;

				default:
					throw new NotImplementedException("Missing code for test case");
			}

			var parser = Parse.Alternatives(_ => _, first, _ => _, second);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			if (whichFailed == "failBoth") {
				Assert.That(result.HasValue, Is.False);
				Assert.That(finalInput, Is.SameAs(expectedFinalInput));
			} else {
				Assert.That(result.HasValue, Is.True);
				Assert.That(result.Value, Is.EqualTo(expectedResult));
				Assert.That(finalInput, Is.SameAs(expectedFinalInput));
				if (expectedResult == 1) {
					Assert.That(second.CallCount, Is.EqualTo(0));
				}
			}
		}

		[Test]
		public void SuccessfulRepeatZeroOrMore() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> {
				{ input, 1 },
				{ input.Skip(1), 2 },
				{ input.Skip(2), 3 },
				{ input.Skip(3), Maybe<int>.Empty }
			};

			var parser = inner.ZeroOrMore();

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }.AsEnumerable()));
			Assert.That(finalInput, Is.SameAs(input.Skip(3)));
		}

		[Test]
		public void EmptyRepeatZeroOrMore() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> {
				{ input, Maybe<int>.Empty }
			};

			var parser = inner.ZeroOrMore();

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(Enumerable.Empty<int>()));
			Assert.That(finalInput, Is.SameAs(input));
		}

		[Test]
		public void SuccessfulRepeatOneOrMore() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> {
				{ input, 1 },
				{ input.Skip(1), 2 },
				{ input.Skip(2), 3 },
				{ input.Skip(3), Maybe<int>.Empty }
			};

			var parser = inner.OneOrMore();

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }.AsEnumerable()));
			Assert.That(finalInput, Is.SameAs(input.Skip(3)));
		}

		[Test]
		public void FailedRepeatOneOrMore() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> {
				{ input, Maybe<int>.Empty }
			};

			var parser = inner.OneOrMore();

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.False);
			Assert.That(finalInput, Is.SameAs(input));
		}

		[Test]
		public void SuccessfulRepeatExactly2() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> {
				{ input, 1 },
				{ input.Skip(1), 2 },
				{ input.Skip(2), 3 },
				{ input.Skip(3), Maybe<int>.Empty }
			};

			var parser = inner.Repeat(minimum: 2, maximum: 2);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(new[] { 1, 2 }.AsEnumerable()));
			Assert.That(finalInput, Is.SameAs(input.Skip(2)));
		}


        [Test]
        public void SuccessfulDelimitedRepeatExactly2()
        {
            var input = new MockInput<int>(5);

            var inner = new MockParser<int, int> {
                { input.Skip(0), 1 },
                { input.Skip(2), 3 },
                { input.Skip(4), 5 },
                { input.Skip(5), Maybe<int>.Empty }
            };

            var delimiter = new MockParser<int, int> {
                { input.Skip(1), 2 },
                { input.Skip(3), 4 },
            };

            var parser = inner.Repeat(delimiter, minimum: 2, maximum: 2);

            IInput<int> finalInput;
            var result = parser.TryParse(input, out finalInput);
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 1, 3 }.AsEnumerable()));
            Assert.That(finalInput, Is.SameAs(input.Skip(3)));
        }

        [Test]
		public void BoundRuleParser() {
			var input = new MockInput<int>(3);

			var inner = new MockParser<int, int> { { input, 1 } };

			var parser = Parse.Rule<int, int>();
			parser.Bind(inner);

			IInput<int> finalInput;
			var result = parser.TryParse(input, out finalInput);
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(1));
			Assert.That(finalInput, Is.SameAs(input.Skip(1)));
		}

		[Test]
		public void UnboundRuleParser() {
			var input = new MockInput<int>(3);

			var parser = Parse.Rule<int, int>();

			IInput<int> finalInput;
			Assert.That(() => parser.TryParse(input, out finalInput), Throws.InvalidOperationException);
		}

		[Test]
		public void ReboundRuleParser() {
			var input = new MockInput<int>(3);

			var inner1 = new MockParser<int, int> { { input, 1 } };
			var inner2 = new MockParser<int, int> { { input, 2 } };

			var parser = Parse.Rule<int, int>();
			parser.Bind(inner1);

			Assert.That(() => parser.Bind(inner1), Throws.Nothing);
			Assert.That(() => parser.Bind(inner2), Throws.InvalidOperationException);
		}
	}
}