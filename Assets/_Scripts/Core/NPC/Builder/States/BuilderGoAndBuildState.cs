namespace _Scripts.Core.NPC.States
{
    using System.Threading;
    using System.Threading.Tasks;
    using AI;
    using UnityEngine;

    public class BuilderGoAndBuildState : BaseState<BuilderFSM>
    {
        private Direction _movingDirection;
        private Vector3 _destinationPosition = Vector3.zero;
        
        public BuilderGoAndBuildState(BuilderFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            _context.IsWalkingToConstructionSite = true;
            _context.IsBuilding = false;
            _context.DestinationOffsetMaxDistance = _context.Site.BuildingWidth / 2;
            _context.DestinationTarget = _context.Site.gameObject.transform;
            
            _destinationPosition = GetDestinationPosition();
            
            _context.CancellationSource = new CancellationTokenSource();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            _context.IsWalkingToConstructionSite = false;
            _context.IsBuilding = false;
            _context.BuildTargetSet = false;
        }

        public override async void UpdateState()
        {
            if (_context.IsWalkingToConstructionSite)
            {
                if (_movingDirection == Direction.Left)
                {
                    _context.AnimationController.TurnLeft();
                }
                else
                {
                    _context.AnimationController.TurnRight();
                }

                _context.AnimationController.PlayAnimation(_context.AnimationController.Walk);
                _context.transform.Translate((int) _movingDirection * _context.Stats.WalkSpeed * Time.deltaTime, 0, 0,
                    Space.World);

                if (Vector2.Distance(_context.transform.position, _destinationPosition) < 0.1f)
                {
                    if (_context.DestinationTarget.position.x > _context.transform.position.x)
                    {
                        _context.AnimationController.TurnRight();
                    }
                    else
                    {
                        _context.AnimationController.TurnLeft();
                    }
                    
                    _context.IsWalkingToConstructionSite = false;
                }
                
                return;
            }

            if (!_context.IsBuilding)
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.Build);
                await WaitUntilBuilding();
                _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
            }
        }
        
        private async Task WaitUntilBuilding()
        {
            _context.IsBuilding = true;
            while (!_context.Site.IsConstructionFinished)
            {
                _context.Site.AddProgress(((BuilderStats) _context.Stats).BuildSpeed);
                if (_context.Site.IsConstructionFinished)
                {
                    break;
                }
                
                await Task.Delay(1000, _context.CancellationSource.Token);

                if (_context.CancellationSource.Token.IsCancellationRequested)
                    break;
            }
            
            _context.IsBuilding = false;
            // Finished building or build canceled
            _context.ChangeState(_context.WanderingState);
        }
        
        private Vector3 GetDestinationPosition()
        {
            var targetPosition = _context.DestinationTarget.position;
            var newPosition = Random.Range(targetPosition.x - _context.DestinationOffsetMaxDistance, targetPosition.x + _context.DestinationOffsetMaxDistance);
            
            _movingDirection = newPosition >= _context.transform.position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(newPosition, 0, 0);
        }
    }
}
