using System;
using System.Runtime.Serialization;

namespace Huobi
{
    [Serializable]
    public class HuobiException : Exception
    {
        private readonly string m_method;

        public HuobiException() : base() { }
        public HuobiException(string callerMethod, string message, Exception innerException)
			: base(message,innerException)
        {
            m_method = callerMethod;
        }

        public HuobiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HuobiException(string message)
            : base(message)
        {
        }

        public HuobiException(string callerMethod, string message)
			: base(message)
        {
            m_method = callerMethod;
        }

	    protected HuobiException(SerializationInfo info, StreamingContext context)
		    : base(info, context)
	    {
	    }

        public string RequestMethod { get { return m_method; } }
    }
}
