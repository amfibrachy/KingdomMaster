namespace _Scripts.Core.JobSystem
{
    using System;
    using System.Collections.Generic;
    using BuildSystem;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class JobProgressController : MonoBehaviour
    {
        [SerializeField] private JobProgressScript jobProgressPrefab;

        public event Action OnJobCreationFinished;
        
        // Privates
        private Queue<JobProgressScript> _pendingJobs = new Queue<JobProgressScript>();
        private JobProgressScript _current;
        private bool _isCreating;
        
        public void AddJobCreationIconTask(JobType job, BuildingDataSO data)
        {
            var newJob = Instantiate(jobProgressPrefab, transform); // TODO can be refactored for pool if needed later
            newJob.Initialize(data);
            
            _pendingJobs.Enqueue(newJob);
        }

        private void Update()
        {
            if (!_isCreating)
            {
                TryStartNextJobCreation();
            }
        }

        private void TryStartNextJobCreation()
        {
            if (_pendingJobs.Count > 0)
            {
                _current = _pendingJobs.Dequeue();
                _current.OnCreated += CreationFinished;
                _current.StartCreation();
                _isCreating = true;
            }
        }

        private void CreationFinished()
        {
            _current.OnCreated -= CreationFinished;
            Destroy(_current.gameObject);

            OnJobCreationFinished?.Invoke();
            _isCreating = false;
        }
    }
}
