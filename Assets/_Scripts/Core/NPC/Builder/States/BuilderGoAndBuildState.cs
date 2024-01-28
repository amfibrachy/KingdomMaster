namespace _Scripts.Core.NPC.States
{
    using System;
    using System.Threading;
    using AI;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class BuilderGoAndBuildState : BaseState<BuilderFSM>
    {
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
            
            _context.IsBuilding = false;
            _context.BuildTargetSet = false;
            _context.IsWalkingToConstructionSite = false;
        }

        public override async void UpdateState()
        {
            if (_context.Site.IsConstructionFinished || _context.Site.IsConstructionCanceled)
            {
                // Finished building or build canceled
                _context.ChangeState(_context.WanderingState);
            }
            
            if (_context.IsWalkingToConstructionSite)
            {
                if (_context.MovingDirection == Direction.Left)
                {
                    _context.AnimationController.TurnLeft();
                }
                else
                {
                    _context.AnimationController.TurnRight();
                }

                _context.AnimationController.PlayAnimation(_context.AnimationController.Walk);
                _context.transform.Translate((int) _context.MovingDirection * _context.Stats.WalkSpeed * Time.deltaTime, 0, 0,
                    Space.World);

                // Reached destination position
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

                try
                {
                    await WaitUntilBuilding();
                }
                catch (OperationCanceledException)
                {
                    _context.Debug.Log($"OperationCanceledException for builder: {_context.gameObject.name}");
                }
                finally
                {
                    _context.IsBuilding = false;
                    _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
                    
                    // Finished building or build canceled
                    _context.ChangeState(_context.WanderingState);
                }
            }
        }
        
        private async UniTask WaitUntilBuilding()
        {
            _context.IsBuilding = true;
            while (!_context.Site.IsConstructionFinished && !_context.Site.IsConstructionCanceled)
            {
                _context.Site.AddProgress(((BuilderStats) _context.Stats).BuildSpeed / 10f);
                if (_context.Site.IsConstructionFinished)
                {
                    break;
                }
                
                await UniTask.Delay(100, DelayType.DeltaTime, PlayerLoopTiming.Update, _context.CancellationSource.Token);
            }
        }
        
        private Vector3 GetDestinationPosition()
        {
            var targetPosition = _context.DestinationTarget.position;
            var newPosition = Random.Range(targetPosition.x - _context.DestinationOffsetMaxDistance, targetPosition.x + _context.DestinationOffsetMaxDistance);
            
            _context.MovingDirection = newPosition >= _context.transform.position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(newPosition, 0, 0);
        }
    }
}
