namespace _Scripts.Core.JobSystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using NPC;
    using UnityEngine;
    using Utils;
    using Utils.Debugging;

    public class SluggardsManager : MonoBehaviour, IDispatchable, ICountable
    {
        private List<SluggardFSM> _allSluggards = new List<SluggardFSM>();
        private List<SluggardFSM> _availableSluggards = new List<SluggardFSM>();
        private List<SluggardFSM> _pendingSluggards = new List<SluggardFSM>();

        private Dictionary<JobType, int> _pendingRequests = new Dictionary<JobType, int>();
        
        public int Population { get; set; }
        public int Count => _availableSluggards.Count;

        public event Action OnAvailableSluggardsChanged;
        
        // Injectables
        [Inject(Id = "SluggardsParent")] private Transform _sluggardsParent;
        [Inject] private IDebug _debug;
        
        private BuildingsManager _buildingsManager;

        [Inject]
        public void Construct(BuildingsManager buildingsManager)
        {
            _buildingsManager = buildingsManager;
        }
        
        private void Start()
        {
            var initialSluggards = Util.GetActiveChildComponents<SluggardFSM>(_sluggardsParent);

            if (initialSluggards != null)
            {
                _availableSluggards.AddRange(initialSluggards); // TODO Handle case when builder dies (arrays will throw exception)
            }
            
            _allSluggards.AddRange(_availableSluggards);
            
            StartCoroutine(CheckPendingRequests());
        }
        
        public void CreateJobRequests(Dictionary<JobType, int> requests)
        { 
            var keys = new List<JobType>(requests.Keys);
            
            for (int i = 0; i < keys.Count; i++) 
            {
                var job = keys[i];
                var jobsCount = requests[job];
                
                if (_availableSluggards.Count < jobsCount)
                {
                    _debug.LogError($"For {job} request => _availableSluggards.Count is {_availableSluggards.Count}, while job request count is {jobsCount}");
                    return;
                }
                
                var sluggardsToAssign = _availableSluggards.GetRange(0, jobsCount);
                var buildings = _buildingsManager.GetFreeBuildings(job); // TODO optimize to raycast once for all jobs in requests

                int index = 0;
                
                foreach (var building in buildings)
                {
                    while (index < sluggardsToAssign.Count && building.CurrentFreePlaces > 0)
                    {
                        building.ReserveFreePlace();
                        sluggardsToAssign[index].SetJobTask(building, job);
                        requests[job]--;
                        index++;
                    }
                    
                    if (index == sluggardsToAssign.Count)
                    {
                        break;
                    }
                }

                if (index < sluggardsToAssign.Count)
                {
                    var unassignedJobsLeft = sluggardsToAssign.Count - index;
                    _pendingSluggards = _pendingSluggards.Concat(sluggardsToAssign.TakeLast(unassignedJobsLeft)).ToList();
                    
                    if (_pendingRequests.ContainsKey(job))
                    {
                        _pendingRequests[job] += unassignedJobsLeft;
                    }
                    else
                    {
                        _pendingRequests.Add(job, unassignedJobsLeft);
                    }
                }
                
                _availableSluggards.RemoveRange(0, sluggardsToAssign.Count);
                OnAvailableSluggardsChanged?.Invoke();
            }
        }

        private void TryCreatePendingJobRequests()
        { 
            var keys = new List<JobType>(_pendingRequests.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                var job = keys[i];
                var jobsCount = _pendingRequests[job];

                if (_pendingSluggards.Count < jobsCount)
                {
                    _debug.LogError($"For {job} request => _pendingSluggards.Count is {_pendingSluggards.Count}, while job request count is {jobsCount}");
                    return;
                }

                var sluggardsToAssign = _pendingSluggards.GetRange(0, jobsCount);
                var buildings = _buildingsManager.GetFreeBuildings(job); // TODO optimize to raycast once for all jobs in requests

                int index = 0;

                foreach (var building in buildings)
                {
                    while (index < sluggardsToAssign.Count && building.CurrentFreePlaces > 0)
                    {
                        building.ReserveFreePlace();
                        sluggardsToAssign[index].SetJobTask(building, job);
                        _pendingRequests[job]--;
                        index++;
                    }

                    if (index == sluggardsToAssign.Count)
                    {
                        break;
                    }
                }

                if (index > 0)
                {
                    _pendingSluggards.RemoveRange(0, index);
                }

                if (_pendingRequests[job] == 0)
                {
                    _pendingRequests.Remove(job);
                }
            }
        }
        
        private IEnumerator CheckPendingRequests()
        {
            while (true)
            {
                if (_pendingRequests.Count > 0)
                {
                    TryCreatePendingJobRequests();
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private void Update()
        {
            // TODO check for sluggards that reached destination and fire event for UI update
        }

        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            _allSluggards.Remove(fsm as SluggardFSM);
        }
    }
}
