using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;

using Pears.Inputs;

namespace Pears.Tests
{
    [TestFixture]
    public class EnumerableInputTests
    {
        [Test]
        public void ScanForward()
        {
            var expected = Enumerable.Range(0, 10);
            var input = expected.AsInput();
            Assert.AreEqual(expected, input.AsEnumerable());
            Assert.AreEqual(expected.Skip(2), input.Next.Next.AsEnumerable());
            IInput<int> a, b;

            for (a = input, b = input; !a.IsEndOfStream && !b.IsEndOfStream; a = a.Next, b = b.Next)
            {
                Assert.AreSame(a, b);
                Assert.AreEqual(a.Token, b.Token);
            }
        }

        [Test]
        public void EndOfStream()
        {
            var expected = Enumerable.Empty<int>();
            var input = expected.AsInput();
            Assert.AreEqual(expected, input.AsEnumerable());

            Assert.Throws<EndOfStreamException>(() => { var token = input.Token; });
            Assert.AreSame(input, input.Next);
        }

        [TestCaseSource(nameof(WhereCases))]
        public void Where(Func<int, bool> predicate)
        {
            var original = Enumerable.Range(0, 10);
            var input = original.AsInput();

            Assert.AreEqual(
                original.Where(predicate).ToArray(),
                input.Where(predicate).AsEnumerable().ToArray()
            );
        }

        private static IEnumerable<Func<int, bool>> WhereCases()
        {
            yield return i => i % 2 == 0;
            yield return i => i % 2 != 0;
            yield return i => i % 3 == 0;

        }

        [TestCaseSource(nameof(WhereCases))]
        public void Select(Func<int, bool> predicate)
        {
            var original = Enumerable.Range(0, 10);
            var input = original.AsInput();

            Assert.AreEqual(
                original.Select(predicate).ToArray(),
                input.Select(predicate).AsEnumerable().ToArray()
            );
        }
    }
}