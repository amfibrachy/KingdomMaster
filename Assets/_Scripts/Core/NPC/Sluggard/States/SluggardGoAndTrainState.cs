namespace _Scripts.Core.NPC.States
{
    using System;
    using System.Threading;
    using AI;
    using UnityEngine;

    public class SluggardGoAndTrainState : BaseState<SluggardFSM>
    {
        private Vector3 _destinationPosition = Vector3.zero;
        
        public SluggardGoAndTrainState(SluggardFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            _context.IsWalkingToJobBuilding = true;
            _context.DestinationTarget = _context.JobBuilding.EntrancePosition;
            
            _destinationPosition = GetDestinationPosition();
            
            _context.CancellationSource = new CancellationTokenSource();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            _context.JobTrainingTargetSet = false;
            _context.IsWalkingToJobBuilding = false;
        }

        public override void UpdateState()
        {
            if (_context.JobBuilding == null || !_context.JobTrainingTargetSet)
            {
                // Building no longer exists
                _context.ChangeState(_context.WanderingState);
            }
            
            if (_context.IsWalkingToJobBuilding)
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
                    _context.IsWalkingToJobBuilding = false;
                    _context.JobBuilding.StartSluggardJobCreationTask(_context.Job);
                    _context.Dispatch();
                }
            }
        }

        private Vector3 GetDestinationPosition()
        {
            var targetPositionX = _context.DestinationTarget.position.x;
            _context.MovingDirection = targetPositionX >= _context.transform.position.x ? Direction.Right : Direction.Left;
            
            return new Vector3(targetPositionX, 0, 0);
        }
    }
}
