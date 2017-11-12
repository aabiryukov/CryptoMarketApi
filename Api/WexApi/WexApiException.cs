using System;
using System.Runtime.Serialization;

namespace Wex
{
	[Serializable]
	public class WexApiException : Exception
	{
		#region Public Instance Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WexApiException" /> class.
		/// </summary>
		public WexApiException() 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WexApiException" /> class 
		/// with a descriptive message.
		/// </summary>
		/// <param name="message">A descriptive message to include with the exception.</param>
		public WexApiException(String message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WexApiException" /> class
		/// with the specified descriptive message and inner exception.
		/// </summary>
		/// <param name="message">A descriptive message to include with the exception.</param>
		/// <param name="innerException">A nested exception that is the cause of the current exception.</param>
		public WexApiException(String message, Exception innerException) : base(message, innerException)
		{
		}

		protected WexApiException(SerializationInfo serializationInfo, StreamingContext context)
			: base(serializationInfo, context)
		{
		}

		#endregion Public Instance Constructors

		#region Override implementation of Object
/*
		/// <summary>
		/// Creates and returns a string representation of the current 
		/// exception.
		/// </summary>
		/// <returns>
		/// A string representation of the current exception.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}{2}",
			                     Message, Environment.NewLine, base.ToString());
		}
*/
		#endregion Override implementation of Object
	}
}
