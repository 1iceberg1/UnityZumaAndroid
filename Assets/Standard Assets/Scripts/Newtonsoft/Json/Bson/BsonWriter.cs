using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Bson
{
	public class BsonWriter : JsonWriter
	{
		private readonly BsonBinaryWriter _writer;

		private BsonToken _root;

		private BsonToken _parent;

		private string _propertyName;

		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return _writer.DateTimeKindHandling;
			}
			set
			{
				_writer.DateTimeKindHandling = value;
			}
		}

		public BsonWriter(Stream stream)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			_writer = new BsonBinaryWriter(stream);
		}

		public override void Flush()
		{
			_writer.Flush();
		}

		protected override void WriteEnd(JsonToken token)
		{
			base.WriteEnd(token);
			RemoveParent();
			if (base.Top == 0)
			{
				_writer.WriteToken(_root);
			}
		}

		public override void WriteComment(string text)
		{
			throw new JsonWriterException("Cannot write JSON comment as BSON.");
		}

		public override void WriteStartConstructor(string name)
		{
			throw new JsonWriterException("Cannot write JSON constructor as BSON.");
		}

		public override void WriteRaw(string json)
		{
			throw new JsonWriterException("Cannot write raw JSON as BSON.");
		}

		public override void WriteRawValue(string json)
		{
			throw new JsonWriterException("Cannot write raw JSON as BSON.");
		}

		public override void WriteStartArray()
		{
			base.WriteStartArray();
			AddParent(new BsonArray());
		}

		public override void WriteStartObject()
		{
			base.WriteStartObject();
			AddParent(new BsonObject());
		}

		public override void WritePropertyName(string name)
		{
			base.WritePropertyName(name);
			_propertyName = name;
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseOutput && _writer != null)
			{
				_writer.Close();
			}
		}

		private void AddParent(BsonToken container)
		{
			AddToken(container);
			_parent = container;
		}

		private void RemoveParent()
		{
			_parent = _parent.Parent;
		}

		private void AddValue(object value, BsonType type)
		{
			AddToken(new BsonValue(value, type));
		}

		internal void AddToken(BsonToken token)
		{
			if (_parent != null)
			{
				if (_parent is BsonObject)
				{
					((BsonObject)_parent).Add(_propertyName, token);
					_propertyName = null;
				}
				else
				{
					((BsonArray)_parent).Add(token);
				}
				return;
			}
			if (token.Type != BsonType.Object && token.Type != BsonType.Array)
			{
				throw new JsonWriterException("Error writing {0} value. BSON must start with an Object or Array.".FormatWith(CultureInfo.InvariantCulture, token.Type));
			}
			_parent = token;
			_root = token;
		}

		public override void WriteNull()
		{
			base.WriteNull();
			AddValue(null, BsonType.Null);
		}

		public override void WriteUndefined()
		{
			base.WriteUndefined();
			AddValue(null, BsonType.Undefined);
		}

		public override void WriteValue(string value)
		{
			base.WriteValue(value);
			if (value == null)
			{
				AddValue(null, BsonType.Null);
			}
			else
			{
				AddToken(new BsonString(value, includeLength: true));
			}
		}

		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(uint value)
		{
			if (value > int.MaxValue)
			{
				throw new JsonWriterException("Value is too large to fit in a signed 32 bit integer. BSON does not support unsigned values.");
			}
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Long);
		}

		public override void WriteValue(ulong value)
		{
			if (value > long.MaxValue)
			{
				throw new JsonWriterException("Value is too large to fit in a signed 64 bit integer. BSON does not support unsigned values.");
			}
			base.WriteValue(value);
			AddValue(value, BsonType.Long);
		}

		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Number);
		}

		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Number);
		}

		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Boolean);
		}

		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			AddToken(new BsonString(value.ToString(), includeLength: true));
		}

		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Integer);
		}

		public override void WriteValue(decimal value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Number);
		}

		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Date);
		}

		public override void WriteValue(DateTimeOffset value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Date);
		}

		public override void WriteValue(byte[] value)
		{
			base.WriteValue(value);
			AddValue(value, BsonType.Binary);
		}

		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			AddToken(new BsonString(value.ToString(), includeLength: true));
		}

		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			AddToken(new BsonString(value.ToString(), includeLength: true));
		}

		public override void WriteValue(Uri value)
		{
			base.WriteValue(value);
			AddToken(new BsonString(value.ToString(), includeLength: true));
		}

		public void WriteObjectId(byte[] value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if (value.Length != 12)
			{
				throw new Exception("An object id must be 12 bytes");
			}
			AutoComplete(JsonToken.Undefined);
			AddValue(value, BsonType.Oid);
		}

		public void WriteRegex(string pattern, string options)
		{
			ValidationUtils.ArgumentNotNull(pattern, "pattern");
			AutoComplete(JsonToken.Undefined);
			AddToken(new BsonRegex(pattern, options));
		}
	}
}
