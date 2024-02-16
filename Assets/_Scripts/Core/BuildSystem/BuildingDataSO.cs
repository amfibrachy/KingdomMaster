namespace _Scripts.Core.BuildSystem
{
    using System.Collections.Generic;
    using JobSystem;
    using ResourceSystem;
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu(menuName = "Scriptable Objects/BuildSystem/BuildingData", fileName = "so_building_data")]
    public class BuildingDataSO : ScriptableObject
    {
        public BuildingPlacementScript Prefab;
        public BuildingConstructionScript ConstructionPrefab;
        
        [Header("General")]
        [Space]
        public string Name;
        public string Description;
        public int HP;
        public int ResidentsCapacity;
        
        [Header("For build")]
        [Space]
        public BuildingType Type;
        public List<ResourceCost> Costs;
        public float MinBuildDistance;
        public int BuildTime;
        public int MaxBuildersAmount;
        public float BuildingWidth;
        public float BuildingHeight;
        
        [Header("For jobs")]
        [Space]
        public Sprite JobSprite;
        public JobType Job;
        public int TrainingCapacity;
        public int TrainingTime;
    }

    public enum BuildingType
    {
        Camp,
        Tower,
        House,
        Wall
    }
}
