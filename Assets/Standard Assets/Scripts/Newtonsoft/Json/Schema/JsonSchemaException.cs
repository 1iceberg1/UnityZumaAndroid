using System;

namespace Newtonsoft.Json.Schema
{
	public class JsonSchemaException : Exception
	{
		public int LineNumber
		{
			get;
			private set;
		}

		public int LinePosition
		{
			get;
			private set;
		}

		public JsonSchemaException()
		{
		}

		public JsonSchemaException(string message)
			: base(message)
		{
		}

		public JsonSchemaException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal JsonSchemaException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, innerException)
		{
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}
	}
}
