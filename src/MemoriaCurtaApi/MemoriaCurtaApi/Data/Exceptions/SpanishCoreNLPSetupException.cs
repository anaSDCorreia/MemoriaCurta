using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoriaCurtaAPI.Data.Exceptions
{
    public class SpanishCoreNLPSetupException : Exception
    {

        public SpanishCoreNLPSetupException(string message) : base(message) { }

        public SpanishCoreNLPSetupException(string message, Exception e) : base(message, e) { }

    }
}
