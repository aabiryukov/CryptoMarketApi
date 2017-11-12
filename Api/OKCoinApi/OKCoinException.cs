using System;
using System.Runtime.Serialization;

namespace OKCoin
{
    [Serializable]
    public class OKCoinException : Exception
    {
        private readonly string m_method;

        public OKCoinException() : base() { }
        public OKCoinException(string callerMethod, string message, Exception innerException)
			: base(message,innerException)
        {
            m_method = callerMethod;
        }

        public OKCoinException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public OKCoinException(string message)
            : base(message)
        {
        }

        public OKCoinException(string callerMethod, string message)
			: base(message)
        {
            m_method = callerMethod;
        }

	    protected OKCoinException(SerializationInfo info, StreamingContext context)
		    : base(info, context)
	    {
	    }

        public string RequestMethod { get { return m_method; } }
    }
}
