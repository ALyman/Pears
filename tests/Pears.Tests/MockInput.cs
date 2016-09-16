using Pears.Inputs;

namespace Pears.Tests {
	public class MockInput<T> : IInput<T> {
		private readonly int length;
		private readonly int position;

		public MockInput(int length)
			: this(0, length) {
		}

		private MockInput(int position, int length) {
			this.length = length;
			this.position = position;
			if (position >= length) {
				this.IsEndOfStream = true;
				this.Next = this;
			} else {
				this.IsEndOfStream = false;
				this.Next = new MockInput<T>(position + 1, length);
			}
		}

		public bool IsEndOfStream { get; private set; }
		public T Token { get; private set; }
		public IInput<T> Next { get; private set; }

		public override string ToString() {
			return string.Format("Input at position {0} of {1}", position, length);
		}
	}
}