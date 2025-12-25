namespace Opal.Rendering;

/// <summary>
/// String cache for <see cref="ConsoleChar"/> that should render as multiple UTF-16 characters, such as emoji.
/// </summary>
/// <remarks>
/// The cache has a limited size. If the limit is reached, it will start overwriting old cached strings in the order that they were added.
/// </remarks>
public static class ConsoleCharStringCache
{
    private const ushort _cacheSize = ushort.MaxValue;
    private static ushort _position = 0;
    private static string[] _cache = new string[_cacheSize];
    private static readonly Lock _lockObject = new Lock();

    /// <summary>
    /// Clear the string cache, allowing GC to collect the old strings.
    /// </summary>
    /// <remarks>
    /// This method is intended to be used in scenarios where the cache contains many long strings, causing high memory usage.
    /// </remarks>
    public static void Clear()
    {
        _cache = new string[_cacheSize];
    }

    /// <summary>
    /// Returns the string that corresponds to <paramref name="index"/> in the cache.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The string corresponds to <paramref name="index"/>, or <c>null</c> if the index is not currently used.</returns>
    public static string? Get(ushort index)
    {
        return _cache.ElementAtOrDefault(index);
    }

    /// <summary>
    /// Adds <paramref name="sequence"/> to the cache if it is currently not in the cache, returning the index of the string in the cache.
    /// </summary>
    /// <param name="sequence">The sequence to be stored in the cache.</param>
    /// <returns>The index of <paramref name="sequence"/> in the cache.</returns>
    internal static ushort Add(string sequence)
    {
        lock (_lockObject)
        {
            int index = Array.FindIndex(_cache, x => x != null && x.Equals(sequence));
            if (index >= 0)
            {
                return (ushort)index;
            }

            ushort newIndex = _position;
            _cache[_position] = sequence;

            _position++;
            if (_position >= _cacheSize)
            {
                _position = 0;
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
