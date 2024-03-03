namespace _Scripts.Core.NPC.States
{
    using System;
    using System.Threading;
    using AI;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Utils;

    public class LumberjackGoAndChopState : BaseState<LumberjackFSM>
    {
        private Vector3 _destinationPosition = Vector3.zero;
        
        public LumberjackGoAndChopState(LumberjackFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            _context.IsWalkingToChopTree = true;
            _context.IsChopping = false;
            _context.DestinationOffsetDistance = _context.TreeToChop.TreeWidth / 2;
            _context.DestinationTarget = _context.TreeToChop.gameObject.transform;
            
            _destinationPosition = GetDestinationPosition();
            
            _context.CancellationSource = new CancellationTokenSource();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            _context.IsChopping = false;
            _context.ChopTreeSet = false;
            _context.IsWalkingToChopTree = false;
        }

        public override async void UpdateState()
        {
            if (_context.TreeToChop.IsChoppedDown)
            {
                // Finished chopping tree
                _context.ChangeState(_context.WanderingState);
            }
            
            if (_context.IsWalkingToChopTree)
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
                    
                    _context.IsWalkingToChopTree = false;
                }
                
                return;
            }

            if (!_context.IsChopping)
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.ChopTree);

                try
                {
                    await WaitUntilChopping();
                }
                catch (OperationCanceledException)
                {
                    _context.Debug.Log($"OperationCanceledException for lumberjack: {_context.gameObject.name}");
                }
                finally
                {
                    _context.IsChopping = false;
                    _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
                    
                    // Finished chopping tree
                    _context.ChangeState(_context.WanderingState);
                }
            }
        }
        
        private async UniTask WaitUntilChopping()
        {
            _context.IsChopping = true;
            
            while (!_context.TreeToChop.IsChoppedDown)
            {
                _context.TreeToChop.ChopTree(((LumberjackStats) _context.Stats).ChopSpeed);
                if (_context.TreeToChop.IsChoppedDown)
                {
                    break;
                }
                
                await UniTask.Delay((int) (_context.TimeBetweenChops * 1000), DelayType.DeltaTime, PlayerLoopTiming.Update, _context.CancellationSource.Token);
            }
        }
        
        private Vector3 GetDestinationPosition()
        {
            var targetPosition = _context.DestinationTarget.position;
            var newPosition = Util.Choose(targetPosition.x - _context.DestinationOffsetDistance, targetPosition.x + _context.DestinationOffsetDistance);

            _context.MovingDirection = newPosition >= _context.Position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(newPosition, 0, 0);
        }
    }
}
