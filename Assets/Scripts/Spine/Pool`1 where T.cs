using System;
using System.Collections.Generic;

namespace Spine
{
	public class Pool<T> where T : class, new()
	{
		public interface IPoolable
		{
			void Reset();
		}

		public readonly int max;

		private readonly Stack<T> freeObjects;

		public int Count => freeObjects.Count;

		public int Peak
		{
			get;
			private set;
		}

		public Pool(int initialCapacity = 16, int max = int.MaxValue)
		{
			freeObjects = new Stack<T>(initialCapacity);
			this.max = max;
		}

		public T Obtain()
		{
			return (freeObjects.Count != 0) ? freeObjects.Pop() : new T();
		}

		public void Free(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "obj cannot be null");
			}
			if (freeObjects.Count < max)
			{
				freeObjects.Push(obj);
				Peak = Math.Max(Peak, freeObjects.Count);
			}
			Reset(obj);
		}

		public void Clear()
		{
			freeObjects.Clear();
		}

		protected void Reset(T obj)
		{
			(obj as IPoolable)?.Reset();
		}
	}
}
