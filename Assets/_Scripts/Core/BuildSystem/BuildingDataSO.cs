namespace _Scripts.Core.UI.BuildSystem
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/BuildSystem/BuildingData", fileName = "so_building_data")]
    public class BuildingDataSO : ScriptableObject
    {
        public string Name;
        public int Cost;
        public Sprite IconSprite;
        public BuildingPlacementScript Prefab;
    }
}
