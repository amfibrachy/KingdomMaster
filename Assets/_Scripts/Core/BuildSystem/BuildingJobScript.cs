namespace _Scripts.Core.BuildSystem
{
    using System.Collections.Generic;
    using AI;
    using Global;
    using global::Zenject;
    using JobSystem;
    using NPC;
    using UnityEngine;

    [RequireComponent(typeof(BuildingPlacementScript))]
    public class BuildingJobScript : MonoBehaviour
    {
        [SerializeField] private Transform _entrancePosition;
        [SerializeField] private JobProgressController _jobProgressController;

        // Injectables
        [Inject] protected PopulationController _populationController;

        private BuildingDataSO _data;

        public Transform EntrancePosition => _entrancePosition;

        public JobType Job => _data.Job;
        
        public int TotalFreePlaces => _data.TotalPlaces;
        public int CurrentFreePlaces { private set; get; }

        private void Awake()
        {
            _jobProgressController.OnJobCreationFinished += JobCreated;
        }

        public void Initialize(BuildingDataSO data)
        {
            _data = data;
            CurrentFreePlaces = TotalFreePlaces;
        }
        
        public void ReserveFreePlace()
        {
            if (CurrentFreePlaces > 0)
            {
                CurrentFreePlaces--;
            }
        }
        
        public void StartSluggardJobCreationTask(JobType job)
        {
            _jobProgressController.AddJobCreationIconTask(job, _data);
        }
        
        private void JobCreated()
        {
            CurrentFreePlaces++;
            _populationController.OnCreate(AgentType.WithJob, EntrancePosition.position, _data.Job);
        }
    }
}
