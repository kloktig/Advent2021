using System.Collections.Generic;

namespace Advent2021;

public static class ExtFunc
{
    public static string ToStr<T>(this IEnumerable<T> objs, string sep = ",") => string.Join(sep, objs);
}