namespace _Scripts.Core.ResourceSystem
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ResourceCost
    {
        public Sprite ResourceIcon;
        public ResourceType Resource;
        public int Amount;
    }
}
