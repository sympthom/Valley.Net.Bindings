using System;
using System.Collections.Generic;
using System.Text;

namespace Valley.Net.Bindings
{
    public sealed class ErrorAsyncEventArgs : EventArgs
    {
        public Exception Error { get; set; }

        public OrderedAsyncState State { get; set; }
    }
}
