#if NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Pears.Inputs;

namespace Pears.Tests;

[TestFixture]
public class MemoryInputTests
{
    [Test]
    public void ScanForward()
    {
        var expected = new Memory<int>(Enumerable.Range(0, 10).ToArray());
        MemoryInput<int> input = expected.AsInput();
        Assert.AreEqual(expected.ToArray(), input.AsEnumerable());
        Assert.AreEqual(expected.ToArray().Skip(2), input.Next.Next.AsEnumerable());
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
        var expected = new Memory<int>(Enumerable.Empty<int>().ToArray());
        MemoryInput<int> input = expected.AsInput();
        Assert.AreEqual(expected.ToArray(), input.AsEnumerable().ToArray());

        Assert.Throws<EndOfStreamException>(() => { var token = input.Token; });
        Assert.AreSame(input, input.Next);
    }

    [TestCaseSource(nameof(WhereCases))]
    public void Where(Func<int, bool> predicate)
    {
        var original = new Memory<int>(Enumerable.Range(0, 10).ToArray());
        MemoryInput<int> input = original.AsInput();

        Assert.AreEqual(
            original.ToArray().Where(predicate).ToArray(),
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
        var original = new Memory<int>(Enumerable.Range(0, 10).ToArray());
        MemoryInput<int> input = original.AsInput();

        Assert.AreEqual(
            original.ToArray().Select(predicate).ToArray(),
            input.Select(predicate).AsEnumerable().ToArray()
        );
    }
}

#endif