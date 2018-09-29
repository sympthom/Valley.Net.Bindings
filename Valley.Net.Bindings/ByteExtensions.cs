using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valley.Net.Bindings
{
    internal static class ByteExtensions
    {
        public static string BytesToHex(this IEnumerable<byte> source)
        {
            return string.Join(" ", source.Select(x => x.ToString("x2")));
        }
    }
}
