namespace _Scripts.Core.ResourceSystem
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/ResourceSystem/ResourceData", fileName = "so_resource_data")]
    public class ResourceDataSO : ScriptableObject
    {
        public Sprite Icon;
        public ResourceType Type;
    }
}
