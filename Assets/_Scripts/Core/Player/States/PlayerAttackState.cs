namespace _Scripts.Core.Player.States
{
    using AI;
    using UnityEngine;

    public class PlayerAttackState : BaseState<PlayerFSM>
    {
        private bool _isWalkAttacking;
        
        public PlayerAttackState(PlayerFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            if (_context.IsPlayerRunning)
            {
                _context.ToggleRun();
            }
        }

        private void OnAttackFinished()
        {
            _isWalkAttacking = false;
            _context.ChangeState(_context.MoveState);
        }

        public override void UpdateState()
        {
            var facingDirection = _context.GetFacingDirection();
            var movingDirection = _context.GetMoveDirection();
            
            if (movingDirection == Direction.Right && !_context.AnimationController.IsAnimationLocked)
            {
                _context.AnimationController.PlayAnimationUninterrupted(facingDirection == Direction.Left
                    ? _context.AnimationController.ReverseWalkAttack
                    : _context.AnimationController.WalkAttack, OnAttackFinished);
                
                _isWalkAttacking = true;
                _context.transform.Translate((int) movingDirection * _context.Speed * Time.deltaTime, 0, 0, Space.World);
            }
            else if (movingDirection == Direction.Left  && !_context.AnimationController.IsAnimationLocked)
            {
                _context.AnimationController.PlayAnimationUninterrupted(facingDirection == Direction.Right
                    ? _context.AnimationController.ReverseWalkAttack
                    : _context.AnimationController.WalkAttack, OnAttackFinished); 
                
                _isWalkAttacking = true;
                _context.transform.Translate((int) movingDirection * _context.Speed * Time.deltaTime, 0, 0, Space.World);
            }
            else
            {
                if (_isWalkAttacking)
                {
                    _context.AnimationController.PlayAnimationUninterrupted(_context.AnimationController.IdleAttack, OnAttackFinished, true, true);
                }
                else
                {
                    _context.AnimationController.PlayAnimationUninterrupted(_context.AnimationController.IdleAttack, OnAttackFinished, true);
                }

                _isWalkAttacking = false;
            }
        }
    }
}
