using System.Collections.Generic;
using YNL.Utilities.Addons;

public static class Collection
{
    public static Dictionary<K, V> Clone<K, V>(this SerializableDictionary<K, V> origin)
    {
        Dictionary<K, V> clone = new();
        foreach (var pair in origin)
        {
            if (clone.ContainsKey(pair.Key)) clone[pair.Key] = pair.Value;
            else clone.Add(pair.Key, pair.Value);
        }
        return clone;
    }

    public static Dictionary<K, Dictionary<A, B>> Clone<K, A, B>(this SerializableDictionary<K, SerializableDictionary<A, B>> origin)
    {
        Dictionary<K, Dictionary<A, B>> clone = new();
        foreach (var pair in origin)
        {
            Dictionary<A, B> value = new();
            foreach (var pair2 in pair.Value) value.Add(pair2.Key, pair2.Value);

            if (clone.ContainsKey(pair.Key)) clone[pair.Key] = value;
            else clone.Add(pair.Key, value);
        }
        return clone;
    }

    public static SerializableDictionary<K, V> Clone<K, V>(this Dictionary<K, V> origin)
    {
        SerializableDictionary<K, V> clone = new();
        foreach (var pair in origin)
        {
            if (clone.ContainsKey(pair.Key)) clone[pair.Key] = pair.Value;
            else clone.Add(pair.Key, pair.Value);
        }
        return clone;
    }

    public static SerializableDictionary<K, SerializableDictionary<A, B>> Clone<K, A, B>(this Dictionary<K, Dictionary<A, B>> origin)
    {
        SerializableDictionary<K, SerializableDictionary<A, B>> clone = new();
        foreach (var pair in origin)
        {
            SerializableDictionary<A, B> value = new();
            foreach (var pair2 in pair.Value) value.Add(pair2.Key, pair2.Value);

            if (clone.ContainsKey(pair.Key)) clone[pair.Key] = value;
            else clone.Add(pair.Key, value);
        }
        return clone;
    }
}