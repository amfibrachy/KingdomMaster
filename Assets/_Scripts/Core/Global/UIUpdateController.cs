namespace _Scripts.Core.Global
{
    using AI;
    using JobSystem;
    using JobSystem.UI;
    using UnityEngine;

    public class UIUpdateController : MonoBehaviour
    {
        [SerializeField] private JobSystemUIControllerScript _jobSystemUiController;

        public void UpdateCreateUI(AgentType agent, JobType job)
        {
            _jobSystemUiController.UpdateJobUIOnCreate(job);
        }
        
        public void UpdateDispatchUI<T>(AgentType agent, FSM<T> npc, JobType job) where T : IFSM<T>
        {
            _jobSystemUiController.UpdateJobUIOnDispatch(npc, job);
        }
    }
}
