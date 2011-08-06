using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pears
{
    public static class InputSource
    {
        public static IInputSource<TToken> AsInputSource<TToken>(this IEnumerable<TToken> from)
        {
            return new EnumeratorInputSource<TToken>(from.GetEnumerator());
        }
    }
}
