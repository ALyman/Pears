namespace Pears.Inputs {
    public interface IInput
    {
        bool IsEndOfStream { get; }
        IInput Next { get; }
    }

	public interface IInput<out TToken> : IInput {
		TToken Token { get; }
		new IInput<TToken> Next { get; }
	}

    internal interface IWrappedInput
    {
        IInput WrappedInput { get; }
    }

    internal interface IPositionTrackingInput
    {
        int Position { get; }
    }

    internal interface ILineTrackingInput
    {
        int Line { get; }
        int Column { get; }
    }
}