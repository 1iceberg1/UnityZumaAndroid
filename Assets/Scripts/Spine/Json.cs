using SharpJson;
using System.IO;

namespace Spine
{
	public static class Json
	{
		public static object Deserialize(TextReader text)
		{
			JsonDecoder jsonDecoder = new JsonDecoder();
			jsonDecoder.parseNumbersAsFloat = true;
			return jsonDecoder.Decode(text.ReadToEnd());
		}
	}
}
