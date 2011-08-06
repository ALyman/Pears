using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pears.Tests
{
    [TestClass]
    public class InputSourceTests
    {
        [TestMethod]
        public void When_an_InputSource_is_enumerated_it_pulls_lazily()
        {
            var enumerable = new Mock<IEnumerable<int>>(MockBehavior.Strict);
            var enumerator = new Mock<IEnumerator<int>>(MockBehavior.Strict);
            enumerable.Setup(e => e.GetEnumerator()).Returns(() => enumerator.Object);
            enumerator.SetupSequence(e => e.MoveNext()).Returns(true).Returns(true).Returns(false);
            enumerator.SetupSequence(e => e.Current).Returns(1).Returns(2).Throws<Exception>();

            var inputSource = enumerable.Object.AsInputSource();
            Assert.IsFalse(inputSource.EndOfInput);
            Assert.AreEqual(1, inputSource.Value);
            Assert.IsFalse(inputSource.Next.EndOfInput);
            Assert.AreEqual(2, inputSource.Next.Value);
            Assert.IsTrue(inputSource.Next.Next.EndOfInput);
            Assert.AreSame(inputSource.Next.Next, inputSource.Next.Next.Next);
            try
            {
                var shouldThrow = inputSource.Next.Next.Value;
                Assert.Fail("Did not throw the epected exception");
            } catch (EndOfStreamException) { }
        }
    }
}
