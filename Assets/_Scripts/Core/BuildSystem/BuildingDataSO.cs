namespace _Scripts.Core.BuildSystem
{
    using System.Collections.Generic;
    using JobSystem;
    using ResourceSystem;
    using UnityEngine;

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
        
        [Header("Building info")]
        [Space]
        public BuildingType Type;
        public List<ResourceCost> Costs;
        public float MinBuildDistance;
        public float BuildingWidth;
        public float BuildingHeight;
        
        [Header("For build")]
        [Space]
        public int MaxBuildersAmount;
        public int BuildTime;
        
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
        ArcherTower,
        VillagerHouse,
        Wall,
        Blacksmith,
        Windmill,
        MinersShaft,
        FishingDock,
        Eatery,
        EngineersCabin,
        MageTower,
        AlchemistHouse,
        Stockpile,
        HerbalistShack,
    }
}
