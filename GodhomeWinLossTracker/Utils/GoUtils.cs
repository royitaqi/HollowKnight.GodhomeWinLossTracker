using UnityEngine;

namespace GodhomeWinLossTracker.Utils
{
    class GoTag : UnityEngine.MonoBehaviour
    {
        public string DamageSource;
        public string DamageSourceDetail;
    }

    static class GoTagExtension
    {
        // Set the GoTag into GO's root.
        public static GameObject SetGoTag(this GameObject go, string damageSource, string damageSourceDetail)
        {
            var root = FindRoot(go);
            var existingTag = root.GetComponent<GoTag>() ?? root.AddComponent<GoTag>();
            existingTag.DamageSource = damageSource;
            existingTag.DamageSourceDetail = damageSourceDetail;
            return go;
        }

        // Find the potential GoTag in GO's root.
        public static GoTag GetGoTag(this GameObject go)
        {
            return FindRoot(go).GetComponent<GoTag>();
        }

        private static GameObject FindRoot(GameObject go)
        {
            GameObject root = null;
            while (go != null)
            {
                root = go;
                go = go.transform?.parent?.gameObject;
            }
            return root;
        }
    }
}
