using System;
using System.Collections.Generic;
using System.Text;

namespace Valley.Net.Bindings
{
    internal sealed class NullLogger : ITelemetryLogger
    {
        public void Error(string message, string source, Exception ex)
        {
          
        }

        public void Info(string message, string source)
        {
           
        }

        public void Verbose(string message, string source)
        {
            
        }

        public void Warning(string message, string source)
        {
           
        }
    }
}
