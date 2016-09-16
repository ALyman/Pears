using NUnit.Framework;

namespace Pears.Tests
{
    [TestFixture]
    public class MaybeTests {

        [Test]
        public void StructValue() {
            var value = new Maybe<int>(1);
            Assert.That(value.HasValue, Is.True);
            Assert.That(value.Value, Is.EqualTo(1));

            var empty = Maybe<int>.Empty;
            Assert.That(empty.HasValue, Is.False);
            Assert.That(() => empty.Value, Throws.InvalidOperationException);
        }

        [Test]
        public void ReferenceValue() {
            var value = new Maybe<string>("a");
            Assert.That(value.HasValue, Is.True);
            Assert.That(value.Value, Is.EqualTo("a"));

            var empty = Maybe<string>.Empty;
            Assert.That(empty.HasValue, Is.False);
            Assert.That(() => empty.Value, Throws.InvalidOperationException);
        }
    }
}