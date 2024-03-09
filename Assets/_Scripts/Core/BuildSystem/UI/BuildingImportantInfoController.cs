namespace _Scripts.Core.BuildSystem.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class BuildingImportantInfoController : MonoBehaviour
    {
        [SerializeField] private ImportantInfoEntryScript _importantInfoEntry;
        
        [Header("Lumberjack Specifics")]
        [SerializeField] private LayerMask _treesLayer;
        [SerializeField] private int _maxTreesToRaycast = 30;

        public bool HasImportantInfo => _hasImportantInfo; 
        
        // Privates
        private List<ImportantInfoEntryScript> _infos = new List<ImportantInfoEntryScript>();
        private BuildingDataSO _data;
        private bool _hasImportantInfo;
        private Vector3 _currentPosition;
        
        public void InitializeImportantInfos(BuildingDataSO data)
        {
            _data = data;
            _hasImportantInfo = _data.ImportantInfos.Count > 0;
            
            if (!_hasImportantInfo) 
                return;
            
            foreach (var importantInfo in _data.ImportantInfos)
            {
                var newInfo = Instantiate(_importantInfoEntry, transform);
                newInfo.SetText(importantInfo.Text);
                newInfo.SetIcon(importantInfo.Icon);
                newInfo.SetType(importantInfo.InfoType);
                newInfo.SetAmount(0);
                    
                _infos.Add(newInfo);
            }
        }

        private void HandleLumberjackHutTrees()
        {
            var radius = _data.EffectiveRange;
            var colliders = new Collider2D[_maxTreesToRaycast];

            var count = Physics2D.OverlapBoxNonAlloc(_currentPosition, new Vector2(radius, 5f), 0f, colliders,
                _treesLayer);

            if (count >= _maxTreesToRaycast)
            {
                Debug.LogError($"More than {_maxTreesToRaycast} trees are in radius of lumberjack hut");
            }

            foreach (var info in _infos.Where(info => info.InfoType == ImportantInfoType.LumberjackHutTreeCount))
            {
                info.SetAmount(count);
            }
        }

        public void HideInfos()
        {
            gameObject.SetActive(false);
        }
        
        public void ShowInfos()
        {
            gameObject.SetActive(true);
        }

        public void UpdateImportantInfos(Vector3 position)
        {
            _currentPosition = position;
            
            switch (_data.Type)
            {
                case BuildingType.TownCenter:
                    break;
                case BuildingType.ArcherTower:
                    break;
                case BuildingType.VillagerHouse:
                    break;
                case BuildingType.LumberjacksHut:
                    HandleLumberjackHutTrees();
                    
                    break;
                case BuildingType.Wall:
                    break;
                case BuildingType.Blacksmith:
                    break;
                case BuildingType.Windmill:
                    break;
                case BuildingType.MinersShaft:
                    break;
                case BuildingType.FishingDock:
                    break;
                case BuildingType.Eatery:
                    break;
                case BuildingType.EngineersCabin:
                    break;
                case BuildingType.MageTower:
                    break;
                case BuildingType.AlchemistHouse:
                    break;
                case BuildingType.Stockpile:
                    break;
                case BuildingType.HerbalistShack:
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            if (_data != null)
            {
                Gizmos.DrawWireCube(_currentPosition, new Vector2(_data.EffectiveRange, 5f));
            }
        }
    }
}
