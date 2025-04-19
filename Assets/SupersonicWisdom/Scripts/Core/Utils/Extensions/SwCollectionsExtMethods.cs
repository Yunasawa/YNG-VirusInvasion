using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal static class SwCollectionsExtMethods
    {
        #region --- Public Methods ---
        
        /// <summary>
        /// Adds items to the list only if they are not already present.
        /// </summary>
        /// <typeparam name="TValue">The type of the items.</typeparam>
        /// <param name="self">The list to add items to.</param>
        /// <param name="items">The collection of items to be added.</param>
        internal static void SwAddUnique<TValue>(this IList<TValue> self, IEnumerable<TValue> items)
        {
            if (self == null || items == null) return;

            foreach (var item in items)
            {
                if (!self.Contains(item))
                {
                    self.Add(item);
                }
            }
        }

        /// <summary>
        /// This method modifies the stack and sorts it, the first element popped will be the smallest by default (controlled by ascending)
        /// </summary>
        /// <param name="stack">any stack to sort</param>
        /// <param name="ascending">if true the first element popped will be the smallest</param>
        /// <typeparam name="T">could be anything as long as it's comparable</typeparam>
        internal static void SortStack<T>(this Stack<T> stack, bool ascending = true) where T : IComparable<T>
        {
            List<T> list = stack.ToList();
            list.Sort();

            if (ascending)
            {
                list.Reverse();
            }
            
            stack.Clear();
            foreach (T element in list)
            {
                stack.Push(element);
            }
        }
        
        internal static void SwAdd<TValue>(this HashSet<TValue> self, TValue addition)
        {
            self.Add(addition);
        }

        internal static void SwForEach<TValue>(this IEnumerable<TValue> self, Action<TValue> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }
        
        internal static void SwAddAll<TValue>(this IList<TValue> self, IEnumerable<TValue> other)
        {
            other.SwForEach(self.Add);
        }

        internal static bool SwIsNullOrEmpty(this ICollection self)
        {
            return self == null || self.Count == 0;
        }
        
        internal static TSource FirstOr<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
        {
            var result = defaultValue;

            try
            {
                result = source.First(predicate);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogWarning(EWisdomLogType.Utils, $"{nameof(FirstOr)}");
            }

            return result;
        }

        /// <summary>
        /// Safely inserts a value into a list within a dictionary. 
        /// If the key does not exist in the dictionary, a new list is created for that key.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the list.</typeparam>
        /// <param name="self">The dictionary to insert the value into.</param>
        /// <param name="key">The key to identify the list within the dictionary.</param>
        /// <param name="value">The value to be added to the list.</param>
        /// <returns>
        /// True if the insertion was successful; false if the key is null.
        /// </returns>
        internal static bool SwSafeInsertToList<TKey, TValue>(this Dictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            if (key == null) return false;

            if (!self.ContainsKey(key))
            {
                self[key] = new List<TValue>();
            }
            
            self[key].Add(value);

            return true;
        }
        
        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static bool SwAddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (key == null) return false;

            if (self.ContainsKey(key))
            {
                self.Remove(key);
            }

            self.Add(key, value);

            return true;
        }

        /// <summary>
        ///     Merge dictionaries extension
        ///     The last source keys overrides the first source keys
        /// </summary>
        /// <param name="self"></param>
        /// <param name="overwriteValue"></param>
        /// <param name="sources"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static Dictionary<TKey, TValue> SwMerge<TKey, TValue>(this Dictionary<TKey, TValue> self, bool overwriteValue, params Dictionary<TKey, TValue>[] sources)
        {
            foreach (var source in sources)
            {
                if (source != null)
                {
                    foreach (var keyValuePair in source)
                    {
                        if (overwriteValue || !self.ContainsKey(keyValuePair.Key))
                        {
                            self[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }

            return self;
        }
        
        /// <summary>
        /// Replaces the values of existing keys in the dictionary with values from the provided source dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="self">The dictionary whose values will be replaced.</param>
        /// <param name="sources">An array of dictionaries containing key-value pairs to replace in the original dictionary.</param>
        /// <returns>The original dictionary with updated values for existing keys.</returns>
        internal static Dictionary<TKey, TValue> SwReplaceExistingKeys<TKey, TValue>(this Dictionary<TKey, TValue> self, params Dictionary<TKey, TValue>[] sources)
        {
            foreach (var source in sources)
            {
                if (source != null)
                {
                    foreach (var keyValuePair in source)
                    {
                        if (self.ContainsKey(keyValuePair.Key))
                        {
                            self[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }

            return self;
        }
        
        internal static Dictionary<TKey, TValue> RemoveNullValues<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            if (self == null)
            {
                    return null;
            }

            var keysToRemove = new List<TKey>();

            foreach (var data in self)
            {
                if (data.Value == null)
                {
                    keysToRemove.Add(data.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                self.Remove(key);
            }

            return self;
        }
        
        internal static bool SwContains<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
        {
            return key != null && self != null && self.ContainsKey(key);
        }

        /// <summary>
        /// Replaces the value associated with a specified key in the dictionary if the value type matches the expected type.
        /// Logs messages based on the success or failure of the operation.
        /// </summary>
        /// <typeparam name="T">The type of the value to replace in the dictionary.</typeparam>
        /// <param name="dict">The dictionary containing key-value pairs where the value is of type object.</param>
        /// <param name="stringKey">The key whose associated value is to be replaced.</param>
        /// <param name="value">The new value to set for the specified key.</param>
        /// <param name="logType">The type of log to use for logging messages (default is EWisdomLogType.Utils).</param>
        /// <returns>
        /// True if the key is not found in the dictionary; false if the operation proceeds (regardless of success or failure).
        /// </returns>
        internal static bool ReplaceValueForSpecificValueType<T>(this Dictionary<string, object> dict, string stringKey, T value, EWisdomLogType logType = EWisdomLogType.Utils)
        {
            if (!dict.SwContains(stringKey))
            {
                SwInfra.Logger.Log(logType, $"Key is not defined: Key = {stringKey}");
                SwApplicationUtils.QuitApplicationOnDevelopment();

                return false;
            }

            if (value is int intValue)
            {
                if (dict[stringKey] is int or long)
                {
                    dict[stringKey] = intValue;
                    SwInfra.Logger.Log(logType, $"Set int data: Key = {stringKey}, Value = {intValue}");
                }
                else
                {
                    SwInfra.Logger.LogError(logType, $"Invalid type! You Tried to set int value for key = {stringKey}");
                    SwApplicationUtils.QuitApplicationOnDevelopment();
                    
                    return false;
                }
            }
            else if (value is bool boolValue)
            {
                if (dict[stringKey] is bool)
                {
                    dict[stringKey] = boolValue;
                    SwInfra.Logger.Log(logType, $"Set bool data: Key = {stringKey}, Value = {boolValue}");
                }
                else
                {
                    SwInfra.Logger.LogError(logType, $"Invalid type! You Tried to set bool value for key = {stringKey}");
                    SwApplicationUtils.QuitApplicationOnDevelopment();
                    
                    return false;
                }
            }
            else if (value is float floatValue)
            {
                if (dict[stringKey] is float or double)
                {
                    dict[stringKey] = floatValue;
                    SwInfra.Logger.Log(logType, $"Set float data: Key = {stringKey}, Value = {floatValue}");
                }
                else
                {
                    SwInfra.Logger.LogError(logType, $"Invalid type! You Tried to set float value for key = {stringKey}");
                    SwApplicationUtils.QuitApplicationOnDevelopment();
                    
                    return false;
                }
            }
            else if (value is string stringValue)
            {
                if (dict[stringKey] is string)
                {
                    dict[stringKey] = stringValue;
                    SwInfra.Logger.Log(logType, $"Set string data: Key = {stringKey}, Value = {stringValue}");
                }
                else
                {
                    SwInfra.Logger.LogError(logType, $"Invalid type! You Tried to set string value for key = {stringKey}");
                    SwApplicationUtils.QuitApplicationOnDevelopment();
                    
                    return false;
                }
            }
            else
            {
                SwInfra.Logger.Log(logType, $"Unsupported type: {typeof(T)}");
                SwApplicationUtils.QuitApplicationOnDevelopment();
                
                return false;
            }

            return true;
        }
        
        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static TValue SwSafelyGet<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, [CanBeNull] TValue defaultValue)
        {
            if (self == null || key == null) return defaultValue;
            if (!self.ContainsKey(key)) return defaultValue;
            self.TryGetValue(key, out var value);

            return value == null ? defaultValue : value;
        }

        internal static string SwToString<TSource>(this IEnumerable<TSource> source)
        {
            return SwUtils.JsonHandler.SerializeObject(source);
        }
        
        internal static int SwToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        internal static bool SwToBool(this int i)
        {
            return i == 1;
        }
        
        internal static HashSet<TSource> SwToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return new HashSet<TSource>(source);
        }
        
        internal static TSource SwSafelyGet<TSource>(this IEnumerable<TSource> source, int index, TSource defaultValue)
        {
            if (source == null) return defaultValue;
            var arrOrNull = source as TSource[];
            var arr = arrOrNull ?? source.ToArray();

            return arr.Length > index ? arr[index] : defaultValue;
        }

        internal static string SwToJsonString(this Dictionary<string, object> self)
        {
            return SwUtils.JsonHandler.SerializeObject(self);
        }

        public static void FillWith<TEnum, S>(this IDictionary<TEnum, S> dict, S value) where TEnum : struct, Enum
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                try
                {
                    dict[enumValue] = value;
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogException(e, EWisdomLogType.Utils, $"{nameof(FillWith)}");
                }
            }
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation using the formatting conventions of the invariant culture.
        /// Applicable for all types implementing IConvertible.
        /// </summary>
        /// <param name="value">The object to be converted to string</param>
        /// <returns>A string representation of value, formatted by the format specification of the invariant culture.</returns>
        public static string SwToString<T>(this T value) where T : IConvertible
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        
        
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation using the specified format and the formatting conventions of the invariant culture.
        /// </summary>
        /// <param name="value"> This is the value you want converted </param>
        /// <param name="format">Format in a string form, like "R" </param>
        /// <typeparam name="T">Has to be formattable</typeparam>
        /// <returns>Formatted string</returns>
        public static string SwToString<T>(this T value, string format) where T : IFormattable
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
        
        public static string SubstringUntilUpperLetter(this string self)
        {
            for (var i = 1; i < self.Length; i++)
            {
                if (char.IsUpper(self[i]))
                {
                    return self.Substring(0, i);
                }
            }

            return self;
        }

        #endregion
    }
}