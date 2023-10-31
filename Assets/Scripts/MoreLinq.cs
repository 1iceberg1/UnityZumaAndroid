using System;
using System.Collections.Generic;

public static class MoreLinq
{
	public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
	{
		return source.MaxBy(selector, Comparer<TKey>.Default);
	}

	public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (selector == null)
		{
			throw new ArgumentNullException("selector");
		}
		if (comparer == null)
		{
			throw new ArgumentNullException("comparer");
		}
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			TSource val = enumerator.Current;
			TKey y = selector(val);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey val2 = selector(current);
				if (comparer.Compare(val2, y) > 0)
				{
					val = current;
					y = val2;
				}
			}
			return val;
		}
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
	{
		return source.MinBy(selector, Comparer<TKey>.Default);
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (selector == null)
		{
			throw new ArgumentNullException("selector");
		}
		if (comparer == null)
		{
			throw new ArgumentNullException("comparer");
		}
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			TSource val = enumerator.Current;
			TKey y = selector(val);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey val2 = selector(current);
				if (comparer.Compare(val2, y) < 0)
				{
					val = current;
					y = val2;
				}
			}
			return val;
		}
	}
}
