namespace _Scripts.Core.BuildSystem
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/BuildSystem/BuildingData", fileName = "so_building_data")]
    public class BuildingDataSO : ScriptableObject
    {
        public string Name;
        public int Cost;
        public float BuildTime;
        public int BuildersNeeded;
        public Sprite IconSprite;
        
        public BuildingPlacementScript Prefab;
        public BuildingConstructionScript ConstructionPrefab;
        
        public float MinBuildDistance;
        public BuildingType Type;

        public float BuildingWidth;
        public float BuildingHeight;
    }

    public enum BuildingType
    {
        Camp,
        Tower,
        House,
        Wall
    }
}
