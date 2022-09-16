using System;
using System.IO;

using Pears.Parsers;

namespace Pears.Inputs
{
    public class TokenizerInput<TSource, TResult> : IInput<TResult>
    {
        private readonly IParser<TSource, TResult> tokenizer;
        private IInput<TSource> startInput;
        private IInput<TSource> finalInput;

        private TResult token;
        private TokenizerInput<TSource, TResult> next;

        public TokenizerInput(IInput<TSource> input, IParser<TSource, TResult> tokenizer)
        {
            this.startInput = input;
            this.tokenizer = tokenizer;
        }

        public bool IsEndOfStream
        {
            get
            {
                return startInput.IsEndOfStream;
            }
        }

        public TResult Token
        {
            get
            {
                if (startInput.IsEndOfStream)
                {
                    throw new EndOfStreamException();
                }
                EnsureRead();
                return token;
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
                else if (startInput.IsEndOfStream)
                {
                    return this;
                }
                else
                {
                    EnsureRead();
                    return next = new TokenizerInput<TSource, TResult>(finalInput, tokenizer);
                }
            }
        }

        private void EnsureRead()
        {
            if (finalInput == null)
            {
                this.token = tokenizer.TryParse(startInput, out finalInput);
            }
        }
    }
}