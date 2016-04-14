#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

public static string ToJoinedString<T>(this IEnumerable<T> source, string separator = "")
{
    return string.Join(separator, source);
}

public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] values)
{
    if (source == null) throw new ArgumentNullException("source");

    return source.ConcatCore(values);
}

private static IEnumerable<TSource> ConcatCore<TSource>(this IEnumerable<TSource> source, params TSource[] values)
{
    foreach (var item in source) yield return item;
    foreach (var x in values) yield return x;
}
