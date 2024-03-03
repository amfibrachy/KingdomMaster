namespace _Scripts.Utils
{
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public static class Util
    {
        public static T[] GetActiveChildComponents<T>(Transform parent) where T : Component
        {
            var childCount = parent.childCount;
            var activeChildren = new List<T>(childCount);

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                T component = child.GetComponent<T>();

                if (child.gameObject.activeSelf && component != null)
                {
                    activeChildren.Add(component);
                }
            }

            return activeChildren.ToArray();
        }

        public static bool IsMouseOverUI()
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        
        public static T Choose<T>(params T[] options)
        {
            if (options == null || options.Length == 0)
            {
                throw new System.ArgumentException("Options array cannot be null or empty");
            }

            int index = Random.Range(0, options.Length);
            return options[index];
        }
    }
}
