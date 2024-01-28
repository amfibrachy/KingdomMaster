namespace _Scripts.Core.JobSystem
{
    using System;
    using System.Collections.Generic;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using NPC;
    using UnityEngine;
    using Utils.Debugging;

    public class SluggardsManager : MonoBehaviour, IDispatchable
    {
        [SerializeField] private BuildingsManager _buildingsManager;
        [SerializeField] private SluggardFSM[] _initialSluggards;
        
        private List<SluggardFSM> _allSluggards = new List<SluggardFSM>();
        
        private List<SluggardFSM> _sluggardsActiveList = new List<SluggardFSM>();
        private List<SluggardFSM> _availableSluggards = new List<SluggardFSM>();
        
        public int Population { get; set; }
        public int SluggardCount => _availableSluggards.Count;

        public event Action OnAvailableSluggardsChanged;
        
        // Injectables
        [Inject] private IDebug _debug;

        private void Start()
        {
            _availableSluggards.AddRange(_initialSluggards); // TODO Handle case when worker dies (arrays will throw exception)
            _allSluggards.AddRange(_availableSluggards);
        }
        
        public void CreateJobRequests(Dictionary<JobType, int> requests)
        { 
            foreach (var (job, jobsCount) in requests)
            {
                if (_availableSluggards.Count < jobsCount)
                {
                    _debug.LogError($"For {job} request => _availableSluggards.Count is {_availableSluggards.Count}, while job request count is {jobsCount}");
                }
                
                var sluggardsToAssign = _availableSluggards.GetRange(0, jobsCount);
                _availableSluggards.RemoveRange(0, jobsCount);
                OnAvailableSluggardsChanged?.Invoke();
                _sluggardsActiveList.AddRange(sluggardsToAssign);

                var buildings = _buildingsManager.GetFreeBuildings(job); // TODO optimize to raycast once for all jobs in requests

                int index = 0;
                
                foreach (var building in buildings)
                {
                    while (index < sluggardsToAssign.Count && building.CurrentFreePlaces > 0)
                    {
                        building.AddSluggardJobCreationTask(sluggardsToAssign[index]);
                        sluggardsToAssign[index].SetJobTask(building, job);
                        index++;
                    }

                    if (index == sluggardsToAssign.Count)
                        break;
                }
            }
        }

        private void Update()
        {
            // TODO check for sluggards that reached destination and fire event for UI update
        }

        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            _allSluggards.Remove(fsm as SluggardFSM);
            int k = 5;
        }
    }
}
