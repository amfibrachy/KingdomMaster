namespace _Scripts.Core.Global
{
    using AI;
    using JobSystem;
    using JobSystem.UI;
    using UnityEngine;

    public class UIUpdateController : MonoBehaviour
    {
        [SerializeField] private JobSystemUIControllerScript _jobSystemUiController;

        public void UpdateUI(AgentType agent, JobType job)
        {
            _jobSystemUiController.UpdateJobUI(job);
        }
    }
}
