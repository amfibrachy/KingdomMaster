namespace _Scripts.Core.JobSystem.UI
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class JobSystemUIControllerScript : MonoBehaviour
    {
        [FormerlySerializedAs("jobsSystemController")] [SerializeField] private JobsManager jobsManager;
        
        [SerializeField] private CanvasGroup _jobsSystemCanvasGroup;
        [SerializeField] private RectTransform _jobsPanelTransform;
        [SerializeField] private float _outsidePosX = 455f;
        
        public bool IsShown { private set; get; }

        public void ShowPanel()
        {
            _jobsSystemCanvasGroup.DOFade(1f, 0.15f);
            _jobsPanelTransform.DOAnchorPosX(0f, 0.3f).OnComplete(() => IsShown = true);
        }

        public void HidePanel()
        {
            _jobsSystemCanvasGroup.DOFade(0f, 0.15f);
            _jobsPanelTransform.DOAnchorPosX(_outsidePosX, 0.3f).OnComplete(() => IsShown = false);
        }
        
        private void OnJobIncreaseClicked(int index)
        {
            // TODO Pass which job was clicked

            jobsManager.CreateJobRequest();
        }
    }
}
