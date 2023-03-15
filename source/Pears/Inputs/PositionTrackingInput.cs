using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pears.Inputs
{
    public class PositionTrackingInput<T> : IInput<T>, IWrappedInput, IPositionTrackingInput
    {
        private readonly IInput<T> _input;
        private readonly int _position;
        private PositionTrackingInput<T> _next;

        public PositionTrackingInput(IInput<T> input, int position = 0)
        {
            _input = input;
            _position = position;
        }

        public bool IsEndOfStream => _input.IsEndOfStream;
        public T Token => _input.Token;
        public IInput<T> Next
        {
            get
            {
                if (_next != null) { return _next; }
                if (_input.IsEndOfStream) { return this; }
                return _next = new PositionTrackingInput<T>(_input.Next, _position + 1);
            }
        }

        IInput IInput.Next => this.Next;
        IInput IWrappedInput.WrappedInput => this._input;

        public int Position => _position;
    }
}
