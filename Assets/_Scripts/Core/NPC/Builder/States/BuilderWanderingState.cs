namespace _Scripts.Core.NPC.States
{
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;
    using AI;
    using UnityEngine;

    public class BuilderWanderingState : BaseState<BuilderFSM>
    {
        private bool _isWanderingToDestination;
        private bool _isWaitingInIdle;
        private CancellationToken _token;

        private Direction _movingDirection;
        Vector3 _destinationPosition = Vector3.zero;
        
        public BuilderWanderingState(BuilderFSM context) : base(context)
        {
        }

        public override async void UpdateState()
        {
            if (_isWanderingToDestination)
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
                _context.transform.Translate((int) _movingDirection * _context.Speed * Time.deltaTime, 0, 0,
                    Space.World);

                if (Vector2.Distance(_context.transform.position, _destinationPosition) < 0.1f)
                {
                    _isWanderingToDestination = false;
                }
                
                return;
            }

            if (!_isWaitingInIdle)
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
                
                await WaitInIdle();
                
                _destinationPosition = GetNewDestinationPosition();
                _isWanderingToDestination = true;
            }
        }

        private async Task WaitInIdle()
        {
            _isWaitingInIdle = true;
            var waitTime = Random.Range(0f, _context.IdleWaitMaxTime);

            await Task.Delay((int) (waitTime * 1000), _token);

            _isWaitingInIdle = false;
        }

        private Vector3 GetNewDestinationPosition()
        {
            var targetPosition = _context.WanderingTarget.position;
            var newPosition = Random.Range(targetPosition.x - _context.WanderingMaxDistance, targetPosition.x + _context.WanderingMaxDistance);
            
            _movingDirection = newPosition >= _context.transform.position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(newPosition, 0, 0);
        }
    }
}
