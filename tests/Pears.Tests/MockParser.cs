using System;
using System.Collections.Generic;

using NUnit.Framework;

using Pears.Inputs;
using Pears.Parsers;

namespace Pears.Tests {
	class MockParser<TToken, TResult> : Dictionary<IInput<TToken>, Maybe<TResult>>, IParser<TToken, TResult> {
		public MockParser() {}

		public MockParser(Maybe<TResult> result, IInput<TToken> expectedInput) {
			this.Add(expectedInput, result);
		}

		public string Name { get; set; }

		public Maybe<TResult> TryParse(IInput<TToken> input, out IInput<TToken> finalInput) {
			this.CallCount++;
			Maybe<TResult> result;
			Assume.That(this.TryGetValue(input, out result), Is.True, "MockParser got asked about an unexpected input: {0}", input);
			finalInput = result.HasValue ? input.Next : input;
			return result;
		}

        public int CallCount { get; private set; }
	}

	static class MockParser {
		public static MockParser<TToken, TResult> Create<TToken, TResult>(TResult result, IInput<TToken> expectedInput) {
			return new MockParser<TToken, TResult>(result, expectedInput);
		}

		public static MockParser<TToken, TResult> Create<TToken, TResult>(Maybe<TResult> result, IInput<TToken> expectedInput) {
			return new MockParser<TToken, TResult>(result, expectedInput);
		}
	}
}