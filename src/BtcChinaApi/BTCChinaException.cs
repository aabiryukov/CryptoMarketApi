using System;
using System.Runtime.Serialization;

namespace BTCChina
{
    [Serializable()]
    public class BTCChinaException : Exception
    {
        private readonly string m_method;
        private readonly string m_jsonId;

        public BTCChinaException() : base() { }
        public BTCChinaException(string callerMethod, string callerID, string message, Exception innerException)
			: base(message,innerException)
        {
            m_method = callerMethod;
            m_jsonId = callerID;
        }

        public BTCChinaException(string callerMethod, string callerID, string message)
			: base(message)
        {
            m_method = callerMethod;
            m_jsonId = callerID;
        }

	    public BTCChinaException(SerializationInfo info, StreamingContext context)
		    : base(info, context)
	    {
	    }

        public string RequestMethod { get { return m_method; } }
        public string RequestID { get { return m_jsonId; } }
    }
}
