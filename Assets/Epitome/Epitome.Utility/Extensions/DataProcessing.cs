using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epitome
{
    public static class DataProcessing
    {
        // ============ 字典扩展

        public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey[] keys)
        {
            return CompareLists<TKey>(dic.Keys.Intersect(keys).ToList(), keys.ToList());
        }

        public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
        {
            if (aListA == null || aListB == null || aListA.Count != aListB.Count)
                return false;
            if (aListA.Count == 0)
                return true;
            Dictionary<T, int> lookUp = new Dictionary<T, int>();
            // create index for the first list
            for (int i = 0; i < aListA.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListA[i], out count))
                {
                    lookUp.Add(aListA[i], 1);
                    continue;
                }
                lookUp[aListA[i]] = count + 1;
            }
            for (int i = 0; i < aListB.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListB[i], out count))
                {
                    // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                    return false;
                }
                count--;
                if (count <= 0)
                    lookUp.Remove(aListB[i]);
                else
                    lookUp[aListB[i]] = count;
            }
            // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
            return lookUp.Count == 0;
        }

        public static TValue[] GetValues<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey[] keys)
        {
            List<TValue> values = new List<TValue>();

            TValue value;
            for (int i = 0; i < keys.Length; i++)
            {
                if (dic.TryGetValue(keys[i],out value))
                {
                    values.Add(value);
                }
            }

            return values.ToArray();
        }

        public static T GetMax<T>(this IEnumerable<T> gather)
        {
            return gather.Max();
        }

        public static T GetMin<T>(this IEnumerable<T> gather)
        {
            return gather.Min();
        }
    }
}
