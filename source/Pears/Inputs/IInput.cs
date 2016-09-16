namespace Pears.Inputs {
	public interface IInput<out TToken> {
		bool IsEndOfStream { get; }
		TToken Token { get; }
		IInput<TToken> Next { get; }
	}
}