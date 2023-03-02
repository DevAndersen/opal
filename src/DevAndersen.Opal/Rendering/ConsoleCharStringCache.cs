namespace DevAndersen.Opal.Rendering;

/// <summary>
/// String cache for <see cref="ConsoleChar"/> that should render as multiple UTF-16 characters, such as emoji.
/// </summary>
/// <remarks>
/// The cache has a limited size. If the limit is reached, it will start overwriting old cached strings in the order that they were added.
/// </remarks>
public static class ConsoleCharStringCache
{
    private const ushort cacheSize = ushort.MaxValue;
    private static ushort position = 0;
    private static string[] cache = new string[cacheSize];
    private static readonly object lockObject = new object();

    /// <summary>
    /// Clear the string cache, allowing GC to collect the old strings.
    /// </summary>
    /// <remarks>
    /// This method is intended to be used in scenarios where the cache contains many long strings, causing high memory usage.
    /// </remarks>
    public static void Clear()
    {
        cache = new string[cacheSize];
    }

    /// <summary>
    /// Returns the string that corresponds to <paramref name="index"/> in the cache.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The string corresponds to <paramref name="index"/>, or <c>null</c> if the index is not currently used.</returns>
    public static string? Get(ushort index)
    {
        return cache.ElementAtOrDefault(index);
    }

    /// <summary>
    /// Adds <paramref name="sequence"/> to the cache if it is currently not in the cache, returning the index of the string in the cache.
    /// </summary>
    /// <param name="sequence">The sequence to be stored in the cache.</param>
    /// <returns>The index of <paramref name="sequence"/> in the cache.</returns>
    internal static ushort Add(string sequence)
    {
        lock (lockObject)
        {
            int index = Array.FindIndex(cache, x => x != null && x.Equals(sequence));
            if (index >= 0)
            {
                return (ushort)index;
            }

            ushort newIndex = position;
            cache[position] = sequence;

            position++;
            if (position >= cacheSize)
            {
                position = 0;
            }

            return newIndex;
        }
    }

    /// <summary>
    /// Append the string corresponds to <paramref name="index"/> to <paramref name="stringBuilder"/>, returning the length of the appended string.
    /// </summary>
    /// <param name="stringBuilder"></param>
    /// <param name="index"></param>
    /// <returns>The length of the appended string.</returns>
    internal static int AppendFromCache(StringBuilder stringBuilder, ushort index)
    {
        string? cachedString = Get(index);
        stringBuilder.Append(cachedString);
        return cachedString?.Length ?? 0;
    }
}
