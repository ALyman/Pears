using System;

namespace Pears.Inputs
{
    public class SelectInput<TSource, TResult> : IInput<TResult>, IWrappedInput
    {
        private Maybe<TResult> token = Maybe<TResult>.Empty;
        private readonly IInput<TSource> startInput;
        private readonly Func<TSource, TResult> selector;
        private IInput<TResult> next;

        public SelectInput(IInput<TSource> startInput, Func<TSource, TResult> selector)
        {
            this.startInput = startInput;
            this.selector = selector;
        }

        public bool IsEndOfStream => startInput.IsEndOfStream;

        public TResult Token
        {
            get
            {
                if (token.HasValue)
                {
                    return token;
                }
                else
                {
                    return token = selector(startInput.Token);
                }
            }
        }

        public IInput<TResult> Next
        {
            get
            {
                if (next != null)
                {
                    return next;
                }

                if (startInput.IsEndOfStream)
                {
                    return this;
                }

                return next = new SelectInput<TSource, TResult>(startInput.Next, selector);
            }
        }

        IInput IInput.Next => this.Next;
        IInput IWrappedInput.WrappedInput => this.startInput;
    }
}