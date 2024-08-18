using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ServerCore
{
    public static class CollectionEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return CollectionsMarshal.AsSpan(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> AsDictionary<TKey, TValue>(
            this List<TValue> list, 
            Func<TValue, TKey> keyGetter
        ) where TKey : notnull
        {
            if (list == null) return default;

            var dictionary = new Dictionary<TKey, TValue>(list.Count);
            foreach (var item in CollectionsMarshal.AsSpan(list))
            {
                dictionary.Add(keyGetter(item), item);
            }

            return dictionary;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> AsDictionary<TKey, TValue, TItem>(
            this List<TItem> list, 
            Func<TItem, TKey> keyGetter, 
            Func<TItem, TValue> valueGetter
        ) where TKey : notnull
        {
            if (list == null) return default;

            var dictionary = new Dictionary<TKey, TValue>(list.Count);
            foreach (var item in CollectionsMarshal.AsSpan(list))
            {
                dictionary.Add(keyGetter(item), valueGetter(item));
            }

            return dictionary;
        }
    }
}
