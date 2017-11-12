using System;
using System.Runtime.Serialization;

namespace Bitstamp
{
	[Serializable]
	public class BitstampException : Exception
	{
		#region Public Instance Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BitstampException" /> class.
		/// </summary>
		public BitstampException() 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BitstampException" /> class 
		/// with a descriptive message.
		/// </summary>
		/// <param name="message">A descriptive message to include with the exception.</param>
		public BitstampException(String message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BitstampException" /> class
		/// with the specified descriptive message and inner exception.
		/// </summary>
		/// <param name="message">A descriptive message to include with the exception.</param>
		/// <param name="innerException">A nested exception that is the cause of the current exception.</param>
		public BitstampException(String message, Exception innerException) : base(message, innerException)
		{
		}

		protected BitstampException(SerializationInfo serializationInfo, StreamingContext context)
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
