namespace _Scripts.Core.Global
{
    using System.Collections.Generic;
    using BuildSystem;
    using BuildSystem.TownCenter;
    using global::Zenject;
    using UnityEngine;
    using Utils;
    using Utils.Debugging;

    public class KingdomBordersController : MonoBehaviour
    {
        public Vector2 TownCenterPosition => _townCenterPosition;
        public Vector2 LeftBorderPosition => _wallsLeft.Count > 0 ? _wallsLeft.Max : _townCenterPosition;
        public Vector2 RightBorderPosition => _wallsRight.Count > 0 ? _wallsRight.Max : _townCenterPosition;
        
        // Injectables
        [Inject(Id = "WallsParent")] private Transform _wallsParent;
        [Inject] private IDebug _debug;
        [Inject] private TownCenterScript _townCenter;
        
        // Privates
        private SortedSet<Vector2> _wallsRight;
        private SortedSet<Vector2> _wallsLeft;
        private Vector2 _townCenterPosition;
        
        private void Awake()
        {
            _townCenterPosition = _townCenter.transform.position;
            var initialWalls = Util.GetActiveChildComponents<Transform>(_wallsParent);

            if (initialWalls == null || initialWalls.Length != 2)
            {
                _debug.LogError($"There are not 2 initial walls on the scene");
                return;
            }

            _wallsRight = new SortedSet<Vector2>(Comparer<Vector2>.Create((position1, position2) => {
                var distance1 = Vector2.Distance(_townCenterPosition, position1);
                var distance2 = Vector2.Distance(_townCenterPosition, position2);

                if (distance1 < distance2)
                    return -1;
                    
                if (distance1 > distance2)
                    return 1;
                    
                return 0;
            }));
            
            _wallsLeft = new SortedSet<Vector2>(Comparer<Vector2>.Create((position1, position2) => {
                var distance1 = Vector2.Distance(_townCenterPosition, position1);
                var distance2 = Vector2.Distance(_townCenterPosition, position2);

                if (distance1 < distance2)
                    return -1;
                    
                if (distance1 > distance2)
                    return 1;
                    
                return 0;
            }));

            if (initialWalls[0].position.x > initialWalls[1].position.x)
            {
                AddRightWall(initialWalls[0].position);
                AddLeftWall(initialWalls[1].position);
            }
            else
            {
                AddRightWall(initialWalls[1].position);
                AddLeftWall(initialWalls[0].position);
            }
        }

        private void AddLeftWall(Vector3 newWall)
        {
            _wallsLeft.Add(newWall);
        }

        private void AddRightWall(Vector3 newWall)
        {
            _wallsRight.Add(newWall);
        }

        private void RemoveLeftWall(Vector3 destroyedWall)
        {
            _wallsLeft.Remove(destroyedWall);
        }

        private void RemoveRightWall(Vector3 destroyedWall)
        {
            _wallsRight.Remove(destroyedWall);
        }

        public void AddWall(BuildingDataScript building)
        {
            var buildingPos = building.transform.position;

            if (buildingPos.x > TownCenterPosition.x)
            {
                AddRightWall(buildingPos);
            }
            else
            {
                AddLeftWall(buildingPos);
            }
        }
        
        public void RemoveWall(BuildingDataScript building)
        {
            var buildingPos = building.transform.position;

            if (buildingPos.x > TownCenterPosition.x)
            {
                RemoveRightWall(buildingPos);
            }
            else
            {
                RemoveLeftWall(buildingPos);
            }
        }
        
        // public void DetectWallDestruction(Vector3 destroyedWall)
        // {
        //     if (_wallsRight.Contains(destroyedWall))
        //     {
        //         RemoveWall(destroyedWall);
        //         // Additional logic such as updating UI or game state
        //     }
        // }
    }
}
