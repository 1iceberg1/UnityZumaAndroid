using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultSerializationBinder : SerializationBinder
	{
		internal struct TypeNameKey
		{
			internal readonly string AssemblyName;

			internal readonly string TypeName;

			public TypeNameKey(string assemblyName, string typeName)
			{
				AssemblyName = assemblyName;
				TypeName = typeName;
			}

			public override int GetHashCode()
			{
				return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0) ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
			}

			public override bool Equals(object obj)
			{
				if (!(obj is TypeNameKey))
				{
					return false;
				}
				return Equals((TypeNameKey)obj);
			}

			public bool Equals(TypeNameKey other)
			{
				return AssemblyName == other.AssemblyName && TypeName == other.TypeName;
			}
		}

		internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

		private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache = new ThreadSafeStore<TypeNameKey, Type>(GetTypeFromTypeNameKey);

		[CompilerGenerated]
		private static Func<TypeNameKey, Type> _003C_003Ef__mg_0024cache0;

		private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
		{
			string assemblyName = typeNameKey.AssemblyName;
			string typeName = typeNameKey.TypeName;
			if (assemblyName != null)
			{
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly == null)
				{
					throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));
				}
				Type type = assembly.GetType(typeName);
				if (type == null)
				{
					throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));
				}
				return type;
			}
			return Type.GetType(typeName);
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			return _typeCache.Get(new TypeNameKey(assemblyName, typeName));
		}
	}
}
