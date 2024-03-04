namespace _Scripts.Core.NPC.States
{
    using System;
    using System.Threading;
    using AI;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class BuilderWanderingState : BaseState<BuilderFSM>
    {
        private Direction _movingDirection;
        Vector3 _destinationPosition = Vector3.zero;
        
        public BuilderWanderingState(BuilderFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            
            _context.IsWandering = true;
            _context.IsWaitingInIdle = false;

            _destinationPosition = GetNewDestinationPosition();
            
            _context.CancellationSource = new CancellationTokenSource();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            _context.IsWandering = false;
            _context.IsWaitingInIdle = false;
        }

        public override async void UpdateState()
        {
            if (_context.BuildTargetSet)
            {
                _context.ChangeState(_context.GoAndBuildState);
            }
            
            if (_context.IsWandering)
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
                    _context.IsWandering = false;
                }
                
                return;
            }

            if (!_context.IsWaitingInIdle)
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
                
                try
                {
                    await WaitInIdle();
                }
                catch (OperationCanceledException)
                {
                    _context.IsWaitingInIdle = false;
                    return;
                }

                _destinationPosition = GetNewDestinationPosition();
                _context.IsWandering = true;
            }
        }

        private async UniTask WaitInIdle()
        {
            _context.IsWaitingInIdle = true;
            var waitTime = Random.Range(1f, _context.IdleWaitMaxTime);
            
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), DelayType.DeltaTime, PlayerLoopTiming.Update, _context.CancellationSource.Token);
            
            _context.IsWaitingInIdle = false;
        }

        private Vector3 GetNewDestinationPosition()
        {
            var newPosition = Random.Range(_context.BordersController.LeftBorderPosition.x + 2f, _context.BordersController.RightBorderPosition.x - 2f);
            
            _movingDirection = newPosition >= _context.transform.position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(newPosition, 0, 0);
        }
    }
}
