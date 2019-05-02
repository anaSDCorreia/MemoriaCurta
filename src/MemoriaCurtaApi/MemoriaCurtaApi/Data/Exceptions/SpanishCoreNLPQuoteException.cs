using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoriaCurtaAPI.Data.Exceptions
{
    public class SpanishCoreNLPQuoteException : Exception
    {
        public SpanishCoreNLPQuoteException(string message) : base(message) { }

        public SpanishCoreNLPQuoteException(string message, Exception e) : base(message, e) { }
    }
}
