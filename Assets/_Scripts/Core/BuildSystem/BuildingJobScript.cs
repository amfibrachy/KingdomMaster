namespace _Scripts.Core.BuildSystem
{
    using System;
    using System.Collections.Generic;
    using JobSystem;
    using NPC;
    using UnityEngine;

    [RequireComponent(typeof(BuildingPlacementScript))]
    public class BuildingJobScript : MonoBehaviour
    {
        [SerializeField] private Transform _entrancePosition;

        // Privates
        private List<SluggardFSM> _assignedSluggards = new List<SluggardFSM>(); 
        private List<SluggardFSM> _learningSluggards = new List<SluggardFSM>();
        
        private BuildingDataSO _data;

        public Transform EntrancePosition => _entrancePosition;

        public JobType? Job { private set; get; }
        
        public int TotalFreePlaces => _data.TotalPlaces;
        public int CurrentFreePlaces { private set; get; }

        public void Initialize(BuildingDataSO data)
        {
            _data = data;
            Job = GetJobTypeByBuildingType();
            CurrentFreePlaces = TotalFreePlaces;
        }
        
        public void AddSluggardJobCreationTask(SluggardFSM sluggard)
        {
            if (CurrentFreePlaces > 0)
            {
                _assignedSluggards.Add(sluggard);
                CurrentFreePlaces--;
            }
        }
        
        public void StartSluggardJobCreationTask(JobType job)
        {
            // TODO
            int a = 66;
        }

        private JobType? GetJobTypeByBuildingType()
        {
            BuildingType type = _data.Type;

            return type switch
            {
                BuildingType.Camp => JobType.Builder,
                _ => null
            };
        }
    }
}
