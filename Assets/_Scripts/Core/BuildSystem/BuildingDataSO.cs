namespace _Scripts.Core.BuildSystem
{
    using System.Collections.Generic;
    using JobSystem;
    using ResourceSystem;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/BuildSystem/BuildingData", fileName = "so_building_data")]
    public class BuildingDataSO : ScriptableObject
    {
        [Header("General")]
        [Space]
        public string Name;
        public BuildingPlacementScript Prefab;
        public BuildingConstructionScript ConstructionPrefab;
        
        [Header("For build")]
        [Space]
        public BuildingType Type;
        public List<ResourceCost> Costs;
        public float MinBuildDistance;
        public float BuildTime;
        public int BuildersNeeded;
        public float BuildingWidth;
        public float BuildingHeight;

        [Header("For jobs")]
        [Space]
        public int TotalPlaces;

        public float JobCreationTime;
        public JobType Job;
        public Sprite JobCreationSprite;
    }

    public enum BuildingType
    {
        Camp,
        Tower,
        House,
        Wall
    }
}
