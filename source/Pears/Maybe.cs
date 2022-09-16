using System;

namespace Pears {
	[Serializable]
	public struct Maybe<T> {
		public static readonly Maybe<T> Empty;
		private T value;
		private bool hasValue;

		public Maybe(T value) {
			this.hasValue = true;
			this.value = value;
		}

		public static implicit operator Maybe<T>(T x) {
			return new Maybe<T>(x);
		}

		public static implicit operator T(Maybe<T> x) {
			return x.Value;
		}

		public T Value {
			get {
				if (!hasValue) {
					throw new InvalidOperationException();
				}
				return value;
			}
		}

		public bool HasValue {
			get { return hasValue; }
		}

	    public override string ToString()
	    {
	        if (HasValue)
	        {
	            return $"Some({Value})";
	        }
	        else
	        {
	            return "None()";
	        }
	    }
	}
}