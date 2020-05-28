using Epitome.Utility;
using UnityEngine;

namespace Epitome
{
    public static class TransformExtensions
    {
        public static void SetPosX(this Transform self,float x)
        {
            Vector3 oldPos = self.position;
            self.position = new Vector3(x, oldPos.y, oldPos.z);
        }

        public static void SetPosY(this Transform self, float y)
        {
            Vector3 oldPos = self.position;
            self.position = new Vector3(oldPos.x, y, oldPos.z);
        }

        public static void SetPosZ(this Transform self, float z)
        {
            Vector3 oldPos = self.position;
            self.position = new Vector3(oldPos.x, oldPos.y, z);
        }

        public static float GetPosX(this Transform self)
        {
            return self.position.x;
        }

        public static float GetPosY(this Transform self)
        {
            return self.position.y;
        }

        public static float GetPosZ(this Transform self)
        {
            return self.position.z;
        }

        /// <summary>
        /// Activates/Deactivates the Transform.
        /// </summary>
        public static void SetActive(this Transform self, bool state) { self.gameObject.SetActive(state); }

        /// <summary>
        /// The local active state of this Transform. (Read Only).
        /// </summary>
        public static bool ActiveSelf(this Transform self) { return self.gameObject.activeSelf; }

        public static T[] GetComponentsInChildren<T, Culling>(this Transform self)
        {
            T[] result = new T[] { };
            result = Data.MergeArray(result, self.GetComponents<T>());
            for (int i = 0; i < self.childCount; i++)
            {
                Transform child = self.GetChild(i);
                Culling culling = child.GetComponent<Culling>();

                if (culling != null) continue;

                result = Data.MergeArray(result, child.GetComponentsInChildren<T, Culling>());
            }

            return result;
        }

        public static T GetComponentInChildren<T, Culling>(this Transform self)
        {
            T[] ts = self.GetComponentsInChildren<T, Culling>();
            if (ts != null && ts.Length > 0) return ts[0];
            return default(T);
        }
    }
}
