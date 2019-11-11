using System;
using System.Collections.Generic;
using System.Linq;

namespace PakTrack.Utilities
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int numOfChunks)
        {
            var chunkSize = Math.Ceiling((double)source.Count / numOfChunks);
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}