using System;
using System.Security.Cryptography;
using System.Text;

namespace SupersonicWisdomSDK
{
    internal class SwCryptoUtils
    {
        /// <summary>
        /// Generates a deterministic bucket group index based on a prefix and key, using MD5 hashing and modular arithmetic.
        /// </summary>
        /// <param name="prefix">The prefix string to be combined with the key.</param>
        /// <param name="key">The key string to be combined with the prefix.</param>
        /// <param name="max">The maximum value for the bucket group index (non-inclusive).</param>
        /// <returns>An integer representing the bucket group index, deterministically generated within the range [0, max).</returns>
        internal int GetBucketGroup(string prefix, string key, int max)
        {
            using var md5 = MD5.Create();
            var finalKey = prefix + key;
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(finalKey));
            var numericHash = BitConverter.ToUInt64(hashBytes, 0);
            var result = (int)(numericHash % (ulong)max);

            return result;
        }

    }
}