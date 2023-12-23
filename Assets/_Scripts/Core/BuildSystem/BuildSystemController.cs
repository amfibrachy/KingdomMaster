namespace _Scripts.Core.BuildSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BuildSystemController : MonoBehaviour
    {
        [SerializeField] private PlacementSystemScript _placementSystem;
        [SerializeField] private Transform _buildingsParent;

        private List<BuildingConstructionScript> _constructionsActive = new List<BuildingConstructionScript>();
        private Queue<BuildingConstructionScript> _constructionsQueue = new Queue<BuildingConstructionScript>();
        private List<BuildingConstructionScript> _constructionsFinished = new List<BuildingConstructionScript>();

        private bool _isProcessing;

        private void Awake()
        {
            _placementSystem.OnBuildingPlaced += PlaceConstructionSiteAndEnqueueRequest;
        }
        
        private void PlaceConstructionSiteAndEnqueueRequest(BuildingDataSO building, Vector2 position)
        {
            var constructionSite = Instantiate(building.ConstructionPrefab, position, Quaternion.identity, _buildingsParent);
            constructionSite.SetData(building);
            
            _constructionsQueue.Enqueue(constructionSite);
        }

        private void Update()
        {
            if (!_isProcessing)
            {
                UpdateConstructionList();

                if (_constructionsActive.Count > 0)
                {
                    StartCoroutine(ProcessBuildings());
                }
            }
        }

        private void UpdateConstructionList()
        {
            while (_constructionsQueue.Count > 0)
            {
                _constructionsActive.Add(_constructionsQueue.Dequeue());
            }
        }
        
        private IEnumerator ProcessBuildings()
        {
            _isProcessing = true;

            foreach (var construction in _constructionsActive)
            {
                construction.AddProgress(15f);
                
                if (construction.IsConstructionFinished())
                {
                    var constructionPosition = construction.transform.position;

                    Instantiate(construction.GetBuildingPrefab(), constructionPosition, Quaternion.identity, _buildingsParent);
                    _constructionsFinished.Add(construction);
                }
            }

            foreach (var construction in _constructionsFinished)
            {
                _constructionsActive.Remove(construction);
                Destroy(construction.gameObject);
            }
            
            _constructionsFinished.Clear();
            
            yield return new WaitForSeconds(1f);

            _isProcessing = false;
        }
    }
}
