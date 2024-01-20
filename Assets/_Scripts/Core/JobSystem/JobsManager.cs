namespace _Scripts.Core.JobSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using NPC;
    using UI;
    using UnityEngine;

    public class JobsManager : MonoBehaviour
    {
        [SerializeField] private SluggardFSM[] _initialSluggards;
        
        public int Population { get; set; }
        public int SluggardCount => _availableSluggards.Count;
        
        private List<SluggardFSM> _availableSluggards = new List<SluggardFSM>();
        
        private void Start()
        {
            _availableSluggards.AddRange(_initialSluggards); // TODO Handle case when worker dies (arrays will throw exception)
        }
        
        public void CreateJobsRequest(Dictionary<JobType, int> requests)
        {
            for (int i = 0; i < requests.Values.Sum(); i++)
            {
                _availableSluggards.RemoveAt(0);
            }
        }
    }
}
