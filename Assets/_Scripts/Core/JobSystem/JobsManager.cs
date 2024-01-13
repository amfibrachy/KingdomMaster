namespace _Scripts.Core.JobSystem
{
    using System.Collections.Generic;
    using NPC;
    using UI;
    using UnityEngine;

    public class JobsManager : MonoBehaviour
    {
        [SerializeField] private SluggardFSM[] _initialSluggards;
        [SerializeField] private JobEntryScript[] _jobEntries;
        
        
        private List<SluggardFSM> _availableSluggards = new List<SluggardFSM>();
        
        private void Start()
        {
            _availableSluggards.AddRange(_initialSluggards); // TODO Handle case when worker dies (arrays will throw exception)
        }
        
        public void CreateJobRequest()
        {

        }
    }
}
