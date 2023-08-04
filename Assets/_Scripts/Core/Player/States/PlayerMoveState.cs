namespace _Scripts.Core.Player.States
{
    using Core.States;
    using UnityEngine;

    public class PlayerMoveState : BaseState<PlayerFSM>
    {
        public PlayerMoveState(PlayerFSM context) : base(context)
        {
        }

        public override void UpdateState()
        {
            var facingDirection = _context.GetFacingDirection();
            
            UpdateFacing(facingDirection);

            if (!_context.IsInBuildMode)
            {
                if (_context.AttackAction.triggered)
                {
                    _context.ChangeState(_context.AttackState);
                    return;
                }
            }

            var movingDirection = _context.GetMoveDirection();

            if (movingDirection == Direction.Right)
            {
                if (facingDirection == Direction.Left)
                {
                    // Avoid running backwards, switch to walking
                    if (_context.IsPlayerRunning)
                        _context.ToggleRun();
                    
                    _context.AnimationController.PlayAnimation(_context.AnimationController.ReverseWalk);
                }
                else
                {
                    _context.AnimationController.PlayAnimation(_context.IsPlayerRunning
                        ? _context.AnimationController.Run
                        : _context.AnimationController.Walk);
                }
            }
            else if (movingDirection == Direction.Left)
            {
                if (facingDirection == Direction.Right)
                {
                    // Avoid running backwards, switch to walking
                    if (_context.IsPlayerRunning)
                        _context.ToggleRun();
                    
                    _context.AnimationController.PlayAnimation(_context.AnimationController.ReverseWalk);
                }
                else
                {
                    _context.AnimationController.PlayAnimation(_context.IsPlayerRunning
                        ? _context.AnimationController.Run
                        : _context.AnimationController.Walk);
                }
            }
            else
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
            }
            
            _context.transform.Translate((int) movingDirection * _context.Speed * Time.deltaTime, 0, 0, Space.World);
        }

        private void UpdateFacing(Direction facing)
        {
            if (facing == Direction.Left)
            {
                _context.AnimationController.TurnLeft();
            }
            else if (facing == Direction.Right)
            {
                _context.AnimationController.TurnRight();
            }
        }
    }
}
