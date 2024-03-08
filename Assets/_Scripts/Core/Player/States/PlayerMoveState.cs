namespace _Scripts.Core.Player.States
{
    using AI;
    using UnityEngine;
    using Utils;

    public class PlayerMoveState : BaseState<PlayerFSM>
    {
        public PlayerMoveState(PlayerFSM context) : base(context)
        {
        }

        public override void UpdateState()
        {
            var facingDirection = _context.GetFacingDirection();
            
            UpdateFacing(facingDirection);

            if (!Util.IsMouseOverUI())
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
                        _context.SetRunEnabled(false);
                    
                    _context.AnimationController.PlayAnimation(_context.AnimationController.ReverseWalk);
                }
                else
                {
                    if (_context.IsPlayerRunning)
                    {
                        _context.AnimationController.PlayAnimation(_context.AnimationController.Run);
                        
                        _context.DustAnimationController.gameObject.SetActive(true);
                        _context.DustAnimationController.PlayAnimation(_context.AnimationController.Dust);
                    }
                    else
                    {
                        _context.AnimationController.PlayAnimation(_context.AnimationController.Walk);
                    }
                }
            }
            else if (movingDirection == Direction.Left)
            {
                if (facingDirection == Direction.Right)
                {
                    // Avoid running backwards, switch to walking
                    if (_context.IsPlayerRunning)
                        _context.SetRunEnabled(false);
                    
                    _context.AnimationController.PlayAnimation(_context.AnimationController.ReverseWalk);
                }
                else
                {
                    if (_context.IsPlayerRunning)
                    {
                        _context.AnimationController.PlayAnimation(_context.AnimationController.Run);
                        
                        _context.DustAnimationController.gameObject.SetActive(true);
                        _context.DustAnimationController.PlayAnimation(_context.AnimationController.Dust);
                    }
                    else
                    {
                        _context.AnimationController.PlayAnimation(_context.AnimationController.Walk);
                    }
                }
            }
            else
            {
                _context.AnimationController.PlayAnimation(_context.AnimationController.Idle);
            }
            
            _context.transform.Translate((int) movingDirection * _context.CurrentSpeed * Time.deltaTime, 0, 0, Space.World);
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
