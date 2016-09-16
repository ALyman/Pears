using System;
using System.Collections.Generic;
using System.Linq;

namespace Pears {
	public struct StringSegment : IEquatable<StringSegment>, IEquatable<string> {
		private readonly int _length;
		private readonly int _startIndex;
		private readonly string _string;

		public StringSegment(string @string, int startIndex, int length) {
			if (@string == null) {
				if (startIndex != 0) {
					throw new ArgumentOutOfRangeException("startIndex", "startIndex must be zero for a null string");
				}
				if (length != 0) {
					throw new ArgumentOutOfRangeException("length", "length must be zero for a null string");
				}
			} else {
				if (startIndex < 0) {
					throw new ArgumentOutOfRangeException("startIndex", "startIndex must be non-negative");
				}
				if (length < 0) {
					throw new ArgumentOutOfRangeException("length", "length must be non-negative");
				}
				if (startIndex + length > @string.Length) {
					throw new ArgumentOutOfRangeException("length", "startIndex and length cause segment to go past the bounds of string");
				}
			}

			this._string = @string;
			this._startIndex = startIndex;
			this._length = length;
		}

		public StringSegment(string @string) {
			this._string = @string;
			this._startIndex = 0;
			this._length = @string == null ? 0 : @string.Length;
		}

		public int Length {
			get { return _length; }
		}

		public int StartIndex {
			get { return _startIndex; }
		}

		public string String {
			get { return _string; }
		}

		public static explicit operator string(StringSegment source) {
			if (source._string == null) { return null; }
			return source.String.Substring(source.StartIndex, source.Length);
		}

		public static implicit operator StringSegment(string source) {
			return new StringSegment(source, 0, source == null ? 0 : source.Length);
		}

		public bool Equals(StringSegment other) {
			if (other._string == this._string && other._startIndex == this._startIndex && other._length == this._length) {
				return true;
			}

			if (other._length != this._length) {
				return false;
			}

			var thisString = this._string;
			var otherString = other._string;
			var endIndex = this._startIndex + this._length;

			for (int i = this._startIndex, j = other._startIndex; i < endIndex; i++, j++) {
				if (thisString[i] != otherString[j]) {
					return false;
				}
			}

			return true;
		}

		public bool Equals(string other) {
			return this.Equals((StringSegment)other);
		}

		public IEnumerable<StringSegment> Split(params char[] separator) {
			if (_string == null || _length == 0) {
				yield return this;
				yield break;
			}

			var _endIndex = _startIndex + _length;

			int current, next;

			for (current = _startIndex, next = -1; current < _endIndex && current != -1; current = next + 1) {
				next = _string.IndexOfAny(separator, current, _endIndex - current);
				if (next > _endIndex || next == -1) {
					break;
				} else {
					yield return new StringSegment(_string, current, next - current);
				}
			}

			if (current <= _endIndex) {
				yield return new StringSegment(_string, current, _endIndex - current);
			}
		}

		public IEnumerable<StringSegment> Split(char[] separator, StringSplitOptions options) {
			if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries)) {
				return Split(separator).Where(s => s.Length > 0);
			} else {
				return Split(separator);
			}
		}

		public StringSegment Substring(int startIndex) {
			return Substring(startIndex, _length - startIndex);
		}

		public StringSegment Substring(int startIndex, int length) {
			if (startIndex < 0) {
				throw new ArgumentOutOfRangeException("startIndex", "startIndex must be non-negative");
			}

			if (length < 0) {
				throw new ArgumentOutOfRangeException("length", "length must be non-negative");
			}

			if (startIndex > _length) {
				throw new ArgumentOutOfRangeException("startIndex", "startIndex must be before the end of the string");
			}

			if (startIndex + length > _length) {
				throw new ArgumentOutOfRangeException("length", "startIndex and length cause segment to go past the bounds of string");
			}

			return new StringSegment(_string, _startIndex + startIndex, length);
		}

		public override string ToString() {
			if (_string == null) { return null; }
			return _string.Substring(_startIndex, _length);
		}
	}
}