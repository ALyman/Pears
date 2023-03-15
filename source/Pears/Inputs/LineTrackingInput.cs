namespace Pears.Inputs;

public class LineTrackingInput : IInput<char>, IWrappedInput, IPositionTrackingInput, ILineTrackingInput
{
    private readonly IInput<char> _input;
    private readonly int _position;
    private readonly int _line;
    private readonly int _column;
    private LineTrackingInput _next;

    public LineTrackingInput(IInput<char> input, int position = 0, int line = 1, int column = 0)
    {
        _input = input;
        _position = position;
        this._line = line;
        this._column = column;
    }

    public bool IsEndOfStream => _input.IsEndOfStream;
    public char Token => _input.Token;
    public IInput<char> Next
    {
        get
        {
            if (_next != null) { return _next; }
            if (_input.IsEndOfStream) { return this; }

            return _input.Token switch
            {
                '\n' => _next = new LineTrackingInput(_input.Next, _position + 1, _line + 1, 0),
                '\r' => _next = new LineTrackingInput(_input.Next, _position + 1, _line, _column),
                _ => _next = new LineTrackingInput(_input.Next, _position + 1, _line, _column + 1)
            };
        }
    }

    IInput IInput.Next => this.Next;
    IInput IWrappedInput.WrappedInput => this._input;

    public int Position => _position;
    public int Line => _line;
    public int Column => _column;
}