using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pears
{
    public interface IInputSource<TToken>
    {
        TToken Value { get; }
        IInputSource<TToken> Next { get; }
        bool EndOfInput { get; }
    }
}
