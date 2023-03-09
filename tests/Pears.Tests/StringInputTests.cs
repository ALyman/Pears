using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;

using Pears.Inputs;

namespace Pears.Tests
{
    [TestFixture]
    public class StringInputTests
    {
        [Test]
        public void ScanForward()
        {
            var expected = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".ToList();
            StringInput input = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".AsInput();
            Assert.AreEqual(expected.Select(ch => (int)ch).ToArray(), input.AsEnumerable().Select(ch => (int)ch).ToArray());
            Assert.AreEqual(expected.Skip(2).Select(ch => (int)ch).ToArray(), input.Next.Next.AsEnumerable().Select(ch => (int)ch).ToArray());
            IInput<char> a, b;

            for (a = input, b = input; !a.IsEndOfStream && !b.IsEndOfStream; a = a.Next, b = b.Next)
            {
                Assert.AreSame(a, b);
                Assert.AreEqual(a.Token, b.Token);
            }
        }

        [Test]
        public void EndOfStream()
        {
            var expected = "".ToList();
            StringInput input = "".AsInput();
            Assert.AreEqual(expected.Select(ch => (int)ch), input.AsEnumerable().Select(ch => (int)ch));

            Assert.Throws<EndOfStreamException>(() => { var token = input.Token; });
            Assert.AreSame(input, input.Next);
        }

        [TestCaseSource(nameof(WhereCases))]
        public void Where(Func<char, bool> predicate)
        {
            var original = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".ToList();
            StringInput input = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".AsInput();

            Assert.AreEqual(
                original.Where(predicate).Select(ch => (int)ch).ToArray(),
                input.Where(predicate).AsEnumerable().Select(ch => (int)ch).ToArray()
            );
        }

        private static IEnumerable<Func<char, bool>> WhereCases()
        {
            yield return i => i % 2 == 0;
            yield return i => i % 2 != 0;
            yield return i => i % 3 == 0;

        }

        [TestCaseSource(nameof(WhereCases))]
        public void Select(Func<char, bool> predicate)
        {
            var original = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".ToList();
            StringInput input = "\x0\x1\x2\x3\x4\x5\x6\x7\x8\x9".AsInput();

            Assert.AreEqual(
                original.Select(predicate).ToArray(),
                input.Select(predicate).AsEnumerable().ToArray()
            );
        }
    }
}