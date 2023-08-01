namespace _Scripts.Core.UI.BuildSystem
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/BuildSystem/BuildingData", fileName = "so_building_data")]
    public class BuildingDataSO : ScriptableObject
    {
        public string Name;
        public int Cost;
        public float BuildTime;
        public Sprite IconSprite;
        
        public BuildingPlacementScript Prefab;
        public BuildingConstructionScript ConstructionPrefab;
        
        public float MinBuildDistance;
        public BuildingType Type;
    }

    public enum BuildingType
    {
        Tower,
        House,
        Wall
    }
}
